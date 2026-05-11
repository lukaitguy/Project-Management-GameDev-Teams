using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjektniMenadzment.Data;
using ProjektniMenadzment.Models.Domain;
using ProjektniMenadzment.Repositories;
using ProjektniMenadzment.Repositories.Interfaces;
using ProjektniMenadzment.Services;
using ProjektniMenadzment.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<PMDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PMConnectionString")));
builder.Services.AddDbContext<PMAuthDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PMAuthDbConnectionString")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<PMAuthDbContext>();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
});

builder.Services.AddScoped<IProjektiRepository, ProjektiRepository>();
builder.Services.AddScoped<IZadaciRepository, ZadaciRepository>();
builder.Services.AddScoped<IZanroviRepository, ZanroviRepository>();
builder.Services.AddScoped<IKorisniciRepository, KorisniciRepository>();
builder.Services.AddScoped<IClanoviProjektaRepository, ClanoviProjektaRepository>();
builder.Services.AddScoped<IResursiRepository, ResursiRepository>();
builder.Services.AddScoped<IAdminKorisniciRepository, AdminKorisniciRepository>();
builder.Services.AddScoped<IZadaciKomentarRepository, ZadaciKomentarRepository>();
builder.Services.AddScoped<IBuildoviRepository, BuildoviRepository>();


builder.Services.AddScoped<IProjektiService, ProjektiService>();
builder.Services.AddScoped<IZanroviService, ZanroviService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error"); ;
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();


app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapFallbackToFile("/app/{*path:nonfile}", "app/index.html");

app.Run();
