namespace Ced.Utility.Email
{
    public interface IEmailHelper
    {
        void SendEmail(string subject, string body, string to);
    }
}