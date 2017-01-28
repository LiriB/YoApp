﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using YoApp.Backend.Models;

namespace YoApp.Backend.Data.EF
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<VerificationtRequest> VerificationtRequests { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            :base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ApplicationUser>()
                .Property(au => au.Status)
                .HasMaxLength(30);

            builder.Entity<ApplicationUser>()
                .Property(au => au.NickName)
                .HasMaxLength(20);

            builder.Entity<VerificationtRequest>()
                .Property(vr => vr.PhoneNumber)
                .HasMaxLength(30);

            builder.Entity<VerificationtRequest>()
                .Property(vr => vr.VerificationCode)
                .HasMaxLength(8);

            base.OnModelCreating(builder);
        }
    }
}
