using Microsoft.EntityFrameworkCore;
using BioLabManager.Models;

namespace BioLabManager.Services;

public class BioLabDbContext : DbContext
{
	public DbSet<Sample> Samples { get; set; }
	public DbSet<Lab> Labs { get; set; }
	public DbSet<User> Users { get; set; }

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		string dbPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "biolab.db");
		optionsBuilder.UseSqlite($"Data Source={dbPath}");
	}
}