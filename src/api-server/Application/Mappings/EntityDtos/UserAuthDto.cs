namespace Application.Mappings.EntityDtos;

public class UserAuthDto
{
    public UserDto User { get; set; }
    public string JwtToken { get; set; }
    public Guid? RefreshToken { get; set; }
}