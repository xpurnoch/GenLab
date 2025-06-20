using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BioLabManager.Migrations
{
    /// <inheritdoc />
    public partial class AddLabsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Samples_Lab_LabId",
                table: "Samples");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Lab_LabId",
                table: "User");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Lab",
                table: "Lab");

            migrationBuilder.RenameTable(
                name: "Lab",
                newName: "Labs");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Labs",
                table: "Labs",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Samples_Labs_LabId",
                table: "Samples",
                column: "LabId",
                principalTable: "Labs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_User_Labs_LabId",
                table: "User",
                column: "LabId",
                principalTable: "Labs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Samples_Labs_LabId",
                table: "Samples");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Labs_LabId",
                table: "User");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Labs",
                table: "Labs");

            migrationBuilder.RenameTable(
                name: "Labs",
                newName: "Lab");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Lab",
                table: "Lab",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Samples_Lab_LabId",
                table: "Samples",
                column: "LabId",
                principalTable: "Lab",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_User_Lab_LabId",
                table: "User",
                column: "LabId",
                principalTable: "Lab",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
