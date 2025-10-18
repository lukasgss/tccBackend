using Microsoft.AspNetCore.Http;

namespace Application.Commands.Users.Edit;

public class EditUserRequest
{
    public required string FullName { get; set; } = null!;
    public required string PhoneNumber { get; set; } = null!;
    public bool OnlyWhatsAppMessages { get; set; }
    public IFormFile? Image { get; set; }
    public string ExistingImage { get; set; } = null!;
    public IFormFile? DefaultAdoptionForm { get; set; }
}