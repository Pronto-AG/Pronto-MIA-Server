// <auto-generated />
using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Pronto_MIA.DataAccess.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class AccessControlListadd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccessControlLists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    CanViewUsers = table.Column<bool>(type: "boolean", nullable: false),
                    CanEditUsers = table.Column<bool>(type: "boolean", nullable: false),
                    CanViewDeploymentPlans = table.Column<bool>(type: "boolean", nullable: false),
                    CanEditDeploymentPlans = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessControlLists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccessControlLists_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AccessControlLists",
                columns: new[] { "Id", "CanEditDeploymentPlans", "CanEditUsers", "CanViewDeploymentPlans", "CanViewUsers", "UserId" },
                values: new object[] { -1, true, true, true, true, -1 });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -1,
                columns: new[] { "PasswordHash", "UserName" },
                values: new object[] { new byte[] { 160, 170, 181, 57, 56, 11, 64, 111, 255, 135, 44, 16, 13, 143, 10, 57, 47, 144, 79, 128, 47, 6, 240, 159, 91, 163, 33, 239, 51, 140, 5, 59, 228, 56, 155, 57, 18, 151, 143, 191, 223, 64, 140, 19, 23, 125, 90, 34, 241, 76, 199, 118, 197, 240, 49, 56, 110, 38, 182, 112, 19, 172, 195, 113, 47, 223, 184, 44, 27, 214, 16, 242, 169, 148, 104, 135, 116, 43, 152, 103, 130, 206, 221, 110, 254, 123, 231, 102, 42, 239, 67, 130, 53, 113, 12, 150, 249, 50, 117, 225, 113, 38, 174, 80, 246, 112, 27, 104, 229, 197, 63, 76, 246, 218, 33, 63, 197, 145, 244, 65, 91, 81, 228, 81, 208, 91, 117, 249, 101, 112, 179, 10, 130, 136, 0, 168, 150, 224, 43, 124, 151, 107, 110, 10, 23, 83, 212, 80, 45, 113, 201, 148, 17, 114, 46, 169, 232, 169, 138, 135, 89, 53, 154, 213, 123, 208, 0, 155, 155, 0, 44, 249, 199, 222, 211, 120, 137, 158, 135, 20, 247, 225, 64, 119, 64, 177, 76, 43, 106, 59, 205, 69, 30, 104, 84, 115, 252, 213, 154, 16, 235, 86, 107, 165, 86, 125, 87, 171, 100, 92, 114, 185, 85, 117, 119, 147, 128, 31, 168, 227, 83, 203, 123, 182, 229, 205, 165, 114, 12, 231, 171, 5, 15, 199, 227, 175, 168, 225, 180, 30, 90, 122, 175, 224, 109, 166, 93, 79, 69, 82, 95, 47, 78, 64, 213, 105, 13, 104, 183, 153, 8, 239, 49, 170, 233, 125, 79, 93, 78, 154, 51, 210, 50, 108, 169, 27, 204, 101, 47, 232, 179, 174, 45, 14, 234, 165, 167, 76, 171, 213, 98, 251, 82, 8, 195, 74, 161, 64, 111, 78, 101, 146, 217, 143, 43, 248, 254, 233, 54, 140, 96, 182, 10, 27, 227, 64, 121, 70, 19, 161, 37, 249, 7, 73, 27, 215, 160, 207, 19, 172, 124, 6, 176, 71, 16, 75, 32, 92, 143, 100, 188, 175, 189, 227, 113, 249, 235, 68, 238, 151, 60, 134, 209, 189, 104, 1, 219, 157, 84, 149, 179, 50, 219, 8, 81, 188, 68, 194, 8, 98, 118, 135, 197, 212, 153, 226, 240, 162, 98, 253, 63, 125, 29, 112, 194, 162, 113, 175, 5, 162, 114, 208, 107, 177, 202, 88, 127, 196, 166, 82, 5, 61, 254, 6, 172, 248, 243, 140, 155, 93, 246, 184, 238, 132, 207, 112, 120, 79, 140, 30, 224, 112, 197, 209, 228, 90, 194, 214, 42, 229, 85, 167, 27, 12, 85, 179, 197, 131, 120, 158, 57, 251, 14, 55, 0, 244, 42, 240, 134, 91, 107, 152, 201, 46, 104, 3, 94, 191, 76, 114, 242, 42, 28, 120, 121, 161, 215, 5, 23, 57, 171, 150, 191, 224, 86, 25, 248, 120, 176, 240, 37, 151, 195, 74, 208, 146, 104, 159, 34, 147, 107, 239, 156, 198, 190, 97, 196, 132, 15, 54, 33, 75, 152, 78, 166, 227, 174, 236, 74, 67, 237, 184 }, "Admin" });

            migrationBuilder.CreateIndex(
                name: "IX_AccessControlLists_UserId",
                table: "AccessControlLists",
                column: "UserId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessControlLists");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -1,
                columns: new[] { "PasswordHash", "UserName" },
                values: new object[] { new byte[] { 243, 90, 163, 108, 76, 216, 221, 192, 38, 236, 53, 162, 16, 227, 92, 79, 20, 57, 166, 17, 120, 115, 27, 97, 55, 118, 115, 122, 122, 131, 204, 30, 47, 176, 56, 123, 200, 36, 145, 241, 90, 112, 194, 99, 55, 13, 123, 153, 141, 102, 244, 7, 47, 78, 101, 20, 55, 17, 113, 174, 130, 102, 51, 10, 121, 41, 114, 205, 29, 97, 33, 68, 119, 93, 238, 126, 17, 59, 201, 79, 12, 196, 85, 8, 248, 55, 251, 203, 26, 92, 108, 227, 159, 203, 246, 212, 39, 78, 25, 223, 247, 202, 54, 83, 235, 79, 200, 207, 206, 217, 64, 222, 63, 172, 59, 22, 238, 191, 87, 93, 250, 169, 5, 18, 245, 215, 140, 103, 76, 143, 4, 21, 81, 117, 20, 140, 18, 130, 129, 148, 211, 63, 14, 225, 81, 0, 244, 231, 178, 81, 200, 20, 162, 100, 2, 107, 66, 236, 173, 75, 53, 0, 75, 211, 47, 36, 69, 235, 139, 195, 95, 231, 80, 233, 218, 48, 219, 237, 56, 160, 63, 216, 171, 34, 3, 160, 250, 26, 95, 244, 213, 76, 250, 18, 192, 223, 89, 140, 252, 67, 249, 237, 76, 228, 150, 212, 50, 145, 249, 84, 254, 194, 161, 227, 46, 151, 119, 87, 158, 77, 13, 32, 79, 187, 80, 159, 54, 153, 171, 220, 208, 166, 183, 111, 151, 84, 248, 39, 70, 95, 188, 93, 131, 65, 138, 67, 139, 44, 50, 49, 37, 29, 185, 229, 210, 56, 44, 192, 211, 11, 171, 223, 193, 221, 192, 201, 138, 227, 128, 11, 213, 51, 52, 49, 87, 37, 232, 204, 228, 198, 63, 185, 74, 146, 211, 250, 175, 84, 83, 9, 178, 72, 125, 178, 99, 225, 246, 70, 35, 72, 91, 26, 40, 3, 216, 251, 181, 40, 15, 12, 208, 45, 58, 118, 77, 215, 171, 235, 87, 71, 62, 22, 30, 2, 57, 193, 194, 58, 16, 155, 165, 107, 149, 166, 219, 175, 170, 89, 225, 181, 122, 218, 104, 97, 14, 74, 59, 161, 193, 38, 94, 168, 37, 193, 229, 23, 39, 153, 116, 189, 128, 175, 49, 226, 110, 28, 30, 217, 125, 232, 63, 8, 140, 223, 34, 155, 143, 76, 78, 113, 113, 244, 100, 240, 15, 192, 49, 222, 18, 200, 32, 207, 236, 224, 17, 244, 155, 248, 78, 188, 250, 234, 93, 230, 38, 236, 85, 23, 28, 18, 223, 170, 130, 101, 82, 24, 76, 124, 91, 166, 14, 85, 93, 177, 245, 99, 100, 63, 11, 97, 190, 72, 135, 153, 58, 50, 74, 218, 166, 99, 70, 102, 185, 50, 249, 127, 0, 119, 243, 254, 92, 33, 142, 128, 29, 123, 191, 162, 236, 130, 160, 163, 71, 132, 157, 227, 221, 19, 12, 72, 138, 191, 183, 139, 146, 30, 254, 135, 31, 42, 214, 191, 14, 63, 108, 208, 207, 75, 209, 252, 197, 170, 150, 95, 96, 166, 192, 162, 203, 163, 142, 215, 203, 145, 33, 164, 254, 166, 12, 184, 124, 136 }, "Franz" });
        }
    }
}
