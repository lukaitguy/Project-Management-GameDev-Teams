using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProjektniMenadzment.Data;
using ProjektniMenadzment.Models.Domain;
using ProjektniMenadzment.Repositories;
using ProjektniMenadzment.Repositories.Interfaces;
using ProjektniMenadzment.Services;
using ProjektniMenadzment.Services.Interfaces;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// --- Databases ---
builder.Services.AddDbContext<PMDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PMConnectionString")));
builder.Services.AddDbContext<PMAuthDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PMAuthDbConnectionString")));

// --- Identity (use Core so it doesn't override JWT as default scheme) ---
builder.Services.AddIdentityCore<IdentityUser>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<PMAuthDbContext>()
.AddSignInManager()
.AddDefaultTokenProviders();

// --- JWT Authentication ---
var jwtKey = builder.Configuration["Jwt:Key"]!;
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

// --- CORS ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
        policy.WithOrigins("https://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// --- Repositories ---
builder.Services.AddScoped<IProjektiRepository, ProjektiRepository>();
builder.Services.AddScoped<IZadaciRepository, ZadaciRepository>();
builder.Services.AddScoped<IZanroviRepository, ZanroviRepository>();
builder.Services.AddScoped<IKorisniciRepository, KorisniciRepository>();
builder.Services.AddScoped<IClanoviProjektaRepository, ClanoviProjektaRepository>();
builder.Services.AddScoped<IResursiRepository, ResursiRepository>();
builder.Services.AddScoped<IAdminKorisniciRepository, AdminKorisniciRepository>();
builder.Services.AddScoped<IZadaciKomentarRepository, ZadaciKomentarRepository>();
builder.Services.AddScoped<IBuildoviRepository, BuildoviRepository>();

// --- Services ---
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IProjektiService, ProjektiService>();
builder.Services.AddScoped<IZanroviService, ZanroviService>();
builder.Services.AddScoped<IZadaciService, ZadaciService>();
builder.Services.AddScoped<IKomentariService, KomentariService>();
builder.Services.AddScoped<IResursiService, ResursiService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAdminKorisniciService, AdminKorisniciService>();
builder.Services.AddScoped<IIzvestajiService, IzvestajiService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.MapGet("/", () => Results.Redirect("/app"));
app.UseRouting();
app.UseCors("AllowAngular");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapFallbackToFile("/app/{*path:nonfile}", "app/index.html");

app.Run();