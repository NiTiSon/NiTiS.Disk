using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace NiTiS.Disk.Api.Data;

public record FileRecord
{
	public long Id { get; set; }

	[Required]
	[ForeignKey(nameof(Owner))]
	public long OwnerId { get; set; }

	[JsonIgnore]
	public User Owner { get; init; }

	[Required]
	[Length(1, 255)]
	public string FileName { get; set; } = null!;

	public string InternalPath { get; set; } = null!;
}