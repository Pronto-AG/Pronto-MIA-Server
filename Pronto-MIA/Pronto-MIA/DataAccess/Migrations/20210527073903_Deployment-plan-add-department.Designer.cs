﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Pronto_MIA.DataAccess;

namespace Pronto_MIA.DataAccess.Migrations
{
    [DbContext(typeof(ProntoMiaDbContext))]
    [Migration("20210527073903_Deployment-plan-add-department")]
    partial class Deploymentplanadddepartment
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityByDefaultColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.5");

            modelBuilder.Entity("Pronto_MIA.Domain.Entities.AccessControlList", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<bool>("CanEditDepartments")
                        .HasColumnType("boolean");

                    b.Property<bool>("CanEditDeploymentPlans")
                        .HasColumnType("boolean");

                    b.Property<bool>("CanEditUsers")
                        .HasColumnType("boolean");

                    b.Property<bool>("CanViewDepartments")
                        .HasColumnType("boolean");

                    b.Property<bool>("CanViewDeploymentPlans")
                        .HasColumnType("boolean");

                    b.Property<bool>("CanViewUsers")
                        .HasColumnType("boolean");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("AccessControlLists");

                    b.HasData(
                        new
                        {
                            Id = -1,
                            CanEditDepartments = true,
                            CanEditDeploymentPlans = true,
                            CanEditUsers = true,
                            CanViewDepartments = true,
                            CanViewDeploymentPlans = true,
                            CanViewUsers = true,
                            UserId = -1
                        });
                });

            modelBuilder.Entity("Pronto_MIA.Domain.Entities.Department", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Departments");
                });

            modelBuilder.Entity("Pronto_MIA.Domain.Entities.DeploymentPlan", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<DateTime>("AvailableFrom")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("AvailableUntil")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("DepartmentId")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("FileExtension")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FileUuid")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("Published")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.HasIndex("DepartmentId");

                    b.HasIndex("FileUuid")
                        .IsUnique();

                    b.ToTable("DeploymentPlans");
                });

            modelBuilder.Entity("Pronto_MIA.Domain.Entities.FcmToken", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<int>("OwnerId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("FcmTokens");
                });

            modelBuilder.Entity("Pronto_MIA.Domain.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<int?>("DepartmentId")
                        .HasColumnType("integer");

                    b.Property<string>("HashGenerator")
                        .HasColumnType("text");

                    b.Property<string>("HashGeneratorOptions")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<byte[]>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("DepartmentId");

                    b.HasIndex("UserName")
                        .IsUnique();

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = -1,
                            HashGenerator = "Pbkdf2Generator",
                            HashGeneratorOptions = "{\"SaltSize\":128,\"HashIterations\":1500,\"HashSize\":512,\"Salt\":\"A+16bv/SvaC7ZJgS7u+CB8nN32PBUAbJuT09NigsCzQx6/CxS1I/5laUaFoJNZ3QhTm4TqFnWYzokdrvrUxbOEN0MN3ZhINcblSLF9LwbZeiT0nYOnQgTBEPL0KoszXdm8x2mYXHAJFYQ9KOsIZregzuiBQfSqsFfR2uDnFHm9o=\"}",
                            PasswordHash = new byte[] { 160, 170, 181, 57, 56, 11, 64, 111, 255, 135, 44, 16, 13, 143, 10, 57, 47, 144, 79, 128, 47, 6, 240, 159, 91, 163, 33, 239, 51, 140, 5, 59, 228, 56, 155, 57, 18, 151, 143, 191, 223, 64, 140, 19, 23, 125, 90, 34, 241, 76, 199, 118, 197, 240, 49, 56, 110, 38, 182, 112, 19, 172, 195, 113, 47, 223, 184, 44, 27, 214, 16, 242, 169, 148, 104, 135, 116, 43, 152, 103, 130, 206, 221, 110, 254, 123, 231, 102, 42, 239, 67, 130, 53, 113, 12, 150, 249, 50, 117, 225, 113, 38, 174, 80, 246, 112, 27, 104, 229, 197, 63, 76, 246, 218, 33, 63, 197, 145, 244, 65, 91, 81, 228, 81, 208, 91, 117, 249, 101, 112, 179, 10, 130, 136, 0, 168, 150, 224, 43, 124, 151, 107, 110, 10, 23, 83, 212, 80, 45, 113, 201, 148, 17, 114, 46, 169, 232, 169, 138, 135, 89, 53, 154, 213, 123, 208, 0, 155, 155, 0, 44, 249, 199, 222, 211, 120, 137, 158, 135, 20, 247, 225, 64, 119, 64, 177, 76, 43, 106, 59, 205, 69, 30, 104, 84, 115, 252, 213, 154, 16, 235, 86, 107, 165, 86, 125, 87, 171, 100, 92, 114, 185, 85, 117, 119, 147, 128, 31, 168, 227, 83, 203, 123, 182, 229, 205, 165, 114, 12, 231, 171, 5, 15, 199, 227, 175, 168, 225, 180, 30, 90, 122, 175, 224, 109, 166, 93, 79, 69, 82, 95, 47, 78, 64, 213, 105, 13, 104, 183, 153, 8, 239, 49, 170, 233, 125, 79, 93, 78, 154, 51, 210, 50, 108, 169, 27, 204, 101, 47, 232, 179, 174, 45, 14, 234, 165, 167, 76, 171, 213, 98, 251, 82, 8, 195, 74, 161, 64, 111, 78, 101, 146, 217, 143, 43, 248, 254, 233, 54, 140, 96, 182, 10, 27, 227, 64, 121, 70, 19, 161, 37, 249, 7, 73, 27, 215, 160, 207, 19, 172, 124, 6, 176, 71, 16, 75, 32, 92, 143, 100, 188, 175, 189, 227, 113, 249, 235, 68, 238, 151, 60, 134, 209, 189, 104, 1, 219, 157, 84, 149, 179, 50, 219, 8, 81, 188, 68, 194, 8, 98, 118, 135, 197, 212, 153, 226, 240, 162, 98, 253, 63, 125, 29, 112, 194, 162, 113, 175, 5, 162, 114, 208, 107, 177, 202, 88, 127, 196, 166, 82, 5, 61, 254, 6, 172, 248, 243, 140, 155, 93, 246, 184, 238, 132, 207, 112, 120, 79, 140, 30, 224, 112, 197, 209, 228, 90, 194, 214, 42, 229, 85, 167, 27, 12, 85, 179, 197, 131, 120, 158, 57, 251, 14, 55, 0, 244, 42, 240, 134, 91, 107, 152, 201, 46, 104, 3, 94, 191, 76, 114, 242, 42, 28, 120, 121, 161, 215, 5, 23, 57, 171, 150, 191, 224, 86, 25, 248, 120, 176, 240, 37, 151, 195, 74, 208, 146, 104, 159, 34, 147, 107, 239, 156, 198, 190, 97, 196, 132, 15, 54, 33, 75, 152, 78, 166, 227, 174, 236, 74, 67, 237, 184 },
                            UserName = "Admin"
                        });
                });

            modelBuilder.Entity("Pronto_MIA.Domain.Entities.AccessControlList", b =>
                {
                    b.HasOne("Pronto_MIA.Domain.Entities.User", "User")
                        .WithOne("AccessControlList")
                        .HasForeignKey("Pronto_MIA.Domain.Entities.AccessControlList", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Pronto_MIA.Domain.Entities.DeploymentPlan", b =>
                {
                    b.HasOne("Pronto_MIA.Domain.Entities.Department", "Department")
                        .WithMany("DeploymentPlans")
                        .HasForeignKey("DepartmentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Department");
                });

            modelBuilder.Entity("Pronto_MIA.Domain.Entities.FcmToken", b =>
                {
                    b.HasOne("Pronto_MIA.Domain.Entities.User", "Owner")
                        .WithMany("FcmTokens")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("Pronto_MIA.Domain.Entities.User", b =>
                {
                    b.HasOne("Pronto_MIA.Domain.Entities.Department", "Department")
                        .WithMany("Users")
                        .HasForeignKey("DepartmentId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Department");
                });

            modelBuilder.Entity("Pronto_MIA.Domain.Entities.Department", b =>
                {
                    b.Navigation("DeploymentPlans");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("Pronto_MIA.Domain.Entities.User", b =>
                {
                    b.Navigation("AccessControlList");

                    b.Navigation("FcmTokens");
                });
#pragma warning restore 612, 618
        }
    }
}
