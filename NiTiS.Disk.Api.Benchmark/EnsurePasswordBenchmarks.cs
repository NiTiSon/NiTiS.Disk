using BenchmarkDotNet.Attributes;

namespace NiTiS.Disk.Api.Benchmark
{
	public class EnsurePasswordBenchmarks
	{
		public volatile bool isValid;

		[Benchmark]
		public void EnsurePassword_SearchValuesImpl()
		{
			string[] passwords = TestPasswords;

			for (int i = passwords.Length - 1; i >= 0; i--)
			{
				isValid = EnsurePasswordModule.EnsurePasswordIsSecure_SearchValues(passwords[i]);
			}
		}
		
		[Benchmark]
		public void EnsurePassword_Span_SearchValuesImpl()
		{
			string[] passwords = TestPasswords;

			for (int i = passwords.Length - 1; i >= 0; i--)
			{
				isValid = EnsurePasswordModule.EnsurePasswordIsSecure_SearchValues_Span(passwords[i]);
			}
		}

		[Benchmark]
		public void EnsurePassword_HashSetImpl()
		{
			string[] passwords = TestPasswords;

			for (int i = passwords.Length - 1; i >= 0; i--)
			{
				isValid = EnsurePasswordModule.EnsurePasswordIsSecure_HashSet(passwords[i]);
			}
		}
		
		[Benchmark]
		public void EnsurePassword_Span_HashSetImpl()
		{
			string[] passwords = TestPasswords;

			for (int i = passwords.Length - 1; i >= 0; i--)
			{
				isValid = EnsurePasswordModule.EnsurePasswordIsSecure_HashSet_Span(passwords[i]);
			}
		}

		public static readonly string[] TestPasswords = 
		{
			// Valid passwords (alphanumeric + allowed symbols)
			"P@ssw0rd", "Secur3!T", "C0mpl3x~Pwd", "abc123", "A1B2C3D4",
			"Test_123", "HelloWorld!", "P@$$w0rd", "Qwerty!234", "Admin#2024",
			"LetMeIn42", "TrustNo1*", "Winter2023!", "Summer@2024", "Fall_2025",
			"Spring#2026", "Pa$$w0rd", "Guest123!", "Root~Access", "Super$ecure",
			"AlphaBeta123", "GammaDelta#456", "EpsilonZeta*789", "EtaTheta_101", "IotaKappa!202",
			"LambdaMu$303", "NuXi404%", "OmicronPi505^", "RhoSigma606&", "TauUpsilon707(",
			"PhiChi808)", "PsiOmega909-", "AaBbCc123+", "DdEeFf456=", "GgHhIi789[",
			"JjKkLl012]", "MmNnOo345|", "PpQqRr678\\", "SsTtUu901:", "VvWwXx234;",
			"YyZz567\"", "12345678'", "98765432<", "ABCDEFGH>", "IJKLMNOP?",
			"QRSTUVWX/", "YZabcdef.", "ghijklmn,", "opqrstuv`", "wxyz~!@",

			// Invalid passwords (containing disallowed chars)
			"Pass with Space", "Emoji😊Pwd", "Unicode✓", "New\nLine", "Tab\tHere",
			"Null\0Char", "Quote\"Fail", "Back\\Slash", "Slash/Error", "Pipe|Fail",
			"Colon:Bad", "Semi;Bad", "Comma,No", "Period.No", "Apostrophe'No",
			"Less<Bad", "Greater>Bad", "Question?No", "Tilde~Bad", "Backtick`No",
			"Excl!No", "At@No", "Hash#No", "Dollar$No", "Percent%No",
			"Caret^No", "Amp&No", "Star*No", "Paren(No", "Paren)No",
			"Minus-No", "Plus+No", "Equal=No", "Brace[No", "Brace]No",
			"Curly{No", "Curly}No", "Pipe|No", "Back\\No", "Slash/No",
			"Space No", "Control\u0001Char", "NonASCIIé", "Currency¥", "Math√",
			"Arrow→", "Heart♥", "Copyright©", "Trademark™", "Registered®",
			"Degree°", "PlusMinus±", "Square²", "Cube³", "Microµ",
			"Paragraph¶", "Middle·", "Division÷", "OmegaΩ", "Alphaα",
			"Betaβ", "Gammaγ", "Deltaδ", "Epsilonε", "Zetaζ",
			"Etaη", "Thetaθ", "Iotaι", "Kappaκ", "Lambdaλ",
			"Muμ", "Nuν", "Xiξ", "Omicronο", "Piπ",
			"Rhoρ", "Sigmaσ", "Tauτ", "Upsilonυ", "Phiφ",
			"Chiχ", "Psiψ", "Omegaω"
		};
	}
}