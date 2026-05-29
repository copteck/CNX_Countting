namespace CNX.Shared.Extensions;

public static class StringExtensions
{
    public static string ToSlug(this string text)
    {
        return text.ToLower()
            .Replace(" ", "-")
            .Replace("đ", "d")
            .Replace("ă", "a")
            .Replace("â", "a")
            .Replace("ê", "e")
            .Replace("ô", "o")
            .Replace("ơ", "o")
            .Replace("ư", "u");
    }

    public static bool IsValidTaxCode(this string taxCode)
    {
        if (string.IsNullOrWhiteSpace(taxCode))
            return false;

        // Mã số thuế Việt Nam: 10 hoặc 13 ký tự số (có thể có dấu -)
        var cleaned = taxCode.Replace("-", "");
        return cleaned.Length == 10 || cleaned.Length == 13;
    }
}
