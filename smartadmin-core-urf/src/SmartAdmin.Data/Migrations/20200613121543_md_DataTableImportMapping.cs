using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartAdmin.Data.Migrations
{
    public partial class md_DataTableImportMapping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DataTableImportMappings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 20, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(nullable: true),
                    LastModifiedBy = table.Column<string>(maxLength: 20, nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    EntitySetName = table.Column<string>(maxLength: 50, nullable: false),
                    FieldName = table.Column<string>(maxLength: 50, nullable: false),
                    DefaultValue = table.Column<string>(maxLength: 50, nullable: true),
                    TypeName = table.Column<string>(maxLength: 30, nullable: true),
                    IsRequired = table.Column<bool>(nullable: false),
                    SourceFieldName = table.Column<string>(maxLength: 50, nullable: true),
                    IsEnabled = table.Column<bool>(nullable: false),
                    IgnoredColumn = table.Column<bool>(nullable: false),
                    RegularExpression = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataTableImportMappings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DataTableImportMappings_EntitySetName_FieldName",
                table: "DataTableImportMappings",
                columns: new[] { "EntitySetName", "FieldName" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataTableImportMappings");
        }
    }
}
