using System.ComponentModel.DataAnnotations;
using Domain.Entities.Alerts;

namespace Domain.Entities;

public class State
{
	public State()
	{
	}

	public State(int id, string name)
	{
		Id = id;
		Name = name;
	}

	public int Id { get; private set; }

	[Required, MaxLength(80)]
	public string Name { get; private set; } = null!;

	[Required, MaxLength(3)]
	public string Uf { get; private set; } = null!;

	public virtual ICollection<AdoptionAlert> AdoptionAlerts { get; set; } = null!;
}