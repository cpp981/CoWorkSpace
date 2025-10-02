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
        public const string NoPermissionCreateSpace = "No tienes permisos para crear espacios.";
        public const string NoPermissionOtherProvider = "No puedes crear espacios para otro proveedor.";
        public const string OnlyProvidersCanViewAdmins = "Solo los proveedores pueden ver sus administradores.";
        public const string CannotViewAdminsForOtherProviders = "No puedes ver administradores de otros proveedores.";
        public const string OnlyProvidersCanEditAdmins = "Solo los proveedores pueden editar administradores";
        public const string NoPermissionUpdateSpace = "No tienes permisos para editar espacios.";
        public const string CannotEditAdminsForOtherProviders = "No puedes editar administradores de otro proveedor";
        public const string NoPermissionUpdateOtherProvider = "No puedes editar espacios para otro proveedor.";
        public const string NoPermissionDeleteSpace = "No tienes permisos para borrar espacios.";
        public const string NoPermissionDeleteOtherProviderSpaces = "No puedes borrar espacios de otro proveedor.";
        public const string CannotDeleteAdminsForOtherProviders = "No puedes borrar administradores de otro proveedor.";


        // Errores de validación
        public const string PasswordTooShort = "La contraseña debe tener al menos 6 caracteres.";
        public const string EmailAlreadyRegistered = "El email ya está registrado.";
        public const string OnlyAdminRoleAllowed = "Solo puede crear usuarios con rol de administrador.";
        public const string RoleNotAllowedOnlyCanRegisterProviderOrClient = "Rol no permitido. Solo se pueden registrar usuarios con rol provider o client.";
        public const string InvalidRoleOrRoleIdNotFound = "Rol no válido. RoleId no encontrado en la base de datos.";
        public const string SpaceCreatedError = "Ocurrió un error al crear el espacio.";
        public const string SpaceUpdatedError = "Ocurrió un error al actualizar el espacio.";
        public const string SpaceDeletedError = "Ocurrió un error al borrar el espaccio.";

        // Not Found
        public const string NoSpacesForProvider = "No se encontraron espacios para el proveedor.";
        public const string SpaceNotFound = "No se encontró el espacio.";
        public const string NoAdminsFound = "No se encontraron administradores.";
        public const string AdminNotFound = "No se encontró el administrador";

        // Éxito
        public const string LoginSuccessfully = "Inicio de sesión exitoso.";
        public const string AdminCreatedSuccessfully = "Administrador creado correctamente.";
        public const string AdminUpdatedSuccessfully = "Administrador actualizado correctamente.";
        public const string UserRegisteredSuccessfully = "Usuario registrado correctamente.";
        public const string SpaceCreatedSuccess = "El espacio se ha creado correctamente.";
        public const string SpaceUpdatedSuccess = "El espacio se ha actualizado correctamente,";
        public const string SpaceDeletedSuccess = "El espacio se ha borrado correctamente.";
        public const string AdminDeletedSuccessfully = "El administrador se ha borrado correctamente.";
    }
}
