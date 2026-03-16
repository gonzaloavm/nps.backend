using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Application.DTOs
{
    [DisplayName("Estadísticas NPS")]
    [Description("Resumen detallado del cálculo del Net Promoter Score y la distribución de votos.")]
    public record NpsStatisticsResponse(
        [property: Description("Cantidad total de votos registrados.")]
        int TotalVotes,

        [property: Description("Usuarios que calificaron con 9 o 10.")]
        int Promoters,

        [property: Description("Usuarios que calificaron del 0 al 6.")]
        int Detractors,

        [property: Description("Usuarios que calificaron con 7 u 8.")]
        int Neutrals,

        [property: Description("Resultado del cálculo (Promotores % - Detractores %).")]
        double NpsScore,

        [property: Description("Categoría basada en el puntaje obtenido.")]
        string Classification
    );
    }
