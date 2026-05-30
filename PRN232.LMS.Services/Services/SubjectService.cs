using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Services.Common;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Mappings;
using PRN232.LMS.Services.Models;
using PRN232.LMS.Services.Models.Queries;

namespace PRN232.LMS.Services.Services;

public class SubjectService : ISubjectService
{
    private readonly IUnitOfWork _unitOfWork;

    public SubjectService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<SubjectBusinessModel>> GetListAsync(SubjectListQuery query)
    {
        query.Page = query.Page <= 0 ? 1 : query.Page;
        query.Size = query.Size <= 0 ? 10 : query.Size;

        var source = _unitOfWork.Subjects.GetAll().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLowerInvariant();
            source = source.Where(x => x.SubjectCode.ToLower().Contains(search) || x.SubjectName.ToLower().Contains(search));
        }

        if (query.Credit.HasValue)
        {
            source = source.Where(x => x.Credit == query.Credit.Value);
        }

        var isDesc = false;
        var sortField = "subjectid";

        if (!string.IsNullOrWhiteSpace(query.Sort))
        {
            var sort = query.Sort.Trim();
            isDesc = sort.StartsWith('-');
            sortField = (isDesc ? sort[1..] : sort).ToLowerInvariant();
        }

        source = sortField switch
        {
            "subjectcode" => isDesc ? source.OrderByDescending(x => x.SubjectCode) : source.OrderBy(x => x.SubjectCode),
            "subjectname" => isDesc ? source.OrderByDescending(x => x.SubjectName) : source.OrderBy(x => x.SubjectName),
            "credit" => isDesc ? source.OrderByDescending(x => x.Credit) : source.OrderBy(x => x.Credit),
            _ => isDesc ? source.OrderByDescending(x => x.SubjectId) : source.OrderBy(x => x.SubjectId)
        };

        var totalItems = await source.CountAsync();

        var entities = await source
            .Skip((query.Page - 1) * query.Size)
            .Take(query.Size)
            .ToListAsync();

        return new PagedResult<SubjectBusinessModel>
        {
            Items = entities.Select(x => x.ToBusinessModel()).ToList(),
            Page = query.Page,
            PageSize = query.Size,
            TotalItems = totalItems
        };
    }

    public async Task<SubjectBusinessModel?> GetByIdAsync(int id, string? expand = null)
    {
        var entity = await _unitOfWork.Subjects.GetByIdAsync(id);
        return entity?.ToBusinessModel();
    }

    public async Task<SubjectBusinessModel> CreateAsync(SubjectBusinessModel model)
    {
        var entity = model.ToEntity();
        await _unitOfWork.Subjects.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return entity.ToBusinessModel();
    }

    public async Task<SubjectBusinessModel?> UpdateAsync(int id, SubjectBusinessModel model)
    {
        var entity = await _unitOfWork.Subjects.GetByIdAsync(id);
        if (entity is null) return null;

        entity.UpdateEntity(model);
        _unitOfWork.Subjects.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return entity.ToBusinessModel();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _unitOfWork.Subjects.GetByIdAsync(id);
        if (entity is null) return false;

        _unitOfWork.Subjects.Remove(entity);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
