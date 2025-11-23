using System.Diagnostics.CodeAnalysis;
using Application.Common.Validations.Errors;

namespace Application.Common.Exceptions;

[ExcludeFromCodeCoverage]
public class ValidationException : Exception
{
    public IEnumerable<ValidationError> Errors { get; }

    public ValidationException(IEnumerable<ValidationError> errors)
        : base("Aconteceram um ou mais erros.")
    {
        Errors = errors ?? throw new ArgumentNullException(nameof(errors));
    }
}