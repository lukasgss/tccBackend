using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Application.Commands.UserMessages.SendMessage;

[ExcludeFromCodeCoverage]
public sealed class SendUserMessageRequest
{
    [Required(ErrorMessage = "Campo de conteúdo é obrigatório.")]
    public string Content { get; set; } = null!;

    [Required(ErrorMessage = "Campo de recebedor é obrigatório.")]
    public Guid ReceiverId { get; set; }
}