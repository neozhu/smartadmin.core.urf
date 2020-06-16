using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartAdmin.Data.Migrations
{
    public partial class md_RoleMenus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RoleMenus",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 20, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(nullable: true),
                    LastModifiedBy = table.Column<string>(maxLength: 20, nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    RoleName = table.Column<string>(maxLength: 128, nullable: false),
                    MenuId = table.Column<int>(nullable: false),
                    IsEnabled = table.Column<bool>(nullable: false),
                    Create = table.Column<bool>(nullable: false),
                    Edit = table.Column<bool>(nullable: false),
                    Delete = table.Column<bool>(nullable: false),
                    Import = table.Column<bool>(nullable: false),
                    Export = table.Column<bool>(nullable: false),
                    FunctionPoint1 = table.Column<bool>(nullable: false),
                    FunctionPoint2 = table.Column<bool>(nullable: false),
                    FunctionPoint3 = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleMenus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleMenus_MenuItems_MenuId",
                        column: x => x.MenuId,
                        principalTable: "MenuItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoleMenus_MenuId",
                table: "RoleMenus",
                column: "MenuId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoleMenus");
        }
    }
}
