using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NiTiS.Disk.Api;
using Scalar.AspNetCore;

namespace NiTiS.Disk.Web;

public static class Program
{
	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		builder.Services.AddOpenApi();

		builder.ApplyNiTiSDiskApi();

		builder.Services.AddRazorPages();

		WebApplication app = builder.Build();

		if (!app.Environment.IsDevelopment())
		{
			app.UseExceptionHandler("/Error");
			app.UseHsts();
		}

		app.UseHttpsRedirection();

		app.UseRouting(); 

		app.UseAuthorization();

		app.MapStaticAssets();
		app.MapRazorPages()
			.WithStaticAssets();

		app.MapOpenApi();
		app.MapScalarApiReference("/docs/api", options =>
		{
			options.Title = "NiTiS Disk API";
			options.DefaultHttpClient = new(ScalarTarget.CSharp, ScalarClient.HttpClient);
		});

		app.MapControllers();

		app.Run();
	}
}