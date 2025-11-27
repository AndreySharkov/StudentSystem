using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFirstLab.Data
{
    public class ForumDbContext : DbContext
    {
        public ForumDbContext() { }

        public ForumDbContext(DbContextOptions<ForumDbContext> options) : base(options)
        {

        }

        public DbSet<Post>
    }
}
