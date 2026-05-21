using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Mappings;
using PRN232.LMS.Services.Models;

namespace PRN232.LMS.Services.Services;

public class CourseService : ICourseService
{
    private readonly IUnitOfWork _unitOfWork;

    public CourseService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<CourseBusinessModel>> GetAllAsync()
    {
        var entities = await _unitOfWork.Courses.GetAll().OrderBy(x => x.CourseId).ToListAsync();
        return entities.Select(x => x.ToBusinessModel());
    }

    public async Task<CourseBusinessModel?> GetByIdAsync(int id)
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
