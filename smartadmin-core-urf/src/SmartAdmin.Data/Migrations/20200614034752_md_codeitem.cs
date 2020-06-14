using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartAdmin.Data.Migrations
{
    public partial class md_codeitem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CodeItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 20, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(nullable: true),
                    LastModifiedBy = table.Column<string>(maxLength: 20, nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    Code = table.Column<string>(maxLength: 50, nullable: false),
                    Text = table.Column<string>(maxLength: 50, nullable: false),
                    CodeType = table.Column<string>(maxLength: 20, nullable: false),
                    Description = table.Column<string>(maxLength: 80, nullable: false),
                    IsDisabled = table.Column<int>(nullable: false),
                    Multiple = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CodeItems", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CodeItems_CodeType_Code",
                table: "CodeItems",
                columns: new[] { "CodeType", "Code" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CodeItems");
        }
    }
}
