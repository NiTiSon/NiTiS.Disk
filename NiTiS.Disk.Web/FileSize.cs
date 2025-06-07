namespace NiTiS.Disk.Web;

public static class FileSize
{
	public static string ToDisplay(ulong bytes)
	{
		return bytes switch
		{
			< 1_000 => bytes + " B",
			< 1_000_000 => bytes / 1_000.0 + " KB",
			< 1_000_000_000 => bytes / 1_000_000.0 + " MB",
			_ => bytes / 1_000_000_000.0 + " GB"
		};
	}
}