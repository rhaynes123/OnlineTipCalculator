using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineTipCalculator.Models;

namespace OnlineTipCalculator.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Calculation> Calculations { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Calculation>( e => {
                e.Property(c =>c.Id);
                e.Property(c => c.ResultAmount);
                e.Property(c => c.TipType);
                e.Property(c => c.UserId);
                e.Property(c => c.CreatedDateTime);
            });
        }
    }
}
