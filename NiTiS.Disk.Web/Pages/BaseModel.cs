using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NiTiS.Disk.Api.Data;
using NiTiS.Disk.Api.Managers;

namespace NiTiS.Disk.Web.Pages;

public abstract class BaseModel : PageModel
{
	public User? AuthorizedUser { get; private set; }
	protected UserManager UserManager { get; }

	protected BaseModel(UserManager userManager)
	{
		UserManager = userManager;
	}

	public void OnPostLogout()
	{
		Logout();
	}
	
	public async Task CheckAuthorization()
	{
		Request.Cookies.TryGetValue(UserManager.SessionKey, out var cookie);

		if (cookie is null) return;

		Session? session = await UserManager.GetSessionAsync(cookie);

		AuthorizedUser = session?.User;
		ViewData["Authorized"] = session != null;
	}

	private void Logout()
	{
		Response.Cookies.Delete(UserManager.SessionKey);
	}
}