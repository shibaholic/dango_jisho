namespace Application;

[Serializable]
public class ProblemException : Exception
{
    public string Title  { get; set; }
    public string Message { get; set; }

    public ProblemException(string title, string message) : base(message)
    {
        Title = title;
        Message = message;
    }
}