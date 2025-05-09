using Microsoft.EntityFrameworkCore;

namespace NiTiS.Disk.Api.Data;

public sealed class DiskDbContext : DbContext
{
	public DiskDbContext(DbContextOptions<DiskDbContext> options) : base(options)
	{
		Database.EnsureCreated();
	}

	public DbSet<User> Users { get; set; }

	public DbSet<Login> Logins { get; set; }

	public DbSet<FileRecord> FileRecords { get; set; }

	public DbSet<Session> Sessions { get; set; }
}