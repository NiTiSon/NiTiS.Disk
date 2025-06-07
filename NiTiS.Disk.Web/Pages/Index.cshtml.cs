using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using NiTiS.Disk.Api.Data;
using NiTiS.Disk.Api.Managers;

namespace NiTiS.Disk.Web.Pages;

public class IndexModel : BaseModel
{
	private readonly ILogger<IndexModel> _logger;
	private readonly List<FileRecord> _files;
	
	public IEnumerable<FileRecord> Files => _files;
	
	public IndexModel(UserManager userManager, ILogger<IndexModel> logger) : base(userManager)
	{
		_logger = logger;
		_files = [];
	}

	public async Task<ActionResult> OnGet()
	{
		await CheckAuthorization();

		if (AuthorizedUser is null)
		{
			return Redirect("/Login");
		}
		
		Request.Query.TryGetValue("path", out StringValues fileName);

		_logger.LogInformation("Path {}", fileName.ToString());
		
		_files.Add(
			new FileRecord()
			{
				FileName = fileName,
				Owner = AuthorizedUser,
				Size = 12319,
			}
		);
		
		return Page();
	}
}