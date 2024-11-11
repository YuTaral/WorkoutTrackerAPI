using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessAppAPI.Migrations
{
    /// <inheritdoc />
    public partial class removedPngFromImageNameColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "MuscleGroups",
                keyColumn: "Id",
                keyValue: 1L,
                column: "ImageName",
                value: "icon_mg_abs");

            migrationBuilder.UpdateData(
                table: "MuscleGroups",
                keyColumn: "Id",
                keyValue: 2L,
                column: "ImageName",
                value: "icon_mg_back");

            migrationBuilder.UpdateData(
                table: "MuscleGroups",
                keyColumn: "Id",
                keyValue: 3L,
                column: "ImageName",
                value: "icon_mg_biceps");

            migrationBuilder.UpdateData(
                table: "MuscleGroups",
                keyColumn: "Id",
                keyValue: 4L,
                column: "ImageName",
                value: "icon_mg_calves");

            migrationBuilder.UpdateData(
                table: "MuscleGroups",
                keyColumn: "Id",
                keyValue: 5L,
                column: "ImageName",
                value: "icon_mg_chest");

            migrationBuilder.UpdateData(
                table: "MuscleGroups",
                keyColumn: "Id",
                keyValue: 6L,
                column: "ImageName",
                value: "icon_mg_forearms");

            migrationBuilder.UpdateData(
                table: "MuscleGroups",
                keyColumn: "Id",
                keyValue: 7L,
                column: "ImageName",
                value: "icon_mg_glutes");

            migrationBuilder.UpdateData(
                table: "MuscleGroups",
                keyColumn: "Id",
                keyValue: 8L,
                column: "ImageName",
                value: "icon_mg_hamstrings");

            migrationBuilder.UpdateData(
                table: "MuscleGroups",
                keyColumn: "Id",
                keyValue: 9L,
                column: "ImageName",
                value: "icon_mg_quadtriceps");

            migrationBuilder.UpdateData(
                table: "MuscleGroups",
                keyColumn: "Id",
                keyValue: 10L,
                column: "ImageName",
                value: "icon_mg_shoulders");

            migrationBuilder.UpdateData(
                table: "MuscleGroups",
                keyColumn: "Id",
                keyValue: 11L,
                column: "ImageName",
                value: "icon_mg_triceps");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "MuscleGroups",
                keyColumn: "Id",
                keyValue: 1L,
                column: "ImageName",
                value: "icon_mg_abs.png");

            migrationBuilder.UpdateData(
                table: "MuscleGroups",
                keyColumn: "Id",
                keyValue: 2L,
                column: "ImageName",
                value: "icon_mg_back.png");

            migrationBuilder.UpdateData(
                table: "MuscleGroups",
                keyColumn: "Id",
                keyValue: 3L,
                column: "ImageName",
                value: "icon_mg_biceps.png");

            migrationBuilder.UpdateData(
                table: "MuscleGroups",
                keyColumn: "Id",
                keyValue: 4L,
                column: "ImageName",
                value: "icon_mg_calves.png");

            migrationBuilder.UpdateData(
                table: "MuscleGroups",
                keyColumn: "Id",
                keyValue: 5L,
                column: "ImageName",
                value: "icon_mg_chest.png");

            migrationBuilder.UpdateData(
                table: "MuscleGroups",
                keyColumn: "Id",
                keyValue: 6L,
                column: "ImageName",
                value: "icon_mg_forearms.png");

            migrationBuilder.UpdateData(
                table: "MuscleGroups",
                keyColumn: "Id",
                keyValue: 7L,
                column: "ImageName",
                value: "icon_mg_glutes.png");

            migrationBuilder.UpdateData(
                table: "MuscleGroups",
                keyColumn: "Id",
                keyValue: 8L,
                column: "ImageName",
                value: "icon_mg_hamstrings.png");

            migrationBuilder.UpdateData(
                table: "MuscleGroups",
                keyColumn: "Id",
                keyValue: 9L,
                column: "ImageName",
                value: "icon_mg_quadtriceps.png");

            migrationBuilder.UpdateData(
                table: "MuscleGroups",
                keyColumn: "Id",
                keyValue: 10L,
                column: "ImageName",
                value: "icon_mg_shoulders.png");

            migrationBuilder.UpdateData(
                table: "MuscleGroups",
                keyColumn: "Id",
                keyValue: 11L,
                column: "ImageName",
                value: "icon_mg_triceps.png");
        }
    }
}
