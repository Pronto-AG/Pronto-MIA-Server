using Microsoft.EntityFrameworkCore.Migrations;

namespace Pronto_MIA.DataAccess.Migrations
{
    public partial class AdditionalAccessControls : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CanEditDepartmentDeploymentPlans",
                table: "AccessControlLists",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanEditDepartmentUsers",
                table: "AccessControlLists",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanEditOwnDepartment",
                table: "AccessControlLists",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanViewDepartmentDeploymentPlans",
                table: "AccessControlLists",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanViewDepartmentUsers",
                table: "AccessControlLists",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanViewOwnDepartment",
                table: "AccessControlLists",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanEditDepartmentDeploymentPlans",
                table: "AccessControlLists");

            migrationBuilder.DropColumn(
                name: "CanEditDepartmentUsers",
                table: "AccessControlLists");

            migrationBuilder.DropColumn(
                name: "CanEditOwnDepartment",
                table: "AccessControlLists");

            migrationBuilder.DropColumn(
                name: "CanViewDepartmentDeploymentPlans",
                table: "AccessControlLists");

            migrationBuilder.DropColumn(
                name: "CanViewDepartmentUsers",
                table: "AccessControlLists");

            migrationBuilder.DropColumn(
                name: "CanViewOwnDepartment",
                table: "AccessControlLists");
        }
    }
}
