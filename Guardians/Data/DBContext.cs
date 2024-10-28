using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Guardians.Models;

namespace Guardians.Data
{
    public class DBContext : DbContext
    {
        public DBContext (DbContextOptions<DBContext> options)
            : base(options)
        {
        }

        public DbSet<Guardians.Models.Member> Members { get; set; } = default!;
        public DbSet<Guardians.Models.Unit> Units { get; set; } = default!;
        public DbSet<Guardians.Models.Role> Roles { get; set; } = default!;
        public DbSet<SystemLog> SystemLogs { get; set; }
        public DbSet<LoginLog> LoginLogs { get; set; }
    }
}
