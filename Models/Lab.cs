namespace BioLabManager.Models;
public class Lab
{
	public int Id { get; set; }
	public string Name { get; set; }

	public virtual ICollection<User> Users { get; set; } = new List<User>();
	public virtual ICollection<Sample> Samples { get; set; } = new List<Sample>();
}