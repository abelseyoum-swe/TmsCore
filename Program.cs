using System.Diagnostics;

// ==== Exercise 1: The First Safety Net (LP 1.1: Environment + Null Safety) ====

// == Step 1 - See What the Compiler Catches ==
// This is how the legacy system declared region - no indication it could be empty
// string region = null; // Compiler warning CS8600
// Console.WriteLine(region.ToUpper()); // Compiler warning CS8602

// == Step 2 - Fix It Three Ways ==
// // Declare the variable as nullable with '?'
// This tells the compiler: "I know this might be null. I accept responsibility."

string? region = null;
// Null-conditional operator '?.' — skip the call if null
// If region is null, ToUpper() never executes. No crash.
string? upperRegion = region?.ToUpper();
Console.WriteLine($"Region (conditional): {upperRegion}");
// Null-coalescing operator '??' — provide a fallback value
// If region is null, use "Unassigned" instead.
string displayRegion = region ?? "Unassigned";
Console.WriteLine($"Region (coalesced): {displayRegion}");
// Null-coalescing assignment '??=' — assign only if currently null
// Useful for lazy initialization.
region ??= "Addis Ababa";
Console.WriteLine($"Region (assigned): {region}");

// == Step 3 - Declare Your First TMS Variables ==
string studentName = "Abeba";
string studentId = "STU-001";
int enrollmentCount = 3;
decimal grantAmount = 1999.99m;  // 'm' suffix marks a decimal literal
DateTime enrolledAt = DateTime.UtcNow;
string? campusRegion = null;

Console.WriteLine($"Student: {studentName} ({studentId})");
Console.WriteLine($"Courses: {enrollmentCount}");
Console.WriteLine($"Grant: {grantAmount:F2}");
Console.WriteLine($"Enrolled: {enrolledAt:yyyy-MM-dd}");
Console.WriteLine($"Campus: {campusRegion ?? "Not assigned"}");


// ==== Exercise 2:  The Ministry Audit Failure (LO 1.2: Primitives) ====

// == Step 1 - See the Bug ==
// Legacy implementation — the bug that caused the audit failure
// double grantPerStudent = 1999.99;
// double totalAllocation = grantPerStudent * 100_000;
// Console.WriteLine($"Total allocated (double): {totalAllocation}");

// == Step 2 - Fix It ==
// Fixed implementation - exact financial math
decimal grantPerStudent = 1999.99m;
decimal totalAllocation = grantPerStudent * 100_000m;

Console.WriteLine($"Total allocated (decimal): {totalAllocation}");
Console.WriteLine($"Total allocated (formatted): {totalAllocation:F2}");


// ==== Exercise 3: Pipeline Data Corruption (lo 1.3 & 1.4: Encapsulation) ====

// Legacy implementation - what the logging service did to the data
// public class Enrollment
// {
//     public string StudentId {get; set;} = string.Empty;
//     public string CourseCode {get; set;} = string.Empty;
//     public DateTime ProcessedAt {get; set;}
// }

// Somewhere in the logging pipeline:
// enrollment.CourseCode = null; // <- No compiler error. Data silently corrupted.

// == Step 1 - Understand Why Records Exist ==
// == Step 2 - Create the Domain Models File
// Check Models.cs file for EnrollmentRecord
var enrollment = new EnrollmentRecord("STU-001", "CS-401", DateTime.UtcNow);
Console.WriteLine(enrollment);

// Try to mutate it - uncomment this line and see the compiler error:
// enrollment.CourseCode = "HACKED"; // ERROR: init-only property

// Non-destructive copy - create a NEW record with one field changed
var corrected = enrollment with {CourseCode = "CS-402"};
Console.WriteLine(corrected);

// Value equality - two records with the same data are equal
var duplicate = new EnrollmentRecord("STU-001", "CS-401", enrollment.EnrolledAt);
Console.WriteLine($"Same data? {enrollment == duplicate}"); // True


// ==== Exercise 3 - Part 2: Course Capacity with the field Keyword ====

// Legacy Pre-C# 14 Implementation (Verbose)
// public class Course
// {
//     private int _capacity; // Manual backing field

//     private int Capacity
//     {
//         get => _capacity;
//         set
//         {
//             if (value <= 0)
//                 throw new ArgumentOutOfRangeException("Capacity must be positive.");
//             _capacity = value;
//         }
//     }
// }

var course = new Course {Code = "CS-401", Title = "Advanced C#", Capacity = 30};
Console.WriteLine($"Course: {course.Title} (Capacity: {course.Capacity})");

// Invalid capacity - should throw
try
{
    course.Capacity = -5;
}
catch (ArgumentOutOfRangeException ex)
{
    Console.Write($"\nCaught: {ex.Message}");
}

// Invalid title - should throw
try
{
    course.Title = "";
}
catch (ArgumentException ex)
{
    Console.WriteLine($"\nCaught: {ex.Message}");
}

// ==== Exercise 3 - Part 3: Student Model ====

var s = new Student {Id = "S1", Name = "Abeba", Age = 20, GPA = 3.8m};
Console.WriteLine($"Student: {s.Name}, GPA: {s.GPA}");

