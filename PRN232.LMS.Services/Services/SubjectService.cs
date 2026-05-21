using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Mappings;
using PRN232.LMS.Services.Models;

namespace PRN232.LMS.Services.Services;

public class SubjectService : ISubjectService
{
    private readonly IUnitOfWork _unitOfWork;

    public SubjectService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<SubjectBusinessModel>> GetAllAsync()
    {
        var entities = await _unitOfWork.Subjects.GetAll().OrderBy(x => x.SubjectId).ToListAsync();
        return entities.Select(x => x.ToBusinessModel());
    }

    public async Task<SubjectBusinessModel?> GetByIdAsync(int id)
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
