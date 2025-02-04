using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UndeFacemRevelionul.Migrations
{
    public partial class imaginesuper : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Superstitions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Superstitions");
        }
    }
}