// These should throw - try each one:
// new Student {Id = "S2", Name = "", Age = 20, GPA = 3.0m};

// new Student {Id = "S3", Name = "Test", Age = 12, GPA = 3.0m};

// new Student {Id = "S4", Name = "Test", Age = 20, GPA = 5.0m};


// ==== Exercise 3B: Interface Contract Wiring (LO 1.4: OOP Contracts) ====

// == Step 1 - Define the Contract ==
// Check Models.cs for IGradable interface

// == Step 2 - Implement It on Two Assessment Types ==
// Check Models.cs for Quiz and LabAssignment classes

// == Step 3 - Write the Polymorphic Report ==
void PrintGradeReport(IEnumerable<IGradable> assessments)
{
    Console.WriteLine("--- Grade Report ---");
    foreach (var item in assessments)
    {
        Console.WriteLine($"{item.Title}: {item.CalculateGrade():F2}%");
    }
}

// Test it - one array holds two completely different types
IGradable[] cohortAssessments = [
    new Quiz {Title = "C# Basics", CorrectAnswers = 18, TotalQuestions = 20},
    new LabAssignment {Title = "Registration API", FunctionalityScore = 90m, CodeQualityScore = 85m}
];

PrintGradeReport(cohortAssessments);

// ==== Exercise 4: Defining the "Pyramid of Dom" (LO 1.6: Pattern Matching & Guards) ====

// if (student != null)
// {
//     if (course != null)
//     {
//         if (course.Capacity > 0)
//         {
//             // Success buried three levels deep
//         }
//     }
// }

// == Step 1 - Build the Enrollment Service ==
// Check EnrollmentService.cs

// == Step 2 - Test It ==
var service = new EnrollmentService();

// Test 1: Valid registration
var validStudent = new Student {Id = "S1", Name="Abeba", Age = 20, GPA = 3.8m};
var validCourse = new Course {Code = "CS-401", Title = "Advanced C#", Capacity = 30};
var result = service.ProcessRegistration(validStudent, validCourse);
Console.WriteLine($"Enrolled: {result.StudentId} in {result.CourseCode}");

// Test 2: Null Student should throw
try
{
    service.ProcessRegistration(null, validCourse);
}
catch (ArgumentNullException ex)
{
    Console.WriteLine($"Guard caught: {ex.ParamName}");
}

// Test 3: Full Course should throw
var fullCourse = new Course {Code = "CS-402", Title = "Full Course", Capacity = 1};
fullCourse.EnrolledCount = 1;
try
{
    service.ProcessRegistration(validStudent, fullCourse);
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"Business rule: {ex.Message}");
}


// ==== Exercise 5: The Analytics Dashboard (LO 1.5: Collections & LINQ) ====

// == Step 1 - Create the Student Data ==
// C# 12+ Collection Expressions the modern way to initialize lists
List<Student> students = [
    new Student { Id = "S1", Name = "Abeba", Age = 22, GPA = 3.8m },
    new Student { Id = "S2", Name = "Kidane", Age = 21, GPA = 2.4m },
    new Student { Id = "S3", Name = "Dawit", Age = 20, GPA = 3.1m },
    new Student { Id = "S4", Name = "Sara", Age = 23, GPA = 3.9m },
    new Student { Id = "S5", Name = "Frehiwot", Age = 19, GPA = 2.0m },
    new Student { Id = "S6", Name = "Yonas", Age = 24, GPA = 3.5m },
    new Student { Id = "S7", Name = "Meron", Age = 22, GPA = 1.8m },
    new Student { Id = "S8", Name = "Tesfaye", Age = 21, GPA = 2.9m }
];

// == Step 2 - Build the Honors Leaderboard ==
var leaderboard = students
// TODO 1: Extract students where GPA is >= 3.5m
    .Where(s => s.GPA >= 3.5m)
// TODO 2: Sort the remaining students by GPA descending
    .OrderByDescending(s => s.GPA)
// TODO 3: Project the result so we only keep the 'Name' string
    .Select(s => s.Name)
// TODO 4: Materialize the lazy query into a concrete List
    .ToList();

Console.WriteLine($"Found {leaderboard.Count} Honors Students:");
foreach (var name in leaderboard)
{
    Console.WriteLine($"- {name}");
}

// == Step 3 - Class Average ==
// TODO 5: Use LINQ to calculate the average GPA across all students.
//      Format it to 2 decimal places using :F2.
decimal averageGpa = students.Average(s => s.GPA);
Console.WriteLine($"\nClass Average GPA: {averageGpa:F2}");

// == Step 4 - Group by Academic Standing ==
// TODO 6: Use .GroupBy with a switch expression to classify each student.
// GPA >= 3.5 → "Honors", >= 2.5 → "Good Standing",
// >= 2.0 → "Probation", < 2.0 → "Academic Warning"

var standingGroups = students.GroupBy(s => s.GPA switch
{
    >= 3.5m => "Honors",
    >= 2.5m => "Good Standing",
    >= 2.0m => "Probation",
    _ => "Academic Warning"
});

