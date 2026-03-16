using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Application.DTOs
{
    [DisplayName("Resultado del Voto")]
    [Description("Información devuelta tras registrar exitosamente una valoración.")]
    public record CreateVoteResponse(
        [property: Description("Identificador único del voto generado en la base de datos.")]
        int VoteId
    );
}
