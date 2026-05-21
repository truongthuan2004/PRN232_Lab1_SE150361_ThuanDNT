using PRN232.LMS.Services.Models;

namespace PRN232.LMS.Services.Interfaces;

public interface ISubjectService
{
    Task<IEnumerable<SubjectBusinessModel>> GetAllAsync();
    Task<SubjectBusinessModel?> GetByIdAsync(int id);
    Task<SubjectBusinessModel> CreateAsync(SubjectBusinessModel model);
    Task<SubjectBusinessModel?> UpdateAsync(int id, SubjectBusinessModel model);
    Task<bool> DeleteAsync(int id);
}
