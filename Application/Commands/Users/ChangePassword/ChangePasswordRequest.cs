using System.Diagnostics.CodeAnalysis;

namespace Application.Commands.Users.ChangePassword;

[ExcludeFromCodeCoverage]
public sealed record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword,
    string ConfirmNewPassword
);