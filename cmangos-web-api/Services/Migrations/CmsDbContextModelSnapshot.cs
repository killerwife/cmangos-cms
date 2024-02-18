﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Services.Repositories;

#nullable disable

namespace Services.Migrations
{
    [DbContext(typeof(CmsDbContext))]
    partial class CmsDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Data.Model.AccountExtension", b =>
                {
                    b.Property<uint>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int unsigned");

                    b.Property<DateTime?>("EmailChanged")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("PasswordRecoverySent")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("PasswordRecoveryToken")
                        .HasColumnType("longtext");

                    b.Property<string>("PendingEmail")
                        .HasColumnType("longtext");

                    b.Property<string>("PendingEmailToken")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("PendingEmailTokenSent")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("PendingToken")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("TokenChanged")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.ToTable("account_ext", (string)null);
                });

            modelBuilder.Entity("Data.Model.RefreshToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("CreatedByIp")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("Expires")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("ReplacedByToken")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("Revoked")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("RevokedByIp")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<uint>("UserId")
                        .HasColumnType("int unsigned");

                    b.HasKey("Id");

                    b.ToTable("refresh_token", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
