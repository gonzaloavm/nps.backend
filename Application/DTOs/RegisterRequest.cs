using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Application.DTOs
{
    [DisplayName("Registro de Usuario")]
    [Description("Datos necesarios para crear una nueva cuenta en el sistema.")]
    public record RegisterRequest(
        [property: Description("Nombre de usuario único.")]
        [property: DefaultValue("nuevo_usuario")]
        string Username,

        [property: Description("Contraseña de acceso (mínimo 8 caracteres).")]
        [property: DefaultValue("P@ssw0rd2026!")]
        string Password,

        [property: Description("Rol que desempeñará el usuario (Voter/Admin).")]
        [property: DefaultValue("Voter")]
        string Role
    );
}
