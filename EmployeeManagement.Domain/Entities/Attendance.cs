using EmployeeManagement.Domain.Entities;

public class Attendance
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }

    public DateTime ClockInTime { get; set; }
    public DateTime? ClockOutTime { get; set; }

    public double? TotalHours { get; set; }
    public double? OvertimeHours { get; set; }

    public string? Status { get; set; } 

    public Employee? Employee { get; set; }
}
