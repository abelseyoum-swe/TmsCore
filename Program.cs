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