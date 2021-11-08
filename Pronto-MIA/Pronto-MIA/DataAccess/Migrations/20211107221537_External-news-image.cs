﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace Pronto_MIA.DataAccess.Migrations
{
    public partial class Externalnewsimage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileExtension",
                table: "ExternalNews",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FileUuid",
                table: "ExternalNews",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalNews_FileUuid",
                table: "ExternalNews",
                column: "FileUuid",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ExternalNews_FileUuid",
                table: "ExternalNews");

            migrationBuilder.DropColumn(
                name: "FileExtension",
                table: "ExternalNews");

            migrationBuilder.DropColumn(
                name: "FileUuid",
                table: "ExternalNews");
        }
    }
}
