using Microsoft.EntityFrameworkCore;
using TaskManager.Models;

namespace TaskManager.Data
{
    public class dataContext : DbContext
    {

        public dataContext(DbContextOptions<dataContext> options) : base(options) { } 


        public DbSet<Models.Task> Task { get; set; }


    }
}
