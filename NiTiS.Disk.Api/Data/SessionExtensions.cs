using Microsoft.AspNetCore.Http;
using NiTiS.Disk.Api.Managers;

namespace NiTiS.Disk.Api.Data;

public static class SessionExtensions
{
	public static void Apply(this HttpResponse response, Session session)
	{
		response.Cookies.Append(UserManager.SessionKey, session.Token, new()
		{
			HttpOnly = true,
			Path = "/",
			Secure = true,
			IsEssential = true
		});
	}
}