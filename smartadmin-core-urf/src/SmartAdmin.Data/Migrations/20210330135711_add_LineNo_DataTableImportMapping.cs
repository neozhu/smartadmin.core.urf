using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartAdmin.Data.Migrations
{
    public partial class add_LineNo_DataTableImportMapping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LineNo",
                table: "DataTableImportMappings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LineNo",
                table: "DataTableImportMappings");
        }
    }
}
