using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Services.Common;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Mappings;
using PRN232.LMS.Services.Models;
using PRN232.LMS.Services.Models.Queries;

namespace PRN232.LMS.Services.Services;

public class SemesterService : ISemesterService
{
    private readonly IUnitOfWork _unitOfWork;

    public SemesterService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<SemesterBusinessModel>> GetListAsync(SemesterListQuery query)
    {
        query.Page = query.Page <= 0 ? 1 : query.Page;
        query.Size = query.Size <= 0 ? 10 : query.Size;

        var source = _unitOfWork.Semesters.GetAll().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLowerInvariant();
            source = source.Where(x => x.SemesterName.ToLower().Contains(search));
        }

        var isDesc = false;
        var sortField = "semesterid";

        if (!string.IsNullOrWhiteSpace(query.Sort))
        {
            var sort = query.Sort.Trim();
            isDesc = sort.StartsWith('-');
            sortField = (isDesc ? sort[1..] : sort).ToLowerInvariant();
        }

        source = sortField switch
        {
            "semestername" => isDesc ? source.OrderByDescending(x => x.SemesterName) : source.OrderBy(x => x.SemesterName),
            "startdate" => isDesc ? source.OrderByDescending(x => x.StartDate) : source.OrderBy(x => x.StartDate),
            "enddate" => isDesc ? source.OrderByDescending(x => x.EndDate) : source.OrderBy(x => x.EndDate),
            _ => isDesc ? source.OrderByDescending(x => x.SemesterId) : source.OrderBy(x => x.SemesterId)
        };

        var totalItems = await source.CountAsync();

        var entities = await source
            .Skip((query.Page - 1) * query.Size)
            .Take(query.Size)
            .ToListAsync();

        return new PagedResult<SemesterBusinessModel>
        {
            Items = entities.Select(x => x.ToBusinessModel()).ToList(),
            Page = query.Page,
            PageSize = query.Size,
            TotalItems = totalItems
        };
    }

    public async Task<SemesterBusinessModel?> GetByIdAsync(int id, string? expand = null)
    {
        var entity = await _unitOfWork.Semesters.GetByIdAsync(id);
        return entity?.ToBusinessModel();
    }

    public async Task<SemesterBusinessModel> CreateAsync(SemesterBusinessModel model)
    {
        var entity = model.ToEntity();
        await _unitOfWork.Semesters.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return entity.ToBusinessModel();
    }

    public async Task<SemesterBusinessModel?> UpdateAsync(int id, SemesterBusinessModel model)
    {
        var entity = await _unitOfWork.Semesters.GetByIdAsync(id);
        if (entity is null) return null;

        entity.UpdateEntity(model);
        _unitOfWork.Semesters.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return entity.ToBusinessModel();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _unitOfWork.Semesters.GetByIdAsync(id);
        if (entity is null) return false;

        _unitOfWork.Semesters.Remove(entity);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
