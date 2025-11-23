using System.Diagnostics.CodeAnalysis;

namespace Application.Common.Validations.Errors;

[ExcludeFromCodeCoverage]
public sealed class ValidationError
{
    public string Field { get; }
    public string Message { get; }

    public ValidationError(string field, string message)
    {
        Field = field ?? throw new ArgumentNullException(nameof(field));
        Message = message ?? throw new ArgumentNullException(nameof(message));
    }
}