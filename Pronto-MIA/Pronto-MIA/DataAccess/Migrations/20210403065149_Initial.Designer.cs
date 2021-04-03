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
    [DbContext(typeof(ProntoMIADbContext))]
    [Migration("20210403065149_Initial")]
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
                            HashGeneratorOptions = "{\"SaltSize\":128,\"HashIterations\":1500,\"HashSize\":512,\"Salt\":\"5SOvlnrAiZAtbOQBwnqAnde547ntrciWPfjM/jvy6QRuS6qC/FLI+J8z4CKTaxH6luNIcgQIogoNY17HyRtBVAcoy0vuyum5ezlL5bYiWCF6Au+kq/OLpqjsV1qZqJcUyrwVkv4bh8KRjOegbnhEmnhtuGoo6KFsc8Rfa8M32KE=\"}",
                            PasswordHash = new byte[] { 38, 248, 178, 12, 213, 62, 218, 234, 136, 201, 12, 132, 167, 171, 165, 4, 139, 60, 190, 206, 192, 14, 114, 21, 4, 107, 199, 92, 253, 26, 201, 107, 89, 83, 99, 242, 240, 207, 218, 90, 97, 83, 167, 234, 219, 182, 91, 161, 113, 55, 145, 213, 77, 130, 111, 76, 141, 85, 251, 115, 70, 81, 250, 188, 208, 255, 74, 242, 175, 8, 79, 89, 96, 124, 224, 153, 218, 11, 251, 170, 28, 96, 214, 142, 141, 222, 105, 13, 5, 80, 105, 116, 93, 224, 96, 156, 147, 65, 103, 37, 9, 181, 19, 62, 231, 20, 150, 201, 239, 179, 122, 17, 55, 62, 136, 241, 4, 56, 194, 205, 98, 136, 129, 167, 235, 46, 88, 160, 224, 189, 118, 102, 10, 105, 170, 51, 139, 57, 236, 218, 5, 231, 123, 254, 181, 57, 146, 117, 123, 211, 175, 154, 122, 64, 9, 200, 216, 211, 125, 177, 58, 130, 96, 109, 250, 203, 98, 19, 46, 85, 193, 119, 106, 119, 225, 81, 163, 118, 6, 211, 119, 234, 189, 221, 84, 224, 24, 108, 141, 255, 9, 209, 55, 242, 249, 32, 208, 95, 228, 202, 117, 56, 219, 221, 32, 244, 249, 211, 10, 177, 203, 184, 236, 193, 136, 222, 176, 68, 203, 126, 212, 136, 127, 173, 182, 232, 100, 37, 213, 75, 79, 203, 72, 160, 146, 121, 204, 37, 203, 134, 104, 12, 40, 0, 169, 187, 92, 82, 1, 29, 98, 45, 119, 230, 51, 218, 90, 121, 250, 229, 17, 40, 83, 178, 92, 174, 253, 118, 221, 90, 94, 221, 18, 34, 89, 106, 35, 22, 35, 193, 180, 200, 46, 46, 173, 111, 212, 104, 169, 173, 213, 212, 224, 69, 60, 181, 69, 43, 197, 44, 147, 130, 109, 111, 185, 66, 36, 44, 106, 11, 192, 186, 1, 67, 60, 214, 20, 89, 158, 192, 48, 189, 124, 27, 69, 95, 96, 46, 188, 31, 33, 122, 5, 88, 243, 134, 233, 238, 181, 4, 188, 191, 58, 230, 224, 15, 27, 81, 193, 59, 122, 105, 165, 34, 76, 164, 240, 226, 10, 232, 65, 11, 51, 206, 199, 165, 61, 203, 197, 75, 136, 128, 130, 63, 250, 151, 67, 91, 183, 45, 215, 20, 99, 0, 168, 235, 39, 252, 154, 129, 73, 216, 230, 72, 169, 196, 165, 50, 83, 21, 136, 57, 16, 130, 19, 33, 157, 172, 35, 147, 237, 155, 42, 38, 21, 238, 159, 106, 102, 76, 161, 210, 72, 17, 242, 175, 166, 112, 236, 13, 97, 0, 138, 142, 130, 226, 47, 161, 26, 167, 89, 103, 135, 187, 100, 103, 73, 131, 90, 178, 47, 219, 122, 245, 4, 206, 70, 252, 209, 73, 163, 131, 118, 17, 142, 122, 89, 201, 19, 248, 57, 19, 112, 227, 168, 207, 112, 68, 30, 135, 52, 80, 194, 154, 14, 70, 241, 110, 156, 10, 104, 177, 209, 191, 75, 191, 52, 85, 232, 125, 213, 220, 242, 182, 18, 172, 117, 232, 146, 80, 234, 207 },
                            UserName = "Franz"
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
