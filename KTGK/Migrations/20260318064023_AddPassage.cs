using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KTGK.Migrations
{
    /// <inheritdoc />
    public partial class AddPassage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PassageId",
                table: "Questions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Passages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Passages", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Questions_PassageId",
                table: "Questions",
                column: "PassageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Passages_PassageId",
                table: "Questions",
                column: "PassageId",
                principalTable: "Passages",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Passages_PassageId",
                table: "Questions");

            migrationBuilder.DropTable(
                name: "Passages");

            migrationBuilder.DropIndex(
                name: "IX_Questions_PassageId",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "PassageId",
                table: "Questions");
        }
    }
}
