using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KTGK.Migrations
{
    /// <inheritdoc />
    public partial class AddTimeTakenToResult : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TimeTaken",
                table: "Results",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeTaken",
                table: "Results");
        }
    }
}
