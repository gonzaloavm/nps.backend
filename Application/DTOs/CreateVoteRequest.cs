using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public record CreateVoteRequest(
        int Score
    );
}
