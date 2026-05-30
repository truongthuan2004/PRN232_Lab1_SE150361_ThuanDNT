using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Services.Common;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Mappings;
using PRN232.LMS.Services.Models;
using PRN232.LMS.Services.Models.Queries;

namespace PRN232.LMS.Services.Services;

public class CourseService : ICourseService
{
    private readonly IUnitOfWork _unitOfWork;

    public CourseService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<CourseBusinessModel>> GetListAsync(CourseListQuery query)
    {
        query.Page = query.Page <= 0 ? 1 : query.Page;
        query.Size = query.Size <= 0 ? 10 : query.Size;

        var source = _unitOfWork.Courses.GetAll().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLowerInvariant();
            source = source.Where(x => x.CourseName.ToLower().Contains(search));
        }

        if (query.SemesterId.HasValue)
        {
            source = source.Where(x => x.SemesterId == query.SemesterId.Value);
        }

        var isDesc = false;
        var sortField = "courseid";

        if (!string.IsNullOrWhiteSpace(query.Sort))
        {
            var sort = query.Sort.Trim();
            isDesc = sort.StartsWith('-');
            sortField = (isDesc ? sort[1..] : sort).ToLowerInvariant();
        }

        source = sortField switch
        {
            "coursename" => isDesc ? source.OrderByDescending(x => x.CourseName) : source.OrderBy(x => x.CourseName),
            "semesterid" => isDesc ? source.OrderByDescending(x => x.SemesterId) : source.OrderBy(x => x.SemesterId),
            _ => isDesc ? source.OrderByDescending(x => x.CourseId) : source.OrderBy(x => x.CourseId)
        };

        var totalItems = await source.CountAsync();

        var entities = await source
            .Skip((query.Page - 1) * query.Size)
            .Take(query.Size)
            .ToListAsync();

        return new PagedResult<CourseBusinessModel>
        {
            Items = entities.Select(x => x.ToBusinessModel()).ToList(),
            Page = query.Page,
            PageSize = query.Size,
            TotalItems = totalItems
        };
    }

    public async Task<CourseBusinessModel?> GetByIdAsync(int id, string? expand = null)
    {
        var entity = await _unitOfWork.Courses.GetByIdAsync(id);
        return entity?.ToBusinessModel();
    }

    public async Task<CourseBusinessModel> CreateAsync(CourseBusinessModel model)
    {
        var entity = model.ToEntity();
        await _unitOfWork.Courses.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return entity.ToBusinessModel();
    }

    public async Task<CourseBusinessModel?> UpdateAsync(int id, CourseBusinessModel model)
    {
        var entity = await _unitOfWork.Courses.GetByIdAsync(id);
        if (entity is null) return null;

        entity.UpdateEntity(model);
        _unitOfWork.Courses.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return entity.ToBusinessModel();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _unitOfWork.Courses.GetByIdAsync(id);
        if (entity is null) return false;

        _unitOfWork.Courses.Remove(entity);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
