using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bugtracker.Migrations
{
    public partial class bug : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bug",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Bug_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Bug_Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Bug_CreatedDateT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Bug_ModifiedDateT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Bug_Status = table.Column<int>(type: "int", nullable: false),
                    Bug_CreatedByID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Bug_ModifiedByID = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bug", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bug",
                schema: "Identity");
        }
    }
}
