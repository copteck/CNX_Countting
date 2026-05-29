using CNX.Domain.Common;
using CNX.Domain.Enums;

namespace CNX.Domain.Entities.Accounting;

/// <summary>
/// Hệ thống tài khoản kế toán (Chart of Accounts)
/// </summary>
public class AccountChart : BaseEntity
{
    public Guid TenantId { get; set; }
    public string AccountCode { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public AccountType AccountType { get; set; }
    public Guid? ParentAccountId { get; set; }
    public int Level { get; set; } = 1;
    public bool IsActive { get; set; } = true;
    public string? Description { get; set; }

    // Navigation
    public AccountChart? ParentAccount { get; set; }
    public ICollection<AccountChart> ChildAccounts { get; set; } = new List<AccountChart>();
    public ICollection<JournalEntry> JournalEntries { get; set; } = new List<JournalEntry>();
}
