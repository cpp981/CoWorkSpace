namespace CoWorkSpace.Api.Constants
{
    public static class ApiMessages
    {
        // Errores de autorización
        public const string Unauthorized = "No autorizado.";
        public const string ServerError = "Error en el servidor.";
        public const string InvalidUser = "Usuario inválido.";
        public const string OnlyProvidersCanCreateAdmins = "Solo los proveedores pueden crear administradores.";
        public const string CannotCreateAdminsForOtherProviders = "No puedes crear administradores para otros proveedores.";
        public const string MailAndPasswordAreRequired = "Email y contraseña son requeridos.";
        public const string MailRequired = "El email es requerido.";
        public const string InvalidCredentials = "Credenciales inválidas.";
        public const string InvalidData = "Los datos proporcionados son inválidos";


        // Errores de validación
        public const string EmailAlreadyRegistered = "El email ya está registrado.";
        public const string OnlyAdminRoleAllowed = "Solo puede crear usuarios con rol de administrador.";
        public const string RoleNotAllowedOnlyCanRegisterProviderOrClient = "Rol no permitido. Solo se pueden registrar usuarios con rol provider o client.";
        public const string InvalidRoleOrRoleIdNotFound = "Rol no válido. RoleId no encontrado en la base de datos.";


        // Éxito
        public const string LoginSuccessfully = "Inicio de sesión exitoso.";
        public const string AdminCreatedSuccessfully = "Administrador creado correctamente.";
        public const string UserRegisteredSuccessfully = "Usuario registrado correctamente.";
    }
}
