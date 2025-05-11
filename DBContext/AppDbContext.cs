using Microsoft.EntityFrameworkCore;
using EmployeeMgt.Models;
using System.Collections.Generic;

namespace EmployeeMgt.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }
    }
}
