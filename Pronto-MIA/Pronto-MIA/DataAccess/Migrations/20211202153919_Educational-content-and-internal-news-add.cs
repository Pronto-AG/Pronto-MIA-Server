﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Pronto_MIA.DataAccess.Migrations
{
    public partial class Educationalcontentandinternalnewsadd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CanEditEducationalContent",
                table: "AccessControlLists",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanEditInternalNews",
                table: "AccessControlLists",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanViewEducationalContent",
                table: "AccessControlLists",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanViewInternalNews",
                table: "AccessControlLists",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "EducationalContent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    FileUuid = table.Column<string>(type: "text", nullable: false),
                    FileExtension = table.Column<string>(type: "text", nullable: false),
                    Published = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationalContent", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InternalNews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    AvailableFrom = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FileUuid = table.Column<string>(type: "text", nullable: false),
                    FileExtension = table.Column<string>(type: "text", nullable: false),
                    Published = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InternalNews", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AccessControlLists",
                keyColumn: "Id",
                keyValue: -1,
                columns: new[] { "CanEditEducationalContent", "CanEditInternalNews", "CanViewEducationalContent", "CanViewInternalNews" },
                values: new object[] { true, true, true, true });

            migrationBuilder.CreateIndex(
                name: "IX_EducationalContent_FileUuid",
                table: "EducationalContent",
                column: "FileUuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InternalNews_FileUuid",
                table: "InternalNews",
                column: "FileUuid",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EducationalContent");

            migrationBuilder.DropTable(
                name: "InternalNews");

            migrationBuilder.DropColumn(
                name: "CanEditEducationalContent",
                table: "AccessControlLists");

            migrationBuilder.DropColumn(
                name: "CanEditInternalNews",
                table: "AccessControlLists");

            migrationBuilder.DropColumn(
                name: "CanViewEducationalContent",
                table: "AccessControlLists");

            migrationBuilder.DropColumn(
                name: "CanViewInternalNews",
                table: "AccessControlLists");
        }
    }
}
