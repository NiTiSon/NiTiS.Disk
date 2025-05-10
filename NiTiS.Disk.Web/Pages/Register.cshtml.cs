using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using NiTiS.Disk.Api.Data;
using NiTiS.Disk.Api.Managers;

namespace NiTiS.Disk.Web.Pages;

public class RegisterModel : BaseModel
{
	public InputView Input { get; set; } = new();

	public RegisterModel(UserManager userManager) : base(userManager) {}

	public async Task OnGet()
	{
		await CheckAuthorization();
	}

	public async Task OnPost(InputView input)
	{
		if (input.Password is null || input.Username is null) return;
		
		User? user = new()
		{
			DisplayName = input.Username,
			PasswordHash = UserManager.HashPassword(input.Password),
			Username = input.Username
		};
		
		user = await UserManager.TryRegisterUserAsync(user);
		Session? session = null;
		if (user != null)
		{
			session = (UserManager.AllocateNewSession(user));
		}

		if (await UserManager.SaveChangesAsync() && session != null)
		{
			Response.Apply(session);
		}
	}
	
	public class InputView
	{
		[property: Required(ErrorMessage = "Username is required.")]
		[property: StringLength(64, MinimumLength = 4)]
		[property: DataType(DataType.Text)]
		[property: RegularExpression("^[a-zA-Z0-9_-]*$",
			ErrorMessage = "Only alphanumeric characters, numbers and underscore are allowed.")]
		public string? Username { get; set; }

		[property: Required(ErrorMessage = "Password is required.")]
		[property: StringLength(32, MinimumLength = 6)]
		[property: DataType(DataType.Password)]
		[property: RegularExpression("^[a-zA-Z0-9~`!@#$%^&*()_\\-+={[}\\]|\\\\:;\"'<,>.?/]*$",
			ErrorMessage = "Password must contain only latin letters, numbers and special symbols.")]
		public string? Password { get; set; }

		[property: Required(ErrorMessage = "Confirm password is required.")]
		[property: Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		[property: DataType(DataType.Password)]
		[property: Display(Name = "Confirm Password")]
		public string? ConfirmPassword { get; set; }
	}
}