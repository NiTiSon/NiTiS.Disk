using System.Buffers;
using System.Collections.Generic;

internal static class EnsurePasswordModule
{
	// | Method                          | Mean       | Error    | StdDev   |
	// |-------------------------------- |-----------:|---------:|---------:|
	// | EnsurePassword_SearchValuesImpl |   673.8 ns |  9.86 ns |  8.74 ns |
	// | EnsurePassword_HashSetImpl      | 3,216.6 ns | 27.34 ns | 22.83 ns |

	private static readonly string AlphabetLower = "abcdefghijklmnopqrstuvwxyz";
	private static readonly string AlphabetUpper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
	private static readonly string Numbers = "1234567890";
	private static readonly string Special = "~`!@#$%^&*()_-+={[}]|\\:;\"'<,>.?/";
	private static readonly SearchValues<char> ValidCharacters = SearchValues.Create([.. AlphabetLower, ..AlphabetLower, ..Numbers, ..Special]);
	public static bool EnsurePasswordIsSecureSearchValues(string password)
	{
		string pass = password;
		for (int i = pass.Length - 1; i >= 0; i--)
		{
			if (!ValidCharacters.Contains(pass[i]))
				return false;
		}

		return true;
	}

	private static readonly HashSet<char> ValidCharacters2 = [.. AlphabetLower, ..AlphabetLower, ..Numbers, ..Special];
	public static bool EnsurePasswordIsSecureHashSet(string password)
	{
		string pass = password;
		for (int i = pass.Length - 1; i >= 0; i--)
		{
			if (!ValidCharacters2.Contains(pass[i]))
				return false;
		}

		return true;
	}
}