// Immutable by design - the logging pipeline cannot corrupt this
public record EnrollmentRecord(string StudentId, string CourseCode, DateTime EnrolledAt);

public class Course
{
    public required string Code {get; init;}

    public required string Title
    {
        get;
        set => field = !string.IsNullOrWhiteSpace(value)
            ? value
            : throw new ArgumentException("Title cannot be empty or whitespace.", nameof(value));
    }

    // C# 14 Auto-property validation using 'field'
    public int Capacity
    {
         get;
        set => field = value > 0
            ? value
            : throw new ArgumentOutOfRangeException(nameof(value), "System constraint: Capacity must be grater than zero.");
    }

    public int EnrolledCount {get; set;}
}