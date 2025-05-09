using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace NiTiS.Disk.Api.Data;

public record User
{
	public long Id { get; set; }

	[Required]
	[Length(4, 64)]
	public string DisplayName { get; set; } = null!;

	public UserRights Rights { get; set; } = UserRights.None;

	[JsonIgnore]
	public virtual ICollection<Login> Logins { get; init; } = [];
}