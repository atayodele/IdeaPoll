using Microsoft.EntityFrameworkCore.Migrations;

namespace IdeaSolution.Data.Migrations
{
    public partial class deleteFullnameEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Fullname",
                table: "AspNetUsers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Fullname",
                table: "AspNetUsers",
                nullable: true);
        }
    }
}
