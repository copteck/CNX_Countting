using CNX.Domain.Common;
using CNX.Domain.Enums;

namespace CNX.Domain.Entities.Accounting;

/// <summary>
/// Bút toán kế toán (Journal Entry)
/// </summary>
public class JournalEntry : BaseEntity
{
    public Guid TenantId { get; set; }
    public string EntryNumber { get; set; } = string.Empty;
    public DateTime EntryDate { get; set; }
    public string? Description { get; set; }
    public string? Reference { get; set; }
    public bool IsPosted { get; set; } = false;
    public DateTime? PostedDate { get; set; }

    // Navigation
    public ICollection<JournalEntryLine> Lines { get; set; } = new List<JournalEntryLine>();
}

/// <summary>
/// Chi tiết bút toán (dòng Nợ/Có)
/// </summary>
public class JournalEntryLine : BaseEntity
{
    public Guid JournalEntryId { get; set; }
    public Guid AccountId { get; set; }
    public TransactionType TransactionType { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public string? CostCenter { get; set; }

    // Navigation
    public JournalEntry JournalEntry { get; set; } = null!;
    public AccountChart Account { get; set; } = null!;
}
