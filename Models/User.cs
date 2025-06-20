namespace BioLabManager.Models;

public class User
{
	public int Id { get; set; }
	public string Username { get; set; }
	public string PasswordHash { get; set; }
	public string Role { get; set; }

	public int LabId { get; set; }
	public virtual Lab Lab { get; set; }

	public virtual ICollection<Sample> Samples { get; set; }

}