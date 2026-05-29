using CNX.Domain.Common;

namespace CNX.Domain.Entities.Accounting;

/// <summary>
/// Hóa đơn (Invoice) - đầu vào / đầu ra
/// </summary>
public class Invoice : BaseEntity
{
    public Guid TenantId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public string? InvoiceSerial { get; set; }
    public DateTime InvoiceDate { get; set; }
    public bool IsInputInvoice { get; set; } // true = đầu vào, false = đầu ra
    public string? CustomerName { get; set; }
    public string? CustomerTaxCode { get; set; }
    public string? CustomerAddress { get; set; }
    public decimal SubTotal { get; set; }
    public decimal VatRate { get; set; }
    public decimal VatAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Currency { get; set; } = "VND";
    public decimal ExchangeRate { get; set; } = 1;
    public string? Notes { get; set; }

    // Navigation
    public ICollection<InvoiceLine> Lines { get; set; } = new List<InvoiceLine>();
}

/// <summary>
/// Chi tiết hóa đơn
/// </summary>
public class InvoiceLine : BaseEntity
{
    public Guid InvoiceId { get; set; }
    public string? ItemCode { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string? Unit { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Amount { get; set; }
    public decimal VatRate { get; set; }
    public decimal VatAmount { get; set; }
    public Guid? AccountId { get; set; }

    // Navigation
    public Invoice Invoice { get; set; } = null!;
    public AccountChart? Account { get; set; }
}
