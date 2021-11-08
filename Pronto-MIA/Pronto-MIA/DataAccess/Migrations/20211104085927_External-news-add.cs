﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Pronto_MIA.DataAccess.Migrations
{
    public partial class Externalnewsadd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CanEditExternalNews",
                table: "AccessControlLists",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanViewExternalNews",
                table: "AccessControlLists",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ExternalNews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    AvailableFrom = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Published = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalNews", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AccessControlLists",
                keyColumn: "Id",
                keyValue: -1,
                columns: new[] { "CanEditExternalNews", "CanViewExternalNews" },
                values: new object[] { true, true });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExternalNews");

            migrationBuilder.DropColumn(
                name: "CanEditExternalNews",
                table: "AccessControlLists");

            migrationBuilder.DropColumn(
                name: "CanViewExternalNews",
                table: "AccessControlLists");
        }
    }
}
