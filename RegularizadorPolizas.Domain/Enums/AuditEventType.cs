using System.ComponentModel;

namespace RegularizadorPolizas.Domain.Enums
{
    public enum AuditEventType
    {
        [Description("Inicio de sesión")]
        Login = 1,

        [Description("Cierre de sesión")]
        Logout = 2,

        [Description("Token validado")]
        TokenValidation = 3,

        [Description("Error de autenticación")]
        AuthenticationError = 4,

        [Description("Intento de acceso denegado")]
        AccessDenied = 5,

        [Description("Sesión expirada")]
        SessionExpired = 6,

        [Description("Contraseña cambiada")]
        PasswordChanged = 7,

        [Description("Intento de login fallido")]
        LoginFailed = 8,


        [Description("Cliente creado")]
        ClientCreated = 100,

        [Description("Cliente modificado")]
        ClientUpdated = 101,

        [Description("Cliente eliminado")]
        ClientDeleted = 102,

        [Description("Cliente activado")]
        ClientActivated = 103,

        [Description("Cliente desactivado")]
        ClientDeactivated = 104,

        [Description("Cliente búsqueda realizada")]
        ClientSearched = 105,

        [Description("Cliente exportado")]
        ClientExported = 106,

        [Description("Cliente importado")]
        ClientImported = 107,


        [Description("Corredor creado")]
        BrokerCreated = 200,

        [Description("Corredor modificado")]
        BrokerUpdated = 201,

        [Description("Corredor eliminado")]
        BrokerDeleted = 202,

        [Description("Corredor activado")]
        BrokerActivated = 203,

        [Description("Corredor desactivado")]
        BrokerDeactivated = 204,


        [Description("Compañía creada")]
        CompanyCreated = 300,

        [Description("Compañía modificada")]
        CompanyUpdated = 301,

        [Description("Compañía eliminada")]
        CompanyDeleted = 302,

        [Description("Compañía activada")]
        CompanyActivated = 303,

        [Description("Compañía desactivada")]
        CompanyDeactivated = 304,


        [Description("Moneda creada")]
        CurrencyCreated = 400,

        [Description("Moneda modificada")]
        CurrencyUpdated = 401,

        [Description("Moneda eliminada")]
        CurrencyDeleted = 402,

        [Description("Moneda activada")]
        CurrencyActivated = 403,

        [Description("Moneda desactivada")]
        CurrencyDeactivated = 404,


        [Description("Póliza creada")]
        PolicyCreated = 500,

        [Description("Póliza modificada")]
        PolicyUpdated = 501,

        [Description("Póliza eliminada")]
        PolicyDeleted = 502,

        [Description("Póliza renovada")]
        PolicyRenewed = 503,

        [Description("Póliza endosada")]
        PolicyEndorsed = 504,

        [Description("Póliza cancelada")]
        PolicyCancelled = 505,

        [Description("Póliza activada")]
        PolicyActivated = 506,

        [Description("Póliza vencida")]
        PolicyExpired = 507,

        [Description("Póliza búsqueda realizada")]
        PolicySearched = 508,

        [Description("Póliza exportada")]
        PolicyExported = 509,

        [Description("Póliza importada")]
        PolicyImported = 510,


        [Description("Renovación creada")]
        RenovationCreated = 600,

        [Description("Renovación procesada")]
        RenovationProcessed = 601,

        [Description("Renovación cancelada")]
        RenovationCancelled = 602,

        [Description("Renovación modificada")]
        RenovationUpdated = 603,

        [Description("Renovación aprobada")]
        RenovationApproved = 604,

        [Description("Renovación rechazada")]
        RenovationRejected = 605,


        [Description("Usuario creado")]
        UserCreated = 700,

        [Description("Usuario modificado")]
        UserUpdated = 701,

        [Description("Usuario eliminado")]
        UserDeleted = 702,

        [Description("Usuario activado")]
        UserActivated = 703,

        [Description("Usuario desactivado")]
        UserDeactivated = 704,

        [Description("Rol asignado a usuario")]
        UserRoleAssigned = 705,

        [Description("Rol removido de usuario")]
        UserRoleRemoved = 706,

        [Description("Perfil de usuario actualizado")]
        UserProfileUpdated = 707,

        [Description("Role de usuario modificado")]
        UserRolesChanged = 708,

