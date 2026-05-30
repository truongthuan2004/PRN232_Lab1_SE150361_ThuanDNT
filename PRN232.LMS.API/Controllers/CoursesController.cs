using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Common;
using PRN232.LMS.API.Mappings;
using PRN232.LMS.API.Models.Requests;
using PRN232.LMS.API.Models.Responses;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models.Queries;

namespace PRN232.LMS.API.Controllers;

/// <summary>
/// Manage course resources.
/// </summary>
[ApiController]
[Route("api/courses")]
[Produces("application/json")]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;
    private readonly IEnrollmentService _enrollmentService;

    public CoursesController(ICourseService courseService, IEnrollmentService enrollmentService)
    {
        _courseService = courseService;
        _enrollmentService = enrollmentService;
    }

    /// <summary>
    /// Get paginated list of courses.
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
        [FromQuery] int? semesterId = null)
    {
        var result = await _courseService.GetListAsync(new CourseListQuery
        {
            Search = search,
            Sort = sort,
            Page = page,
            Size = size,
            SemesterId = semesterId
        });

        var items = result.Items
            .Select(x => x.ToResponseObject(fields))
            .ToList();

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
    /// Get a course by id.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<CourseResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<CourseResponse>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<CourseResponse>>> GetById(int id, [FromQuery] string? expand = null)
    {
        var item = await _courseService.GetByIdAsync(id, expand);
        if (item is null)
        {
            return NotFound(ApiResponse<CourseResponse>.Fail("Course not found"));
        }

        return Ok(ApiResponse<CourseResponse>.Ok(item.ToResponse()));
    }

    /// <summary>
    /// Get paginated enrollments by course id.
    /// </summary>
    [HttpGet("{id:int}/enrollments")]
    [ProducesResponseType(typeof(ApiResponse<PagedData<object>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<PagedData<object>>>> GetEnrollmentsByCourseId(
        int id,
        [FromQuery] string? search,
        [FromQuery] string? sort,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] string? fields = null,
        [FromQuery] string? expand = null)
    {
        var course = await _courseService.GetByIdAsync(id);
        if (course is null)
        {
            return NotFound(ApiResponse<object>.Fail("Course not found"));
        }

        var result = await _enrollmentService.GetListAsync(new EnrollmentListQuery
        {
            Search = search,
            Sort = sort,
            Page = page,
            Size = size,
            Fields = fields,
            Expand = expand,
            CourseId = id
        });

        var items = result.Items
            .Select(x => x.ToResponseObject(fields))
            .ToList();

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
    /// Create a new course.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CourseResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<CourseResponse>), StatusCodes.Status400BadRequest)]
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

    /// <summary>
    /// Update an existing course.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<CourseResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<CourseResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<CourseResponse>), StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// Delete a course by id.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
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
