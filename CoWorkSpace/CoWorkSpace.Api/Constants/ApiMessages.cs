namespace CoWorkSpace.Api.Constants
{
    public static class ApiMessages
    {
        // Errores de autorización
        public const string UNAUTHORIZED = "No autorizado.";
        public const string SERVER_ERROR = "Error en el servidor.";
        public const string INVALID_USER = "Usuario inválido.";
        public const string ONLY_PROVIDERS_CAN_CREATE_ADMINS = "Solo los proveedores pueden crear administradores.";
        public const string CANNOT_CREATE_ADMINS_FOR_OTHER_PROVIDERS = "No puedes crear administradores para otros proveedores.";
        public const string MAIL_AND_PASSWORD_ARE_REQUIRED = "Email y contraseña son requeridos.";
        public const string MAIL_REQUIRED = "El email es requerido.";
        public const string INVALID_CREDENTIALS = "Credenciales inválidas.";
        public const string INVALID_DATA = "Los datos proporcionados son inválidos";
        public const string NO_PERMISSION_CREATE_SPACE = "No tienes permisos para crear espacios.";
        public const string NO_PERMISSION_OTHER_PROVIDER = "No puedes crear espacios para otro proveedor.";
        public const string ONLY_PROVIDERS_CAN_VIEW_ADMINS = "Solo los proveedores pueden ver sus administradores.";
        public const string CANNOT_VIEW_ADMINS_FOR_OTHER_PROVIDERS = "No puedes ver administradores de otros proveedores.";
        public const string ONLY_PROVIDERS_CAN_EDIT_ADMINS = "Solo los proveedores pueden editar administradores";
        public const string NO_PERMISSION_UPDATE_SPACE = "No tienes permisos para editar espacios.";
        public const string CANNOT_EDIT_ADMINS_FOR_OTHER_PROVIDERS = "No puedes editar administradores de otro proveedor";
        public const string NO_PERMISSION_UPDATE_OTHER_PROVIDER = "No puedes editar espacios para otro proveedor.";
        public const string NO_PERMISSION_DELETE_SPACE = "No tienes permisos para borrar espacios.";
        public const string NO_PERMISSION_DELETE_OTHER_PROVIDER_SPACES = "No puedes borrar espacios de otro proveedor.";
        public const string CANNOT_DELETE_ADMINS_FOR_OTHER_PROVIDERS = "No puedes borrar administradores de otro proveedor.";
        public const string ONLY_ADMINS_CAN_ACCESS_SPACES = "Solo los administradores pueden acceder a los espacios.";
        public const string CANNOT_ACCESS_OTHER_ADMIN_SPACE = "No puedes ver espacios de otro administrador.";
        public const string CANNOT_ACCESS_OTHER_ADMINS_CLIENTS = "No puedes ver clientes de otros administradores";


        // Errores de validación
        public const string PASSWORD_TOO_SHORT = "La contraseña debe tener al menos 6 caracteres.";
        public const string EMAIL_ALREADY_REGISTERED = "El email ya está registrado.";
        public const string ONLY_ADMIN_ROLE_ALLOWED = "Solo puede crear usuarios con rol de administrador.";
        public const string ROLE_NOT_ALLOWED_ONLY_CAN_REGISTER_PROVIDER_OR_CLIENT = "Rol no permitido. Solo se pueden registrar usuarios con rol provider o client.";
        public const string INVALID_ROLE_OR_ROLEID_NOT_FOUND = "Rol no válido. RoleId no encontrado en la base de datos.";
        public const string SPACE_CREATED_ERROR = "Ocurrió un error al crear el espacio.";
        public const string SPACE_UPDATED_ERROR = "Ocurrió un error al actualizar el espacio.";
        public const string SPACE_DELETED_ERROR = "Ocurrió un error al borrar el espaccio.";

        // Not Found
        public const string NO_SPACES_FOR_PROVIDER = "No se encontraron espacios para el proveedor.";
        public const string SPACE_NOT_FOUND = "No se encontró el espacio.";
        public const string NO_ADMINS_FOUND = "No se encontraron administradores.";
        public const string ADMIN_NOT_FOUND = "No se encontró el administrador";
        public const string NO_SPACES_FOUND_FOR_ADMIN = "No se encontraron espacios para este administrador.";
        public const string NO_CLIENTS_FOUND_FOR_ADMIN = "No se encontraron clientes para este administrador.";

        // Éxito
        public const string LOGIN_SUCCESS = "Inicio de sesión exitoso.";
        public const string ADMIN_CREATED_SUCCESS = "Administrador creado correctamente.";
        public const string ADMIN_UPDATED_SUCCESS = "Administrador actualizado correctamente.";
        public const string USER_REGISTERED_SUCCESS = "Usuario registrado correctamente.";
        public const string SPACE_CREATED_SUCCESS = "El espacio se ha creado correctamente.";
        public const string SPACE_UPDATED_SUCCESS = "El espacio se ha actualizado correctamente,";
        public const string SPACE_DELETED_SUCCESS = "El espacio se ha borrado correctamente.";
        public const string ADMIN_DELETED_SUCCESS = "El administrador se ha borrado correctamente.";
    }
}
