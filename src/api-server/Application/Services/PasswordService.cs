using BC = BCrypt.Net.BCrypt;

namespace Application.Services;

public interface IPasswordService
{
    string HashPassword(string password);
    bool VerifyHashedPassword(string hashedPassword, string providedPassword);
}

public class PasswordService : IPasswordService
{
    public string HashPassword(string password)
    {
        if (password == null)
        {
            throw new ArgumentNullException(nameof(password));
        }

        return BC.HashPassword(password);
    }

    public bool VerifyHashedPassword(string hashedPassword, string providedPassword)
    {
        if (hashedPassword == null) throw new ArgumentNullException(nameof(hashedPassword));
        if (providedPassword == null) throw new ArgumentNullException(nameof(providedPassword));

        if (BC.Verify(providedPassword, hashedPassword))
        {
            return true;
        }

        return false;
    }
}