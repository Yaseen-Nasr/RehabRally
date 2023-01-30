using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RehabRally.Web.Core.Mapping;
using RehabRally.Web.Core.Models;
using RehabRally.Web.Data;
using RehabRally.Web.Helpers;
using RehabRally.Web.Seeds;
using RehabRally.Web.Services;
using System.Reflection;
using System.Text;
using UoN.ExpressiveAnnotations.NetCore.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();
//Add Custom Claimes Config As ClaimTypes.GivenName
builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, ApplicationUserClaimsPrincipalFactory>();

builder.Services.AddCors();
builder.Services.AddAuthentication(options =>
{

})
                .AddJwtBearer(o =>
                {
                    o.RequireHttpsMetadata = false;
                    o.SaveToken = false;
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = builder.Configuration["JWT:Issuer"],
                        ValidAudience = builder.Configuration["JWT:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
                    };
                });
builder.Services.AddAuthorization();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 8;
    options.User.RequireUniqueEmail = true;
});
builder.Services.AddControllersWithViews();
 
builder.Services.AddTransient<IImageService, ImageService>(); 
builder.Services.AddScoped<IAuthService, AuthService>(); 
builder.Services.AddAutoMapper(Assembly.GetAssembly(typeof(MappingProfile)));
builder.Services.AddExpressiveAnnotations();

 var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseMigrationsEndPoint();
}
else
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseCors(o => o.AllowAnyHeader());

app.UseAuthentication();
app.UseAuthorization();

var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
using var scope = scopeFactory.CreateScope();
var roleManger = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
var userManger = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
await DefaultRoles.SeedRolesAsync(roleManger);
await DefaultUsers.SeedAdminUserAsync(userManger);
app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
