﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BioLabManager.Migrations
{
    /// <inheritdoc />
    public partial class AddUserToSamples : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Samples",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Samples_UserId",
                table: "Samples",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Samples_Users_UserId",
                table: "Samples",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Samples_Users_UserId",
                table: "Samples");

            migrationBuilder.DropIndex(
                name: "IX_Samples_UserId",
                table: "Samples");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Samples");
        }
    }
}
