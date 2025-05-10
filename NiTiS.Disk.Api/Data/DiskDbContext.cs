using Microsoft.EntityFrameworkCore;

namespace NiTiS.Disk.Api.Data;

public sealed class DiskDbContext : DbContext
{
	public DiskDbContext(DbContextOptions<DiskDbContext> options) : base(options)
	{
		Database.EnsureCreated();
	}

	protected override void OnModelCreating(ModelBuilder builder)
	{
		builder.Entity<User>()
			.HasIndex(u => u.Username)
			.IsUnique();
	}

	public DbSet<User> Users { get; set; } = null!;
	
	public DbSet<FileRecord> FileRecords { get; set; } = null!;

	public DbSet<Session> Sessions { get; set; } = null!;
}