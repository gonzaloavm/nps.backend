using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{

    [DisplayName("Petición de Voto")]
    [Description("Cuerpo de la solicitud necesario para registrar una nueva valoración NPS.")]
    public record CreateVoteRequest(
        [property: Range(0, 10)]
        [property: Description("Puntuación otorgada (0 = Muy improbable, 10 = Definitivamente lo recomendaría)")]
        int Score
    );
}
