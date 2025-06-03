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

        [Description("Cliente creado")]
        ClientCreated = 100,

        [Description("Cliente modificado")]
        ClientUpdated = 101,

        [Description("Cliente eliminado")]
        ClientDeleted = 102,

        [Description("Corredor creado")]
        BrokerCreated = 200,

        [Description("Corredor modificado")]
        BrokerUpdated = 201,

        [Description("Corredor eliminado")]
        BrokerDeleted = 202,

        [Description("Compañía creada")]
        CompanyCreated = 300,

        [Description("Compañía modificada")]
        CompanyUpdated = 301,

        [Description("Compañía eliminada")]
        CompanyDeleted = 302,

        [Description("Moneda creada")]
        CurrencyCreated = 400,

        [Description("Moneda modificada")]
        CurrencyUpdated = 401,

        [Description("Moneda eliminada")]
        CurrencyDeleted = 402,

        [Description("Póliza creada")]
        PolicyCreated = 500,

        [Description("Póliza modificada")]
        PolicyUpdated = 501,

        [Description("Póliza eliminada")]
        PolicyDeleted = 502,

        [Description("Póliza renovada")]
        PolicyRenewed = 503,

        [Description("Poliza endosada")]
        PolicyEndosed = 504,

        [Description("Renovación creada")]
        RenovationCreated = 600,

        [Description("Renovación procesada")]
        RenovationProcessed = 601,

        [Description("Renovación cancelada")]
        RenovationCancelled = 602,

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

        [Description("Error de sistema")]
        SystemError = 9000,

        [Description("Advertencia del sistema")]
        SystemWarning = 9001,

        [Description("Información del sistema")]
        SystemInfo = 9002
    }
}
