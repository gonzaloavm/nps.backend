using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;


namespace Application.DTOs
{
    [DisplayName("Credenciales de Acceso")]
    [Description("Credenciales necesarias para el inicio de sesión.")]
    public record LoginRequest(
        [property: Description("Nombre de usuario del votante o administrador.")]
        [property: DefaultValue("voter_demo")]
        string Username,

        [property: Description("Contraseña de seguridad.")]
        [property: DefaultValue("P@ssw0rd123")]
        string Password
    );
}
