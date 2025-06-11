namespace RegularizadorPolizas.Domain.Enums
{
    public static class SystemPermissions
    {
        // Clients
        public const string CLIENTS_READ = "clients.read";
        public const string CLIENTS_CREATE = "clients.create";
        public const string CLIENTS_UPDATE = "clients.update";
        public const string CLIENTS_DELETE = "clients.delete";
        public const string CLIENTS_SEARCH = "clients.search";

        // Polizas
        public const string POLIZAS_READ = "polizas.read";
        public const string POLIZAS_CREATE = "polizas.create";
        public const string POLIZAS_UPDATE = "polizas.update";
        public const string POLIZAS_DELETE = "polizas.delete";
        public const string POLIZAS_RENEW = "polizas.renew";
        public const string POLIZAS_SEARCH = "polizas.search";

        // Documents
        public const string DOCUMENTS_READ = "documents.read";
        public const string DOCUMENTS_UPLOAD = "documents.upload";
        public const string DOCUMENTS_PROCESS = "documents.process";
        public const string DOCUMENTS_DELETE = "documents.delete";

        // Brokers
        public const string BROKERS_READ = "brokers.read";
        public const string BROKERS_CREATE = "brokers.create";
        public const string BROKERS_UPDATE = "brokers.update";
        public const string BROKERS_DELETE = "brokers.delete";

        // Companies
        public const string COMPANIES_READ = "companies.read";
        public const string COMPANIES_CREATE = "companies.create";
        public const string COMPANIES_UPDATE = "companies.update";
        public const string COMPANIES_DELETE = "companies.delete";

        // Currencies
        public const string CURRENCIES_READ = "currencies.read";
        public const string CURRENCIES_CREATE = "currencies.create";
        public const string CURRENCIES_UPDATE = "currencies.update";
        public const string CURRENCIES_DELETE = "currencies.delete";

        // Renovations
        public const string RENOVATIONS_READ = "renovations.read";
        public const string RENOVATIONS_CREATE = "renovations.create";
        public const string RENOVATIONS_PROCESS = "renovations.process";
        public const string RENOVATIONS_CANCEL = "renovations.cancel";

        // System Administration
        public const string ADMIN_USERS_MANAGE = "admin.users.manage";
        public const string ADMIN_ROLES_MANAGE = "admin.roles.manage";
        public const string ADMIN_PERMISSIONS_MANAGE = "admin.permissions.manage";
        public const string ADMIN_AUDIT_READ = "admin.audit.read";
        public const string ADMIN_SYSTEM_CONFIG = "admin.system.config";

        // Reports
        public const string REPORTS_READ = "reports.read";
        public const string REPORTS_EXPORT = "reports.export";
    }

    public static class SystemRoles
    {
        public const string SUPER_ADMIN = "SuperAdmin";
        public const string ADMIN = "Admin";
        public const string MANAGER = "Manager";
        public const string OPERATOR = "Operator";
        public const string VIEWER = "Viewer";
        public const string CLIENT = "Client";
    }
}