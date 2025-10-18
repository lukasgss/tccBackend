using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Entities.Alerts;

namespace Domain.Entities;

public class City
{
	public City()
	{
	}

	public City(int id, string name)
	{
		Id = id;
		Name = name;
	}

	public int Id { get; private set; }

	[Required, MaxLength(80)]
	public string Name { get; private set; } = null!;

	[Required, ForeignKey("StateId")]
	public State State { get; private set; } = null!;

	public int StateId { get; private set; }

	public virtual ICollection<AdoptionAlert> AdoptionAlerts { get; set; } = null!;
}