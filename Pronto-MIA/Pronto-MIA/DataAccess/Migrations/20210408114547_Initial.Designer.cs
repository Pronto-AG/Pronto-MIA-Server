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
    [Migration("20210408114547_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityByDefaultColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.4");

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

                    b.Property<string>("FileExtension")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FileUUID")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("FileUUID")
                        .IsUnique();

                    b.ToTable("DeploymentPlans");
                });

            modelBuilder.Entity("Pronto_MIA.Domain.Entities.FCMToken", b =>
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
                            HashGeneratorOptions = "{\"SaltSize\":128,\"HashIterations\":1500,\"HashSize\":512,\"Salt\":\"clwG6UEnsIisjIZLKkuNRbAcN625xRjYrm9hw5uH8IgqHDipcibNGYD4HK9lM3IlAqGZXZ9TXWN5nkkQipxZR40uDTZY+2rgDgkcHoa52QXZi21Zc2z+aCPSfZMGjcJx7DbOXnxrJxs+u0UYKWmo4iXnZM68ls8+1tYdhFNg8lM=\"}",
                            PasswordHash = new byte[] { 134, 88, 72, 71, 115, 209, 223, 129, 35, 4, 101, 55, 181, 78, 36, 138, 77, 244, 73, 44, 168, 110, 135, 158, 248, 30, 112, 237, 194, 13, 213, 61, 35, 27, 71, 194, 188, 238, 105, 24, 102, 78, 212, 78, 147, 220, 174, 152, 37, 247, 132, 158, 241, 213, 227, 205, 205, 12, 6, 7, 245, 199, 212, 188, 175, 101, 149, 8, 190, 133, 94, 14, 52, 26, 20, 50, 168, 114, 60, 16, 244, 127, 146, 133, 67, 36, 63, 125, 44, 131, 227, 76, 27, 124, 179, 33, 246, 123, 173, 29, 6, 165, 97, 10, 155, 59, 28, 88, 164, 236, 30, 1, 154, 129, 38, 232, 233, 220, 238, 243, 102, 144, 132, 191, 142, 217, 103, 69, 130, 56, 135, 249, 54, 250, 178, 82, 18, 134, 146, 2, 130, 230, 131, 17, 172, 112, 115, 35, 164, 155, 26, 14, 38, 12, 250, 5, 157, 74, 85, 36, 190, 85, 81, 237, 3, 149, 122, 47, 177, 3, 71, 13, 71, 237, 88, 26, 201, 226, 151, 76, 232, 148, 118, 115, 24, 63, 116, 125, 118, 255, 118, 39, 154, 56, 7, 222, 70, 86, 78, 196, 161, 111, 199, 2, 95, 170, 8, 227, 175, 62, 233, 185, 177, 223, 166, 242, 156, 179, 3, 202, 141, 151, 54, 104, 193, 68, 87, 233, 130, 12, 44, 155, 59, 31, 137, 12, 240, 175, 147, 229, 66, 128, 177, 168, 55, 244, 103, 207, 146, 254, 253, 58, 16, 203, 87, 172, 8, 46, 37, 206, 229, 196, 163, 227, 41, 227, 152, 115, 84, 77, 11, 78, 255, 54, 119, 19, 52, 84, 244, 33, 71, 87, 145, 11, 77, 76, 189, 46, 223, 143, 33, 198, 128, 111, 147, 207, 110, 50, 70, 82, 216, 255, 0, 217, 215, 57, 99, 223, 72, 144, 6, 105, 246, 199, 88, 102, 104, 99, 129, 2, 156, 247, 53, 65, 30, 184, 2, 185, 209, 88, 82, 205, 81, 84, 14, 101, 26, 147, 163, 105, 124, 98, 200, 126, 98, 187, 204, 5, 67, 40, 125, 246, 94, 253, 63, 112, 2, 227, 59, 194, 2, 164, 11, 206, 92, 67, 215, 201, 254, 210, 103, 194, 40, 195, 178, 36, 162, 97, 236, 21, 74, 204, 58, 206, 159, 158, 25, 167, 113, 240, 83, 245, 203, 12, 141, 97, 222, 66, 105, 22, 130, 56, 187, 92, 127, 41, 16, 142, 77, 114, 230, 137, 102, 9, 59, 22, 225, 179, 187, 235, 211, 197, 120, 0, 69, 9, 165, 180, 251, 216, 180, 110, 49, 234, 168, 165, 195, 207, 82, 23, 143, 52, 238, 5, 126, 237, 142, 134, 58, 209, 214, 94, 101, 116, 233, 117, 228, 126, 228, 244, 75, 223, 31, 194, 70, 245, 100, 32, 39, 169, 122, 209, 121, 199, 85, 255, 164, 248, 76, 26, 66, 108, 9, 163, 187, 58, 128, 211, 42, 33, 20, 251, 112, 165, 134, 131, 109, 28, 164, 133, 134, 178, 208, 89, 105, 87, 244, 50, 198, 147, 126, 59 },
                            UserName = "Franz"
                        });
                });

            modelBuilder.Entity("Pronto_MIA.Domain.Entities.FCMToken", b =>
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
