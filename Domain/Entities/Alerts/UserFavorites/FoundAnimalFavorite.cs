using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Alerts.UserFavorites;

public class FoundAnimalFavorite
{
	public required Guid Id { get; set; }

	[Required, ForeignKey("UserId")]
	public virtual required User User { get; set; }

	public required Guid UserId { get; set; }

	[Required, ForeignKey("FoundAnimalAlertId")]
	public virtual required FoundAnimalAlert FoundAnimalAlert { get; set; }

	public required Guid FoundAnimalAlertId { get; set; }
}