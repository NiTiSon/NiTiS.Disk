using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using NiTiS.Disk.Api.Data;
using NiTiS.Disk.Api.Managers;

namespace NiTiS.Disk.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class UserController : ControllerBase
{
	private readonly UserManager _userManager;

	public UserController(UserManager userManager)
	{
		_userManager = userManager;
	}

	[HttpPost]
	public async Task<ActionResult> RegisterUser(
		[FromHeader(Name = "X-Username")] string xUsername,
		[FromHeader(Name = "X-Password")] string xPassword)
	{
		if (xPassword.Length is < 6 or > 32)
		{
			return BadRequest("Password length must be between 6 and 32 characters.");
		}

		if (xUsername.Length is < 4 or > 64)
		{
			return BadRequest("Login length must be between 4 and 64 characters.");
		}

		if (!UserManager.EnsurePasswordIsValid(xPassword))
		{
			return BadRequest("Password contains invalid characters.");
		}
		
		string hash = UserManager.HashPassword(xPassword);

		if (await _userManager.CheckIfExistsAsync(xUsername))
		{
			return BadRequest("Login already exists.");
		}

		User user = new()
		{
			DisplayName = xUsername,
			Username = xUsername,
			PasswordHash = hash,
		};
		
		User? dbUser = await _userManager.TryRegisterUserAsync(user);
		await _userManager.SaveChangesAsync();
		if (dbUser is null)
		{
			return BadRequest("Failed to register user.");
		}

		return Ok();
	}

	[HttpPost]
	public ActionResult Logout() // Not sure that JS allowed to work with cookies, so there it is
	{
		if (Request.Cookies[UserManager.SessionKey] != null)
		{
			Response.Cookies.Delete(UserManager.SessionKey);
		}

		return Ok();
	}

	[HttpPost]
	public async Task<ActionResult> Login()
	{
		StringValues loginValues = Request.HttpContext.Request.Headers["X-Username"];
		StringValues passwordValues = Request.HttpContext.Request.Headers["X-Password"];
		
		string? login = loginValues.FirstOrDefault();
		string? password = passwordValues.FirstOrDefault();

		if (login == null || password == null)
		{
			return BadRequest("X-Username and X-Password headers are required");
		}

		User? user = await _userManager.GetUserByLoginAsync(login);

		if (user == null)
		{
			return Unauthorized("Invalid username or password");
		}

		if (!_userManager.VerifyPassword(user, password))
		{
			return Unauthorized("Invalid password");
		}

		Session newSession = _userManager.AllocateNewSession(user);

		Response.Cookies.Append(UserManager.SessionKey, newSession.Token, new CookieOptions()
		{
			HttpOnly = true,
			Secure = true,
			SameSite = SameSiteMode.Strict,
			IsEssential = true,
			MaxAge = TimeSpan.FromDays(7)
		});
		await _userManager.SaveChangesAsync();
		return Ok();
	}
	
	[HttpPatch]
	public async Task<ActionResult> ChangeDisplayName(string newName)
	{
		User? user = await _userManager.GetUserByTokenAsync(Request);

		if (user is null)
		{
			return Unauthorized("You are not logged in.");
		}
		
		user.DisplayName = newName;
		await _userManager.SaveChangesAsync();
		return Ok();
	}

	[HttpGet]
	public async Task<string?> GetDisplayName()
	{
		User? user = await _userManager.GetUserByTokenAsync(Request);

		return user?.DisplayName;
	}
}