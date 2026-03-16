using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Application.DTOs
{
    [DisplayName("Respuesta de Autenticación")]
    [Description("Información devuelta tras un inicio de sesión exitoso, incluyendo el token de acceso.")]
    public record LoginResponse(
        [property: Description("Token JWT para autorizar las peticiones.")]
        string Token,

        [property: Description("Rol asignado al usuario.")]
        string Role,

        [property: Description("Nombre del usuario autenticado.")]
        string Username
    );
}
