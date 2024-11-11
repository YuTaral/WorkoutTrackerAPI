using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessAppAPI.Migrations
{
    /// <inheritdoc />
    public partial class addedImageNameAndDefaultColumnsToMuscleGroupsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Default",
                table: "MuscleGroups",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageName",
                table: "MuscleGroups",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "MuscleGroups",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "Default", "ImageName" },
                values: new object[] { "Y", "icon_mg_abs.png" });

            migrationBuilder.UpdateData(
                table: "MuscleGroups",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "Default", "ImageName" },
                values: new object[] { "Y", "icon_mg_back.png" });

            migrationBuilder.UpdateData(
                table: "MuscleGroups",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "Default", "ImageName" },
                values: new object[] { "Y", "icon_mg_biceps.png" });

            migrationBuilder.UpdateData(
                table: "MuscleGroups",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "Default", "ImageName" },
                values: new object[] { "Y", "icon_mg_calves.png" });

            migrationBuilder.UpdateData(
                table: "MuscleGroups",
                keyColumn: "Id",
                keyValue: 5L,
                columns: new[] { "Default", "ImageName" },
                values: new object[] { "Y", "icon_mg_chest.png" });

            migrationBuilder.UpdateData(
                table: "MuscleGroups",
                keyColumn: "Id",
                keyValue: 6L,
                columns: new[] { "Default", "ImageName" },
                values: new object[] { "Y", "icon_mg_forearms.png" });

            migrationBuilder.UpdateData(
                table: "MuscleGroups",
                keyColumn: "Id",
                keyValue: 7L,
                columns: new[] { "Default", "ImageName" },
                values: new object[] { "Y", "icon_mg_glutes.png" });

            migrationBuilder.UpdateData(
                table: "MuscleGroups",
                keyColumn: "Id",
                keyValue: 8L,
                columns: new[] { "Default", "ImageName" },
                values: new object[] { "Y", "icon_mg_hamstrings.png" });

            migrationBuilder.UpdateData(
                table: "MuscleGroups",
                keyColumn: "Id",
                keyValue: 9L,
                columns: new[] { "Default", "ImageName" },
                values: new object[] { "Y", "icon_mg_quadtriceps.png" });

            migrationBuilder.UpdateData(
                table: "MuscleGroups",
                keyColumn: "Id",
                keyValue: 10L,
                columns: new[] { "Default", "ImageName" },
                values: new object[] { "Y", "icon_mg_shoulders.png" });

            migrationBuilder.UpdateData(
                table: "MuscleGroups",
                keyColumn: "Id",
                keyValue: 11L,
                columns: new[] { "Default", "ImageName" },
                values: new object[] { "Y", "icon_mg_triceps.png" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Default",
                table: "MuscleGroups");

            migrationBuilder.DropColumn(
                name: "ImageName",
                table: "MuscleGroups");
        }
    }
}
