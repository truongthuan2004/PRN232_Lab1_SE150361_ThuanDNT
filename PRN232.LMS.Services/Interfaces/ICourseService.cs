using PRN232.LMS.Services.Models;

namespace PRN232.LMS.Services.Interfaces;

public interface ICourseService
{
    Task<IEnumerable<CourseBusinessModel>> GetAllAsync();
    Task<CourseBusinessModel?> GetByIdAsync(int id);
    Task<CourseBusinessModel> CreateAsync(CourseBusinessModel model);
    Task<CourseBusinessModel?> UpdateAsync(int id, CourseBusinessModel model);
    Task<bool> DeleteAsync(int id);
}
