namespace Application.Mappings.EntityDtos;

public class UserDto
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string? Email { get; set; } // TODO: move Email, IsAdmin to UserDetailsDto or something
    public bool IsAdmin { get; set; } = false;
}