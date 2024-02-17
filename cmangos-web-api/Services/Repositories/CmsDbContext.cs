﻿using Data.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Services.Repositories
{
    public class CmsDbContext : DbContext
    {
        public CmsDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(@"Server=(localdb)\mssqllocaldb;Database=Test");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<AccountExtension>(x => x.ToTable("account_ext"));
            builder.Entity<RefreshToken>(x => x.ToTable("refresh_token"));
        }


        public DbSet<AccountExtension> AccountsExt { get; set; }
        public DbSet<RefreshToken> RefreshToken { get; set; }
    }
}
