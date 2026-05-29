namespace CNX.Application.DTOs.Accounting;

public class AccountChartDto
{
    public Guid Id { get; set; }
    public string AccountCode { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty;
    public Guid? ParentAccountId { get; set; }
    public int Level { get; set; }
    public bool IsActive { get; set; }
    public List<AccountChartDto> ChildAccounts { get; set; } = new();
}

public class CreateAccountChartDto
{
    public string AccountCode { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public int AccountType { get; set; }
    public Guid? ParentAccountId { get; set; }
    public string? Description { get; set; }
}

public class JournalEntryDto
{
    public Guid Id { get; set; }
    public string EntryNumber { get; set; } = string.Empty;
    public DateTime EntryDate { get; set; }
    public string? Description { get; set; }
    public bool IsPosted { get; set; }
    public List<JournalEntryLineDto> Lines { get; set; } = new();
}

public class JournalEntryLineDto
{
    public Guid AccountId { get; set; }
    public string AccountCode { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public string TransactionType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? Description { get; set; }
}

public class CreateJournalEntryDto
{
    public DateTime EntryDate { get; set; }
    public string? Description { get; set; }
    public string? Reference { get; set; }
    public List<CreateJournalEntryLineDto> Lines { get; set; } = new();
}

public class CreateJournalEntryLineDto
{
    public Guid AccountId { get; set; }
    public int TransactionType { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
}

public class InvoiceDto
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public string? InvoiceSerial { get; set; }
    public DateTime InvoiceDate { get; set; }
    public bool IsInputInvoice { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerTaxCode { get; set; }
    public decimal SubTotal { get; set; }
    public decimal VatAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public List<InvoiceLineDto> Lines { get; set; } = new();
}

public class InvoiceLineDto
{
    public string ItemName { get; set; } = string.Empty;
    public string? Unit { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Amount { get; set; }
    public decimal VatRate { get; set; }
    public decimal VatAmount { get; set; }
}

public class CreateInvoiceDto
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public string? InvoiceSerial { get; set; }
    public DateTime InvoiceDate { get; set; }
    public bool IsInputInvoice { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerTaxCode { get; set; }
    public string? CustomerAddress { get; set; }
    public List<CreateInvoiceLineDto> Lines { get; set; } = new();
}

public class CreateInvoiceLineDto
{
    public string? ItemCode { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string? Unit { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal VatRate { get; set; }
    public Guid? AccountId { get; set; }
}
