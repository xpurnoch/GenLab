namespace BioLabManager.Models;
public class Sample
{
	public int Id { get; set; }
	public string SampleType { get; set; }
	public string Identifier { get; set; }
	public DateTime CollectedAt { get; set; }
	public string Status { get; set; }
	public string StorageLocation { get; set; }
	public string Sequence { get; set; }


	public int LabId { get; set; }
	public virtual Lab Lab { get; set; }


	public int UserId { get; set; }
	public virtual User User { get; set; }
}