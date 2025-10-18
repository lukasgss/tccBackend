using Domain.Enums;

namespace Domain.Entities;

public class AdoptionReport
{
    public Guid Id { get; set; }
    public string Reason { get; set; } = null!;
    public ReportStatus Status { get; set; }
    public string? RejectedReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public User? Owner { get; set; }
}