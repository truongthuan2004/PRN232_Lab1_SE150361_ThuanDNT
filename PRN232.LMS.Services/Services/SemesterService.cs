using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Mappings;
using PRN232.LMS.Services.Models;

namespace PRN232.LMS.Services.Services;

public class SemesterService : ISemesterService
{
    private readonly IUnitOfWork _unitOfWork;

    public SemesterService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<SemesterBusinessModel>> GetAllAsync()
    {
        var entities = await _unitOfWork.Semesters.GetAll()
            .OrderBy(x => x.SemesterId)
            .ToListAsync();

        return entities.Select(x => x.ToBusinessModel());
    }

    public async Task<SemesterBusinessModel?> GetByIdAsync(int id)
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
