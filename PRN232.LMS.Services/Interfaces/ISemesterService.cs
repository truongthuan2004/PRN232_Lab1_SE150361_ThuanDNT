using PRN232.LMS.Services.Models;

namespace PRN232.LMS.Services.Interfaces;

public interface ISemesterService
{
    Task<IEnumerable<SemesterBusinessModel>> GetAllAsync();
    Task<SemesterBusinessModel?> GetByIdAsync(int id);
    Task<SemesterBusinessModel> CreateAsync(SemesterBusinessModel model);
    Task<SemesterBusinessModel?> UpdateAsync(int id, SemesterBusinessModel model);
    Task<bool> DeleteAsync(int id);
}
