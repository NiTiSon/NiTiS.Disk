using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NiTiS.Disk.Api.Data;

namespace NiTiS.Disk.Api;

public static class NiTiSDiskApi
{
	public static void ApplyNiTiSDiskApi(this WebApplicationBuilder builder)
	{
		builder.Services.AddDbContext<DiskDbContext>(options => options.UseSqlite("Data Source=./nitisdisk.db"));
		builder.Services.AddControllers();
	}
}
