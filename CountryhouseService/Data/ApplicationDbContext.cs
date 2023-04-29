using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CountryhouseService.Models;

namespace CountryhouseService.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }


        public DbSet<Image> Images { get; set; }
        public DbSet<Ad> Ads { get; set; }
        public DbSet<Request> Requests { get; set; }
    }
}