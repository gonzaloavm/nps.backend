using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Application.DTOs
{
    [DisplayName("Resultado de Registro")]
    [Description("Información devuelta tras la creación exitosa de un nuevo usuario.")]
    public record RegisterResponse(
        [property: Description("Identificador único del usuario creado en la base de datos.")]
        int UserId
    );
}
