using GymApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GymApi.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options)
        {
        }
        public DbSet<Member> Members { get; set; }

        public DbSet<MembershipTypes> MembershipTypes { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Enforce unique database index for MemberCode[cite: 1]
            builder.Entity<Member>()
                .HasIndex(m => m.MemberCode)
                .IsUnique();

            // Seed default membership types[cite: 1]
            builder.Entity<MembershipTypes>().HasData(
                new MembershipTypes { Id = 1, Name = "Standart", Code = "ST" },
                new MembershipTypes { Id = 2, Name = "VIP", Code = "VP" },
                new MembershipTypes { Id = 3, Name = "Öğrenci", Code = "OG" },
                new MembershipTypes { Id = 4, Name = "Kurumsal", Code = "KR" }
            );

            List<IdentityRole> roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id = "ADMIN_ROLE",
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    ConcurrencyStamp = "1"
                },
                new IdentityRole
                {
                    Id = "TRAINER_ROLE",
                    Name = "Trainer",
                    NormalizedName = "TRAINER",
                    ConcurrencyStamp = "2"
                },
                new IdentityRole
                {
                    Id = "USER_ROLE",
                    Name = "User",
                    NormalizedName = "USER",
                    ConcurrencyStamp = "3"
                }
            };
            builder.Entity<IdentityRole>().HasData(roles);
        }
    }
}