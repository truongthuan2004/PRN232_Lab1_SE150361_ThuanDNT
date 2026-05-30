using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Common;
using PRN232.LMS.API.Mappings;
using PRN232.LMS.API.Models.Requests;
using PRN232.LMS.API.Models.Responses;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models.Queries;

namespace PRN232.LMS.API.Controllers;

/// <summary>
/// Manage subject resources.
/// </summary>
[ApiController]
[Route("api/subjects")]
[Produces("application/json")]
public class SubjectsController : ControllerBase
{
    private readonly ISubjectService _subjectService;

    public SubjectsController(ISubjectService subjectService)
    {
        _subjectService = subjectService;
    }

    /// <summary>
    /// Get paginated list of subjects.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedData<object>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedData<object>>>> GetList(
        [FromQuery] string? search,
        [FromQuery] string? sort,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] string? fields = null,
        [FromQuery] string? expand = null,
        [FromQuery] int? credit = null)
    {
        var result = await _subjectService.GetListAsync(new SubjectListQuery
        {
            Search = search,
            Sort = sort,
            Page = page,
            Size = size,
            Credit = credit
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
    /// Get a subject by id.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<SubjectResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<SubjectResponse>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<SubjectResponse>>> GetById(int id, [FromQuery] string? expand = null)
    {
        var item = await _subjectService.GetByIdAsync(id, expand);
        if (item is null)
        {
            return NotFound(ApiResponse<SubjectResponse>.Fail("Subject not found"));
        }

        return Ok(ApiResponse<SubjectResponse>.Ok(item.ToResponse()));
    }

    /// <summary>
    /// Create a new subject.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<SubjectResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<SubjectResponse>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<SubjectResponse>>> Create([FromBody] CreateSubjectRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<SubjectResponse>.Fail("Invalid request", ModelState));
        }

        var created = await _subjectService.CreateAsync(request.ToBusinessModel());
        return CreatedAtAction(nameof(GetById), new { id = created.SubjectId },
            ApiResponse<SubjectResponse>.Ok(created.ToResponse(), "Subject created successfully"));
    }

    /// <summary>
    /// Update an existing subject.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<SubjectResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<SubjectResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<SubjectResponse>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<SubjectResponse>>> Update(int id, [FromBody] UpdateSubjectRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<SubjectResponse>.Fail("Invalid request", ModelState));
        }

        var updated = await _subjectService.UpdateAsync(id, request.ToBusinessModel());
        if (updated is null)
        {
            return NotFound(ApiResponse<SubjectResponse>.Fail("Subject not found"));
        }

        return Ok(ApiResponse<SubjectResponse>.Ok(updated.ToResponse(), "Subject updated successfully"));
    }

    /// <summary>
    /// Delete a subject by id.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        var deleted = await _subjectService.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound(ApiResponse<object>.Fail("Subject not found"));
        }

        return Ok(ApiResponse<object>.Ok(new { }, "Subject deleted successfully"));
    }
}
