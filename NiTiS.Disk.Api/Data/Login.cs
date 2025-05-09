using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NiTiS.Disk.Api.Data;

public record Login
{
	public long Id { get; set; }

	[Required]
	[ForeignKey(nameof(AssociatedUser))]
	public long UserId { get; init; }

	[Required]
	public User AssociatedUser { get; init; } = null!;

	[Required]
	public string Username { get; set; } = null!;

	[Required]
	public string PasswordHash { get; set; } = null!;

	[Required]
	public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

	public DateTime? LastUsedAt { get; set; }
}