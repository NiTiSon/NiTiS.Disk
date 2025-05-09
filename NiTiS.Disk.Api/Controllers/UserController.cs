using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Net.ServerSentEvents;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Microsoft.OpenApi.Validations.Rules;
using NiTiS.Disk.Api.Data;

namespace NiTiS.Disk.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class UserController : ControllerBase
{
	private readonly DiskDbContext _context;

	public UserController(DiskDbContext context)
	{
		_context = context;
	}

	[HttpGet("{userId:long}")]
	public async Task<ActionResult> GetUserInfo(long userId)
	{
		User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

		if (user == null)
		{
			return NotFound("User not found");
		}
		else
		{
			return Ok(user);
		}
	}

	[HttpPost("{displayName}")]
	public async Task<ActionResult> RegisterUser(string displayName)
	{
		StringValues loginValues = Request.HttpContext.Request.Headers["X-Username"];
		StringValues passwordValues = Request.HttpContext.Request.Headers["X-Password"];

		string? login = loginValues.FirstOrDefault();
		string? password = passwordValues.FirstOrDefault();

		if (login == null || password == null)
		{
			return BadRequest("X-Username and X-Password headers are required");
		}

		if (password.Length is < 6 or > 32)
		{
			return BadRequest("Password length must be between 6 and 32 characters");
		}

		if (login.Length is < 4 or > 64)
		{
			return BadRequest("Login length must be between 4 and 64 characters");
		}

		string passwordHash = HashPassword(password);

		
	}
	
	[HttpGet]
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
		
		string passwordHash = HashPassword(password);

		Login? userLogin = _context.Logins
			.Include(t => t.AssociatedUser)
			.Where(u => u.Username == login)
			.FirstOrDefault(u => u.PasswordHash == passwordHash);

		if (userLogin == null)
		{
			return Unauthorized("Invalid username or password");
		}
		
		return Ok(CreateNewSession(userLogin.AssociatedUser));
	}

	private string HashPassword(string password)
	{
		return BCrypt.Net.BCrypt.HashPassword(password);
	}

	private Session CreateNewSession(User owner)
	{
		return _context.Sessions.Add(new Session()
		{
			Token = CreateSessionToken(),
			User = owner,
		}).Entity;
	}

	private string CreateSessionToken()
	{
		Span<char> buffer = stackalloc char[128];
		Random shared = Random.Shared;

		for (int i = 0; i < buffer.Length; i++)
		{
			int c = shared.Next(0, 256);

			buffer[i] = c switch
			{
				< 10 => (char)('0' + c),
				< 36 => (char)('A' + c - 10),
				_ => (char)('a' + c - 36)
			};
		}

		return new string(buffer);
	}

	private static readonly string AlphabetLower = "abcdefghijklmnopqrstuvwxyz";
	private static readonly string AlphabetUpper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
	private static readonly string Numbers = "1234567890";
	private static readonly SearchValues<char> ValidCharacters = SearchValues.Create([.. AlphabetLower, ..AlphabetLower, ..Numbers]);
	internal bool EnsurePasswordIsSecure(string password)
	{
		ValidCharacters.Contains(password[0]);
	}

	private static readonly HashSet<char> ValidCharacters2 = [.. AlphabetLower, ..AlphabetLower, ..Numbers];
	internal bool EnsurePasswordIsSecureHashSet(string password)
	{
		string pass = password;
		for (int i = ValidCharacters2.Count - 1; i >= 0; i--)
		{
			if (!ValidCharacters2.Contains(pass[i]))
				return false;
		}
		return true;
	}
}