﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using webapp.data;

#nullable disable

namespace webapp.Migrations
{
    [DbContext(typeof(WebAppContext))]
    partial class UserProfileContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("webapp.model.Service", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("UserProfileId")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("character varying(36)");

                    b.Property<string>("description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("isDeleted")
                        .HasColumnType("boolean");

                    b.Property<string[]>("multimedia")
                        .IsRequired()
                        .HasColumnType("text[]");

                    b.Property<string>("phoneNumber")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("serviceName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string[]>("tags")
                        .IsRequired()
                        .HasColumnType("text[]");

                    b.HasKey("Id");

                    b.HasIndex("UserProfileId");

                    b.ToTable("Services");
                });

            modelBuilder.Entity("webapp.model.UserProfile", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(36)
                        .HasColumnType("character varying(36)");

                    b.Property<string>("address")
                        .HasColumnType("text");

                    b.Property<DateTime>("birthDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("fullName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("isDeleted")
                        .HasColumnType("boolean");

                    b.Property<bool>("isVerified")
                        .HasColumnType("boolean");

                    b.Property<string>("legalDocumentID")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("profilePictureID")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("email")
                        .IsUnique();

                    b.HasIndex("legalDocumentID")
                        .IsUnique();

                    b.HasIndex("profilePictureID")
                        .IsUnique();

                    b.ToTable("UserProfiles");
                });

            modelBuilder.Entity("webapp.model.Service", b =>
                {
                    b.HasOne("webapp.model.UserProfile", "UserProfile")
                        .WithMany()
                        .HasForeignKey("UserProfileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("UserProfile");
                });
#pragma warning restore 612, 618
        }
    }
}