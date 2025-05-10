using System.Threading.Tasks;
using NiTiS.Disk.Api.Data;
using NiTiS.Disk.Api.Managers;

namespace NiTiS.Disk.Web.Pages;

public class IndexModel : BaseModel
{
	public IndexModel(UserManager userManager) : base(userManager)
	{
	}

	public async Task OnGet()
	{
		await CheckAuthorization();
	}
}