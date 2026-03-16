using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Application.DTOs
{
    [DisplayName("Refresco de Sesión")]
    [Description("Respuesta que contiene el nuevo token de acceso generado.")]
    public record RefreshSessionResponse(
        [property: Description("Nuevo token JWT para continuar la sesión.")]
        string Jwt
    );
}
