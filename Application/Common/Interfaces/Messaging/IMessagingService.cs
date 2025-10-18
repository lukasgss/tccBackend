namespace Application.Common.Interfaces.Messaging;

public interface IMessagingService
{
    Task SendAccountConfirmationMessageAsync(string userEmail, string confirmationLink);
    Task SendForgotPasswordMessageAsync(string userEmail, string userName, string forgotPasswordToken);
}