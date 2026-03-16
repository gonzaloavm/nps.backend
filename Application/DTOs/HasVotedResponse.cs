using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Application.DTOs
{
    [DisplayName("Estado de Votación")]
    [Description("Indica si el usuario autenticado ya ha participado en la encuesta actual.")]
    public record HasVotedResponse(
        [property: Description("Verdadero si el usuario ya registró su voto.")]
        bool HasVoted
    );
}
