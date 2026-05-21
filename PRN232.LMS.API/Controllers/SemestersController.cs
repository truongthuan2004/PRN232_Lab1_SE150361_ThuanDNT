using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Common;
using PRN232.LMS.API.Mappings;
using PRN232.LMS.API.Models.Requests;
using PRN232.LMS.API.Models.Responses;
using PRN232.LMS.Services.Interfaces;

namespace PRN232.LMS.API.Controllers;

[ApiController]
[Route("api/semesters")]
public class SemestersController : ControllerBase
{
    private readonly ISemesterService _semesterService;

    public SemestersController(ISemesterService semesterService)
    {
        _semesterService = semesterService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<SemesterResponse>>>> GetAll()
    {
        var items = (await _semesterService.GetAllAsync()).Select(x => x.ToResponse());
        return Ok(ApiResponse<IEnumerable<SemesterResponse>>.Ok(items));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<SemesterResponse>>> GetById(int id)
    {
        var item = await _semesterService.GetByIdAsync(id);
        if (item is null)
        {
            return NotFound(ApiResponse<SemesterResponse>.Fail("Semester not found"));
        }

        return Ok(ApiResponse<SemesterResponse>.Ok(item.ToResponse()));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<SemesterResponse>>> Create([FromBody] CreateSemesterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<SemesterResponse>.Fail("Invalid request", ModelState));
        }

        var created = await _semesterService.CreateAsync(request.ToBusinessModel());
        return CreatedAtAction(nameof(GetById), new { id = created.SemesterId },
            ApiResponse<SemesterResponse>.Ok(created.ToResponse(), "Semester created successfully"));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<SemesterResponse>>> Update(int id, [FromBody] UpdateSemesterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<SemesterResponse>.Fail("Invalid request", ModelState));
        }

        var updated = await _semesterService.UpdateAsync(id, request.ToBusinessModel());
        if (updated is null)
        {
            return NotFound(ApiResponse<SemesterResponse>.Fail("Semester not found"));
        }

        return Ok(ApiResponse<SemesterResponse>.Ok(updated.ToResponse(), "Semester updated successfully"));
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        var deleted = await _semesterService.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound(ApiResponse<object>.Fail("Semester not found"));
        }

        return Ok(ApiResponse<object>.Ok(new { }, "Semester deleted successfully"));
    }
}
