﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Pronto_MIA.DataAccess;

namespace Pronto_MIA.DataAccess.Migrations
{
    [DbContext(typeof(ProntoMiaDbContext))]
    partial class ProntoMiaDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityByDefaultColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.5");

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

                    b.HasIndex("UserName")
                        .IsUnique();

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = -1,
                            HashGenerator = "Pbkdf2Generator",
                            HashGeneratorOptions = "{\"SaltSize\":128,\"HashIterations\":1500,\"HashSize\":512,\"Salt\":\"A+16bv/SvaC7ZJgS7u+CB8nN32PBUAbJuT09NigsCzQx6/CxS1I/5laUaFoJNZ3QhTm4TqFnWYzokdrvrUxbOEN0MN3ZhINcblSLF9LwbZeiT0nYOnQgTBEPL0KoszXdm8x2mYXHAJFYQ9KOsIZregzuiBQfSqsFfR2uDnFHm9o=\"}",
                            PasswordHash = new byte[] { 243, 90, 163, 108, 76, 216, 221, 192, 38, 236, 53, 162, 16, 227, 92, 79, 20, 57, 166, 17, 120, 115, 27, 97, 55, 118, 115, 122, 122, 131, 204, 30, 47, 176, 56, 123, 200, 36, 145, 241, 90, 112, 194, 99, 55, 13, 123, 153, 141, 102, 244, 7, 47, 78, 101, 20, 55, 17, 113, 174, 130, 102, 51, 10, 121, 41, 114, 205, 29, 97, 33, 68, 119, 93, 238, 126, 17, 59, 201, 79, 12, 196, 85, 8, 248, 55, 251, 203, 26, 92, 108, 227, 159, 203, 246, 212, 39, 78, 25, 223, 247, 202, 54, 83, 235, 79, 200, 207, 206, 217, 64, 222, 63, 172, 59, 22, 238, 191, 87, 93, 250, 169, 5, 18, 245, 215, 140, 103, 76, 143, 4, 21, 81, 117, 20, 140, 18, 130, 129, 148, 211, 63, 14, 225, 81, 0, 244, 231, 178, 81, 200, 20, 162, 100, 2, 107, 66, 236, 173, 75, 53, 0, 75, 211, 47, 36, 69, 235, 139, 195, 95, 231, 80, 233, 218, 48, 219, 237, 56, 160, 63, 216, 171, 34, 3, 160, 250, 26, 95, 244, 213, 76, 250, 18, 192, 223, 89, 140, 252, 67, 249, 237, 76, 228, 150, 212, 50, 145, 249, 84, 254, 194, 161, 227, 46, 151, 119, 87, 158, 77, 13, 32, 79, 187, 80, 159, 54, 153, 171, 220, 208, 166, 183, 111, 151, 84, 248, 39, 70, 95, 188, 93, 131, 65, 138, 67, 139, 44, 50, 49, 37, 29, 185, 229, 210, 56, 44, 192, 211, 11, 171, 223, 193, 221, 192, 201, 138, 227, 128, 11, 213, 51, 52, 49, 87, 37, 232, 204, 228, 198, 63, 185, 74, 146, 211, 250, 175, 84, 83, 9, 178, 72, 125, 178, 99, 225, 246, 70, 35, 72, 91, 26, 40, 3, 216, 251, 181, 40, 15, 12, 208, 45, 58, 118, 77, 215, 171, 235, 87, 71, 62, 22, 30, 2, 57, 193, 194, 58, 16, 155, 165, 107, 149, 166, 219, 175, 170, 89, 225, 181, 122, 218, 104, 97, 14, 74, 59, 161, 193, 38, 94, 168, 37, 193, 229, 23, 39, 153, 116, 189, 128, 175, 49, 226, 110, 28, 30, 217, 125, 232, 63, 8, 140, 223, 34, 155, 143, 76, 78, 113, 113, 244, 100, 240, 15, 192, 49, 222, 18, 200, 32, 207, 236, 224, 17, 244, 155, 248, 78, 188, 250, 234, 93, 230, 38, 236, 85, 23, 28, 18, 223, 170, 130, 101, 82, 24, 76, 124, 91, 166, 14, 85, 93, 177, 245, 99, 100, 63, 11, 97, 190, 72, 135, 153, 58, 50, 74, 218, 166, 99, 70, 102, 185, 50, 249, 127, 0, 119, 243, 254, 92, 33, 142, 128, 29, 123, 191, 162, 236, 130, 160, 163, 71, 132, 157, 227, 221, 19, 12, 72, 138, 191, 183, 139, 146, 30, 254, 135, 31, 42, 214, 191, 14, 63, 108, 208, 207, 75, 209, 252, 197, 170, 150, 95, 96, 166, 192, 162, 203, 163, 142, 215, 203, 145, 33, 164, 254, 166, 12, 184, 124, 136 },
                            UserName = "Franz"
                        });
                });

            modelBuilder.Entity("Pronto_MIA.Domain.Entities.FcmToken", b =>
                {
                    b.HasOne("Pronto_MIA.Domain.Entities.User", "Owner")
                        .WithMany("FCMTokens")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("Pronto_MIA.Domain.Entities.User", b =>
                {
                    b.Navigation("FCMTokens");
                });
#pragma warning restore 612, 618
        }
    }
}
