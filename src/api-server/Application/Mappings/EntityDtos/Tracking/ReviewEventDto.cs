using Domain.Entities.Tracking;

namespace Application.Mappings.EntityDtos.Tracking;

public class ReviewEventDto
{
    public string ent_seq { get; set; }
    public Guid UserId { get; set; } // maybe I can also remove this one
    public DateTimeOffset Created { get; set; }
    public ReviewValue Value { get; set; }
}