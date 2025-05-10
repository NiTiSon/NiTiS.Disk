using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NiTiS.Disk.Api.Data;

public record User
{
	public long Id { get; set; }
	
	[Required]
	[Length(4, 64)]
	public string Username { get; set; }

	[Required]
	public string DisplayName { get; set; } = null!;

	public UserRights Rights { get; set; } = UserRights.None;

	[Required]
	public string PasswordHash { get; set; } = null!;

	[Required]
	public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

	public DateTime? LastUsedAt { get; set; }
}