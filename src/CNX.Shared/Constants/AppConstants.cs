namespace CNX.Shared.Constants;

public static class AppConstants
{
    public const string SystemName = "CNX Counting";
    public const string Version = "1.0.0";

    public static class Roles
    {
        public const string SystemAdmin = "SystemAdmin";
        public const string TenantAdmin = "TenantAdmin";
        public const string Accountant = "Accountant";
        public const string Viewer = "Viewer";
    }

    public static class TaxCodes
    {
        public const string VATDeclaration = "01/GTGT";
        public const string CITDeclaration = "03/TNDN";
        public const string PITDeclaration = "05/QTT-TNCN";
    }
}
