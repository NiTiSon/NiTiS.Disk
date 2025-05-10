using System;
using System.Buffers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NiTiS.Disk.Api.Data;

namespace NiTiS.Disk.Api.Managers;

public sealed class UserManager : ManagerBase
{
	public const string SessionKey = "__token";

	private readonly DiskDbContext _context;
	private readonly ILogger<UserManager> _logger;

	public UserManager(DiskDbContext context, ILogger<UserManager> logger)
	{
		_context = context;
		_logger = logger;
	}

	public async ValueTask<bool> CheckIfExistsAsync(string username)
	{
		User? user = await _context.Users.FirstOrDefaultAsync(user => user.Username == username);
		return user != null;
	}

	public async Task<User?> TryRegisterUserAsync(User user)
	{
		try
		{
			user = (await _context.Users.AddAsync(user)).Entity;
			return user;
		}
		catch (SqliteException ex)
		{
			if (_logger.IsEnabled(LogLevel.Critical))
			{
				_logger.LogCritical(ex.ToString());
			}
		}

		return null;
	}

	public Task<User?> GetUserByLoginAsync(string login)
	{
		return _context.Users.FirstOrDefaultAsync(t => t.Username == login);
	}

	public bool VerifyPassword(User user, string password)
	{
		return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
	}
	
	public Session AllocateNewSession(User owner)
	{
		return _context.Sessions.Add(new Session()
		{
			Token = RandomSessionToken(),
			User = owner,
		}).Entity;
	}

	public Task<Session?> GetSessionAsync(HttpRequest request)
	{
		if (request.Cookies.ContainsKey(SessionKey))
		{
			return GetSessionAsync(request.Cookies[SessionKey]!);
		}
		
		return Task.FromResult<Session?>(null);
	}

	public async Task<Session?> GetSessionAsync(string token)
	{
		DateTime currentTime = DateTime.UtcNow;
		Session? session = await _context.Sessions.FirstOrDefaultAsync(t => t.Token == token);

		if (session == null)
			return null;

		return currentTime > session.ExpirationDate ? null : session;
	}
	
	public Task<User?> GetUserByTokenAsync(HttpRequest request)
	{
		if (request.Cookies.ContainsKey(SessionKey))
		{
			return GetUserByTokenAsync(request.Cookies[SessionKey]!);
		}
		
		return Task.FromResult<User?>(null);
	}

	public async Task<User?> GetUserByTokenAsync(string token)
	{
		DateTime currentTime = DateTime.UtcNow;
		Session? session = await _context.Sessions.Include(t => t.User).FirstOrDefaultAsync(t => t.Token == token);

		if (session == null)
			return null;

		return currentTime > session.ExpirationDate ? null : session.User;
	}

	public async Task<bool> SaveChangesAsync()
	{
		try
		{
			await _context.SaveChangesAsync();
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	private const string AlphabetLower = "abcdefghijklmnopqrstuvwxyz";
	private const string AlphabetUpper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
	private const string Numbers = "1234567890";
	private const string Special = "~`!@#$%^&*()_-+={[}]|\\:;\"'<,>.?/";
	private static readonly SearchValues<char> ValidCharacters = SearchValues.Create([..AlphabetLower, ..AlphabetUpper, ..Numbers, ..Special]);
	public static bool EnsurePasswordIsValid(string password)
	{
		string pass = password;
		for (int i = pass.Length - 1; i >= 0; i--)
		{
			if (!ValidCharacters.Contains(pass[i]))
				return false;
		}

		return true;
	}

	public static string HashPassword(string password)
	{
		return BCrypt.Net.BCrypt.HashPassword(password);
	}

	private static string RandomSessionToken()
	{
		Span<char> buffer = stackalloc char[128];
		Random shared = Random.Shared;

		for (int i = 0; i < buffer.Length; i++)
		{
			int c = shared.Next(0, 64);

			buffer[i] = c switch
			{
				< 10 => (char)('0' + c),
				< 36 => (char)('A' + c - 10),
				_ => (char)('a' + c - 36)
			};
		}

		return new string(buffer);
	}
}