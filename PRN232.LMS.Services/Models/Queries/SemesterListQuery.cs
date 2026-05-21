namespace PRN232.LMS.Services.Models.Queries;

public class SemesterListQuery
{
    public string? Search { get; set; }
    public string? Sort { get; set; }
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 10;
}
