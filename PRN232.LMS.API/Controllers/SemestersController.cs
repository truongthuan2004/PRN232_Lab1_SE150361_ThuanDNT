using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Common;
using PRN232.LMS.API.Mappings;
using PRN232.LMS.API.Models.Requests;
using PRN232.LMS.API.Models.Responses;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models.Queries;

namespace PRN232.LMS.API.Controllers;

/// <summary>
/// Manage semester resources.
/// </summary>
[ApiController]
[Route("api/semesters")]
[Produces("application/json")]
public class SemestersController : ControllerBase
{
    private readonly ISemesterService _semesterService;

    public SemestersController(ISemesterService semesterService)
    {
        _semesterService = semesterService;
    }

    /// <summary>
    /// Get paginated list of semesters.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedData<object>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedData<object>>>> GetList(
        [FromQuery] string? search,
        [FromQuery] string? sort,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] string? fields = null,
        [FromQuery] string? expand = null)
    {
        var result = await _semesterService.GetListAsync(new SemesterListQuery
        {
            Search = search,
            Sort = sort,
            Page = page,
            Size = size
        });

        var items = result.Items.Select(x => x.ToResponseObject(fields)).ToList();

        var data = new PagedData<object>
        {
            Items = items,
            Pagination = new PaginationMetadata
            {
                Page = result.Page,
                PageSize = result.PageSize,
                TotalItems = result.TotalItems,
                TotalPages = result.TotalPages
            }
        };

        return Ok(ApiResponse<PagedData<object>>.Ok(data));
    }

    /// <summary>
    /// Get a semester by id.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<SemesterResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<SemesterResponse>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<SemesterResponse>>> GetById(int id, [FromQuery] string? expand = null)
    {
        var item = await _semesterService.GetByIdAsync(id, expand);
        if (item is null)
        {
            return NotFound(ApiResponse<SemesterResponse>.Fail("Semester not found"));
        }

        return Ok(ApiResponse<SemesterResponse>.Ok(item.ToResponse()));
    }

    /// <summary>
    /// Create a new semester.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<SemesterResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<SemesterResponse>), StatusCodes.Status400BadRequest)]
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

    /// <summary>
    /// Update an existing semester.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<SemesterResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<SemesterResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<SemesterResponse>), StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// Delete a semester by id.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
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
