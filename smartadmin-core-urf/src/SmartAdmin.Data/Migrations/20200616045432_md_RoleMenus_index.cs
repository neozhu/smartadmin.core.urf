using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartAdmin.Infrastructure.Migrations
{
    public partial class md_RoleMenus_index : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_RoleMenus_RoleName_MenuId",
                table: "RoleMenus",
                columns: new[] { "RoleName", "MenuId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RoleMenus_RoleName_MenuId",
                table: "RoleMenus");
        }
    }
}
