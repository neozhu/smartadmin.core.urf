using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartAdmin.Infrastructure.Migrations
{
    public partial class md_addpublisher : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Publisher",
                table: "Notifications",
                maxLength: 128,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Publisher",
                table: "Notifications");
        }
    }
}
