using System;
using System.ComponentModel.DataAnnotations;

namespace NiTiS.Disk.Api.Data;

public class Session
{
	public long Id { get; set; }

	[MaxLength(128)]
	[MinLength(128)]
	public string Token { get; set; } = null!;

	public User User { get; set; } = null!;

	public DateTime ExpirationDate { get; set; } = DateTime.UtcNow + TimeSpan.FromDays(7);
}