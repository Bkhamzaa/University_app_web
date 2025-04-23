using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace University_web_app.Migrations
{
    /// <inheritdoc />
    public partial class AddSetnullDeleteOnSubjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_Levels_LevelId",
                table: "Subjects");

            migrationBuilder.AlterColumn<Guid>(
                name: "LevelId",
                table: "Subjects",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_Levels_LevelId",
                table: "Subjects",
                column: "LevelId",
                principalTable: "Levels",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_Levels_LevelId",
                table: "Subjects");

            migrationBuilder.AlterColumn<Guid>(
                name: "LevelId",
                table: "Subjects",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_Levels_LevelId",
                table: "Subjects",
                column: "LevelId",
                principalTable: "Levels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
