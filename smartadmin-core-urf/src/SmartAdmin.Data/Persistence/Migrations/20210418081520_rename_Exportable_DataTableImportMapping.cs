using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartAdmin.Infrastructure.Persistence.Migrations
{
    public partial class rename_Exportable_DataTableImportMapping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IgnoredColumn",
                table: "DataTableImportMappings",
                newName: "Exportable");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Exportable",
                table: "DataTableImportMappings",
                newName: "IgnoredColumn");
        }
    }
}
