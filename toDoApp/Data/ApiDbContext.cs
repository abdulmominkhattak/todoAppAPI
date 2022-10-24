using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using toDoApp.Models;

namespace toDoApp.Data
{
    public class ApiDbContext : IdentityDbContext
    {
        public ApiDbContext(DbContextOptions options) : base(options)
        {

        }

        public virtual DbSet<ItemData> items { get; set; }
    }
}