Console.WriteLine("\n--- Academic Standing Report ---");
foreach (var group in standingGroups)
{
    Console.WriteLine($"\n{group.Key} ({group.Count()}):");
    foreach (var student in group)
    {
        Console.WriteLine($" {student.Name} GPA: {student.GPA}");
    }
}

// == Step 5 - Collection Expressions with Spread ==
// TODO 7: Use the spread operator (..) to merge two arrays and append a value.
string[] backendCourses = ["C#", "ASP.NET Core"];
string[] frontendCourses = ["TypeScript", "Angular"];
string[] allCourses =  [
    ..backendCourses,
    ..frontendCourses,
    "Capstone"
    ];
Console.WriteLine($"\nFull curriculum: {string.Join(", ", allCourses)}");


// ==== Exercise 6: Connection Dropping Under Load (LO 1.7: Async/Await) ====

// == Step 1 - See Thread Starvation in Numbers ==
// Simulate 5 database calls, each taking 300ms

// THE WRONG WAY: Blocking with Thread.Sleep
var sw = Stopwatch.StartNew();
for (int i = 0; i < 5; i++)
{
    Thread.Sleep(300); // Thread is HELD for 300ms cannot serve anyone else
}
Console.WriteLine($"Blocking sequential: {sw.ElapsedMilliseconds}ms");

// ASYNC BUT STILL SEQUENTIAL: Thread released, but calls are one-at-a-time
sw.Restart();
for (int i = 0; i < 5; i++)
{
    await Task.Delay(300); // Thread released while waiting but still sequential
}
Console.WriteLine($"Async sequential: {sw.ElapsedMilliseconds}ms");

// THE RIGHT WAY: Async parallel all 5 start simultaneously
sw.Restart();
var tasks = Enumerable.Range(0, 5).Select(_ => Task.Delay(300));
await Task.WhenAll(tasks);
Console.WriteLine($"Async parallel: {sw.ElapsedMilliseconds}ms");

// == Step 2 - Build the TMS Student Fetcher ==
async Task<Student> FetchStudentAsync(string id)
{
    Console.WriteLine($" Fetching {id}...");
    await Task.Delay(300); // Simulate database latency
    return new Student
    {
        Id = id,
        Name = $"Student-{id}",
        Age = 20,
        GPA = id switch
        {
            "S1" => 3.8m,
            "S2" => 2.4m,
            "S3" => 3.5m,
            "S4" => 1.9m,
            "S5" => 3.2m,
            _ => 2.5m
        }
    };
}

async Task<Course> FetchCourseAsync(string code)
{
    Console.WriteLine($" Fetching course {code}...");
    await Task.Delay(200); // Simulate database latency
    return new Course
    {
        Code = code,
        Title = $"Course-{code}",
        Capacity = code switch
        {
            "CRS-101" => 2,
            "CRS-201" => 30,
            "CRS-301" => 15,
            _ => 25
        }
    };
}

// == Step 3 - Load in Parallel ==
sw.Restart();

// Start all fetches simultaneously students AND courses
string[] studentIds = ["S1", "S2", "S3", "S4", "S5"];
string[] courseCodes = ["CRS-101", "CRS-201", "CRS-301"];

var studentTasks = studentIds.Select(id => FetchStudentAsync(id));
var courseTasks = courseCodes.Select(code => FetchCourseAsync(code));

// Both arrays load concurrently
Student[] studentsNew = await Task.WhenAll(studentTasks);
Course[] courses = await Task.WhenAll(courseTasks);

Console.WriteLine($"\nLoaded {studentsNew.Length} students and {courses.Length} courses in {sw.ElapsedMilliseconds}ms");
foreach (var student in students)
{
    Console.WriteLine($"{student.Name} GPA: {student.GPA}");
}

// ==== Exercise 6 Part B: The TMS Enrollment Engine ====

var enrollCourse = new Course { Code = "CRS-101", Title = "C# Mastery", Capacity = 2};
var enrollService = new EnrollmentService();

var enrollments = new List<EnrollmentRecord>();
var failures = new List<string>();

sw.Restart();

foreach (var student in students)
{
    try
    {
        var record = enrollService.ProcessRegistration(student, enrollCourse);
        enrollCourse.EnrolledCount++;
        enrollments.Add(record);
        Console.WriteLine($"Enrolled: {student.Name}");
    }
    catch (InvalidOperationException ex)
    {
        failures.Add($"{student.Name}: {ex.Message}");
        Console.WriteLine($"Rejected: {student.Name} {ex.Message}");
    }
}


// ==== Exercise 6B: Safe Fire-and-Forget (Optional Not Assessed) ====

async Task SendConfirmationAsync(Student student)
{
    try
    {
        await Task.Delay(100); // Simulate sending email
        Console.WriteLine($"Email sent to {student.Name}");
    }
    catch (Exception ex)
    {
        // Log the failure do NOT re-throw.
        // This is intentional fire-and-forget.
        Console.WriteLine($"Email failed for {student.Name}: {ex.Message}");
    }
}