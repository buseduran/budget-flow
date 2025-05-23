namespace BudgetFlow.Application.Common.Exceptions;
public class EmailSendException : Exception
{
    public EmailSendException() : base("E-posta gönderme sırasında bir hata oluştu.") { }

    public EmailSendException(string message) : base(message) { }

    public EmailSendException(string message, Exception innerException)
        : base(message, innerException) { }
}
