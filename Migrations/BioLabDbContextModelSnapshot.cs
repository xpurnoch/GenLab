﻿// <auto-generated />
using System;
using BioLabManager.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BioLabManager.Migrations
{
    [DbContext(typeof(BioLabDbContext))]
    partial class BioLabDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.4");

            modelBuilder.Entity("BioLabManager.Models.Lab", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Labs");
                });

            modelBuilder.Entity("BioLabManager.Models.Sample", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CollectedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Identifier")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("LabId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SampleType")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Sequence")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("StorageLocation")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("LabId");

                    b.HasIndex("UserId");

                    b.ToTable("Samples");
                });

            modelBuilder.Entity("BioLabManager.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("LabId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("LabId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("BioLabManager.Models.Sample", b =>
                {
                    b.HasOne("BioLabManager.Models.Lab", "Lab")
                        .WithMany("Samples")
                        .HasForeignKey("LabId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BioLabManager.Models.User", "User")
                        .WithMany("Samples")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Lab");

                    b.Navigation("User");
                });

            modelBuilder.Entity("BioLabManager.Models.User", b =>
                {
                    b.HasOne("BioLabManager.Models.Lab", "Lab")
                        .WithMany("Users")
                        .HasForeignKey("LabId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Lab");
                });

            modelBuilder.Entity("BioLabManager.Models.Lab", b =>
                {
                    b.Navigation("Samples");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("BioLabManager.Models.User", b =>
                {
                    b.Navigation("Samples");
                });
#pragma warning restore 612, 618
        }
    }
}
