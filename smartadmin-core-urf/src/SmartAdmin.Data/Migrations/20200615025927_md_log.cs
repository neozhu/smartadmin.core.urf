using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartAdmin.Infrastructure.Migrations
{
    public partial class md_log : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 20, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(nullable: true),
                    LastModifiedBy = table.Column<string>(maxLength: 20, nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    MachineName = table.Column<string>(maxLength: 128, nullable: true),
                    Logged = table.Column<DateTime>(nullable: true),
                    Level = table.Column<string>(maxLength: 5, nullable: true),
                    Message = table.Column<string>(nullable: true),
                    Exception = table.Column<string>(nullable: true),
                    RequestIp = table.Column<string>(maxLength: 32, nullable: true),
                    Properties = table.Column<string>(nullable: true),
                    RequestForm = table.Column<string>(nullable: true),
                    Identity = table.Column<string>(maxLength: 128, nullable: true),
                    Logger = table.Column<string>(maxLength: 256, nullable: true),
                    Callsite = table.Column<string>(maxLength: 256, nullable: true),
                    SiteName = table.Column<string>(maxLength: 128, nullable: true),
                    Resolved = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Logs");
        }
    }
}
