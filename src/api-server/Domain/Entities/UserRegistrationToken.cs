namespace Domain.Entities;

public class UserRegistrationToken : IBaseEntity
{
    public string Token { get; init; }
    public string RecipientName { get; init; }
    public string RecipientEmail { get; init; }
    public DateTimeOffset? DateRegistered { get; set; }
    public Guid? UserId { get; set; }
    public User? User { get; set; }

    public UserRegistrationToken(string recipientName, string recipientEmail)
    {
        RecipientName = recipientName;
        RecipientEmail = recipientEmail;
        
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var stringChars = new char[5];
        var random = new Random();

        for (int i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[random.Next(chars.Length)];
        }

        var finalString = new String(stringChars);

        Token = finalString;
    }
}