        [Description("Role de usuario quitado.")]
        UserRolesRemoved = 709,

        [Description("Rol creado")]
        RoleCreated = 800,

        [Description("Rol modificado")]
        RoleUpdated = 801,

        [Description("Rol eliminado")]
        RoleDeleted = 802,

        [Description("Permiso asignado a rol")]
        RolePermissionGranted = 803,

        [Description("Permiso removido de rol")]
        RolePermissionRevoked = 804,

        [Description("Permiso creado")]
        PermissionCreated = 805,

        [Description("Permiso modificado")]
        PermissionUpdated = 806,

        [Description("Documento cargado")]
        DocumentUploaded = 1000,

        [Description("Documento procesado")]
        DocumentProcessed = 1001,

        [Description("Error en procesamiento de documento")]
        DocumentProcessingError = 1002,

        [Description("Documento vinculado a póliza")]
        DocumentLinkedToPolicy = 1003,

        [Description("Extracción de datos de documento")]
        DocumentDataExtracted = 1004,

        [Description("Documento validado")]
        DocumentValidated = 1005,

        [Description("Documento rechazado")]
        DocumentRejected = 1006,

        [Description("Documento eliminado")]
        DocumentDeleted = 1007,

        [Description("Documento descargado")]
        DocumentDownloaded = 1008,


        [Description("Reporte generado")]
        ReportGenerated = 1100,

        [Description("Reporte exportado")]
        ReportExported = 1101,

        [Description("Reporte enviado por email")]
        ReportEmailed = 1102,

        [Description("Reporte programado")]
        ReportScheduled = 1103,


        [Description("API externa consultada")]
        ExternalApiCalled = 2000,

        [Description("Error en API externa")]
        ExternalApiError = 2001,

        [Description("Sincronización con Velneo")]
        VelneoSyncStarted = 2002,

        [Description("Error de sincronización con Velneo")]
        VelneoSyncError = 2003,

        [Description("Sincronización completada")]
        VelneoSyncCompleted = 2004,

        [Description("Datos importados desde Velneo")]
        VelneoDataImported = 2005,

        [Description("Datos exportados a Velneo")]
        VelneoDataExported = 2006,


        [Description("Backup iniciado")]
        BackupStarted = 3000,

        [Description("Backup completado")]
        BackupCompleted = 3001,

        [Description("Error en backup")]
        BackupError = 3002,

        [Description("Restauración iniciada")]
        RestoreStarted = 3003,

        [Description("Restauración completada")]
        RestoreCompleted = 3004,

        [Description("Error en restauración")]
        RestoreError = 3005,

        [Description("Mantenimiento programado")]
        MaintenanceScheduled = 3006,

        [Description("Mantenimiento iniciado")]
        MaintenanceStarted = 3007,

        [Description("Mantenimiento completado")]
        MaintenanceCompleted = 3008,


        [Description("Regla de negocio violada")]
        BusinessRuleViolation = 4000,

        [Description("Validación fallida")]
        ValidationFailed = 4001,

        [Description("Dato inconsistente detectado")]
        DataInconsistencyDetected = 4002,

        [Description("Proceso automático ejecutado")]
        AutomatedProcessExecuted = 4003,

        [Description("Alerta de vencimiento")]
        ExpirationAlert = 4004,

        [Description("Recordatorio enviado")]
        ReminderSent = 4005,


        [Description("Error de sistema")]
        SystemError = 9000,

        [Description("Advertencia del sistema")]
        SystemWarning = 9001,

        [Description("Información del sistema")]
        SystemInfo = 9002,

        [Description("Sistema iniciado")]
        SystemStarted = 9003,

        [Description("Sistema detenido")]
        SystemStopped = 9004,

        [Description("Configuración cambiada")]
        ConfigurationChanged = 9005,

        [Description("Límite de recursos alcanzado")]
        ResourceLimitReached = 9006,

        [Description("Performance degradada")]
        PerformanceDegraded = 9007,

        [Description("Conexión de base de datos perdida")]
        DatabaseConnectionLost = 9008,

        [Description("Conexión de base de datos restaurada")]
        DatabaseConnectionRestored = 9009,

        [Description("Limpieza automática ejecutada")]
        AutoCleanupExecuted = 9010
    }
}