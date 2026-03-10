using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ProjektniMenadzment.Data
{
    public class PMAuthDbContext : IdentityDbContext
    {
        public PMAuthDbContext(DbContextOptions<PMAuthDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Seed Roles(Administrator, Projektni menadzer, Korisnik)

            var adminRoleId = "eed85986-daff-49b9-b500-3d26b4ccc8e2";
            var projektniMenadzerRoleId = "8074580b-334f-48cd-b6e5-1da93c53ef44";
            var korisnikRoleId = "a520939e-26af-44d3-8bb4-77e867f9d550";

            var roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Name = "Administrator",
                    NormalizedName = "Administrator",
                    Id = adminRoleId,
                    ConcurrencyStamp = adminRoleId
                },
                new IdentityRole
                {
                    Name = "ProjektniMenadzer",
                    NormalizedName = "ProjektniMenadzer",
                    Id = projektniMenadzerRoleId,
                    ConcurrencyStamp = projektniMenadzerRoleId
                },
                new IdentityRole
                {
                    Name = "Korisnik",
                    NormalizedName = "Korisnik",
                    Id = korisnikRoleId,
                    ConcurrencyStamp = korisnikRoleId
                }
            };

            builder.Entity<IdentityRole>().HasData(roles);

            var adminId = "881ecb32-8773-4199-8627-05dc87d5a810";
            var adminUser = new IdentityUser
            {
                UserName = "administrator@pmdb.com",
                Email = "administrator@pmdb.com",
                NormalizedEmail = "administrator@pmdb.com".ToUpper(),
                NormalizedUserName = "administrator@pmdb.com".ToUpper(),
                Id = adminId,
            };

            adminUser.PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(adminUser, "Admin123!");

            builder.Entity<IdentityUser>().HasData(adminUser);

            // Add all roles to Administrator

            var adminRoles = new List<IdentityUserRole<string>>
            {
                new IdentityUserRole<string>
                {
                    RoleId = korisnikRoleId,
                    UserId = adminId
                },
                new IdentityUserRole<string>
                {
                    RoleId = projektniMenadzerRoleId,
                    UserId = adminId
                },
                new IdentityUserRole<string>
                {
                    RoleId = adminRoleId,
                    UserId = adminId
                }
            };

            builder.Entity<IdentityUserRole<string>>().HasData(adminRoles);
        }
    }
}
