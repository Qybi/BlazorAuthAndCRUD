using CLED.BlazorAuthentication.Components;
using CLED.BlazorAuthentication.Components.Account;
using CLED.BlazorAuthentication.Data;
using CLED.BlazorAuthentication.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CLED.BlazorAuthentication
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddRazorComponents()
				.AddInteractiveServerComponents();

			builder.Services.AddCascadingAuthenticationState();
			builder.Services.AddScoped<IdentityUserAccessor>();
			builder.Services.AddScoped<IdentityRedirectManager>();
			builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();
			builder.Services.AddScoped<ProductService>();

			builder.Services.AddAuthentication(options =>
				{
					options.DefaultScheme = IdentityConstants.ApplicationScheme;
					options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
				})
				.AddIdentityCookies();

			var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
			builder.Services.AddDbContext<ApplicationDbContext>(options =>
				options.UseMySQL(connectionString));
			builder.Services.AddDatabaseDeveloperPageExceptionFilter();

			builder.Services.AddIdentityCore<ApplicationUser>(options =>
			{
				options.SignIn.RequireConfirmedAccount = true;
				options.Password.RequiredLength = 6;
			})
				.AddEntityFrameworkStores<ApplicationDbContext>()
				.AddSignInManager()
				.AddDefaultTokenProviders();

			builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseMigrationsEndPoint();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();

			app.UseStaticFiles();
			app.UseAntiforgery();

			app.MapRazorComponents<App>()
				.AddInteractiveServerRenderMode();

			// Add additional endpoints required by the Identity /Account Razor components.
			app.MapAdditionalIdentityEndpoints();

			app.Run();
		}
	}
}
