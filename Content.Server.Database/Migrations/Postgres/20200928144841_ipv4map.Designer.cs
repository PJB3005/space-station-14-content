﻿﻿// <auto-generated />
using System;
using System.Net;
using Content.Server.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Content.Server.Database.Migrations.Postgres
{
    [DbContext(typeof(PostgresServerDbContext))]
    [Migration("20200928144841_ipv4map")]
    partial class ipv4map
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Content.Server.Database.Antag", b =>
                {
                    b.Property<int>("AntagId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("AntagName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("ProfileId")
                        .HasColumnType("integer");

                    b.HasKey("AntagId");

                    b.HasIndex("ProfileId", "AntagName")
                        .IsUnique();

                    b.ToTable("Antag");
                });

            modelBuilder.Entity("Content.Server.Database.AssignedUserId", b =>
                {
                    b.Property<int>("AssignedUserIdId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("AssignedUserIdId");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.HasIndex("UserName")
                        .IsUnique();

                    b.ToTable("AssignedUserIds");
                });

            modelBuilder.Entity("Content.Server.Database.Job", b =>
                {
                    b.Property<int>("JobId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("JobName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Priority")
                        .HasColumnType("integer");

                    b.Property<int>("ProfileId")
                        .HasColumnType("integer");

                    b.HasKey("JobId");

                    b.HasIndex("ProfileId");

                    b.ToTable("Job");
                });

            modelBuilder.Entity("Content.Server.Database.PostgresServerBan", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<ValueTuple<IPAddress, int>?>("Address")
                        .HasColumnType("inet");

                    b.Property<DateTime>("BanTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("BanningAdmin")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("ExpirationTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Reason")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("Address");

                    b.HasIndex("UserId");

                    b.ToTable("Bans");

                    b.HasCheckConstraint("AddressNotIPv6MappedIPv4", "NOT inet '::ff:0.0.0.0/96' >>= \"Address\"");

                    b.HasCheckConstraint("HaveEitherAddressOrUserId", "\"Address\" IS NOT NULL OR \"UserId\" IS NOT NULL");
                });

            modelBuilder.Entity("Content.Server.Database.PostgresServerUnban", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("BanId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("UnbanTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("UnbanningAdmin")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("BanId")
                        .IsUnique();

                    b.ToTable("Unbans");
                });

            modelBuilder.Entity("Content.Server.Database.Prefs", b =>
                {
                    b.Property<int>("PrefsId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("SelectedCharacterSlot")
                        .HasColumnType("integer");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("PrefsId");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("Preferences");
                });

            modelBuilder.Entity("Content.Server.Database.Profile", b =>
                {
                    b.Property<int>("ProfileId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("Age")
                        .HasColumnType("integer");

                    b.Property<string>("CharacterName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("EyeColor")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FacialHairColor")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FacialHairName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("HairColor")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("HairName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("PreferenceUnavailable")
                        .HasColumnType("integer");

                    b.Property<int>("PrefsId")
                        .HasColumnType("integer");

                    b.Property<string>("Sex")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("SkinColor")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Slot")
                        .HasColumnType("integer");

                    b.HasKey("ProfileId");

                    b.HasIndex("PrefsId");

                    b.HasIndex("Slot", "PrefsId")
                        .IsUnique();

                    b.ToTable("Profiles");
                });

            modelBuilder.Entity("Content.Server.Database.Antag", b =>
                {
                    b.HasOne("Content.Server.Database.Profile", "Profile")
                        .WithMany("Antags")
                        .HasForeignKey("ProfileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Content.Server.Database.Job", b =>
                {
                    b.HasOne("Content.Server.Database.Profile", "Profile")
                        .WithMany("Jobs")
                        .HasForeignKey("ProfileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Content.Server.Database.PostgresServerUnban", b =>
                {
                    b.HasOne("Content.Server.Database.PostgresServerBan", "Ban")
                        .WithOne("Unban")
                        .HasForeignKey("Content.Server.Database.PostgresServerUnban", "BanId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Content.Server.Database.Profile", b =>
                {
                    b.HasOne("Content.Server.Database.Prefs", "Prefs")
                        .WithMany("Profiles")
                        .HasForeignKey("PrefsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
