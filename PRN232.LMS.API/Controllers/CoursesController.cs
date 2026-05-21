using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Common;
using PRN232.LMS.API.Mappings;
using PRN232.LMS.API.Models.Requests;
using PRN232.LMS.API.Models.Responses;
using PRN232.LMS.Services.Interfaces;

namespace PRN232.LMS.API.Controllers;

[ApiController]
[Route("api/courses")]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;

    public CoursesController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<CourseResponse>>>> GetAll()
    {
        var items = (await _courseService.GetAllAsync()).Select(x => x.ToResponse());
        return Ok(ApiResponse<IEnumerable<CourseResponse>>.Ok(items));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<CourseResponse>>> GetById(int id)
    {
        var item = await _courseService.GetByIdAsync(id);
        if (item is null)
        {
            return NotFound(ApiResponse<CourseResponse>.Fail("Course not found"));
        }

        return Ok(ApiResponse<CourseResponse>.Ok(item.ToResponse()));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<CourseResponse>>> Create([FromBody] CreateCourseRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<CourseResponse>.Fail("Invalid request", ModelState));
        }

        var created = await _courseService.CreateAsync(request.ToBusinessModel());
        return CreatedAtAction(nameof(GetById), new { id = created.CourseId },
            ApiResponse<CourseResponse>.Ok(created.ToResponse(), "Course created successfully"));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<CourseResponse>>> Update(int id, [FromBody] UpdateCourseRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<CourseResponse>.Fail("Invalid request", ModelState));
        }

        var updated = await _courseService.UpdateAsync(id, request.ToBusinessModel());
        if (updated is null)
        {
            return NotFound(ApiResponse<CourseResponse>.Fail("Course not found"));
        }

        return Ok(ApiResponse<CourseResponse>.Ok(updated.ToResponse(), "Course updated successfully"));
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        var deleted = await _courseService.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound(ApiResponse<object>.Fail("Course not found"));
        }

        return Ok(ApiResponse<object>.Ok(new { }, "Course deleted successfully"));
    }
}
