﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Service.Registration.Database;

namespace Service.Registration.Database.Migrations
{
    [DbContext(typeof(RegistrationContext))]
    partial class RegistrationContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("registration")
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.3")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("Service.Registration.Database.RegistrationEntity", b =>
                {
                    b.Property<string>("BrokerId")
                        .HasColumnType("text");

                    b.Property<string>("ClientId")
                        .HasColumnType("text");

                    b.Property<string>("BrandId")
                        .HasColumnType("text");

                    b.Property<DateTime>("RegistrationTime")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("BrokerId", "ClientId");

                    b.ToTable("registrations");
                });
#pragma warning restore 612, 618
        }
    }
}
