using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessAppAPI.Migrations
{
    /// <inheritdoc />
    public partial class changedPullUps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "MuscleGroupExercises",
                keyColumn: "Id",
                keyValue: 3L,
                column: "Name",
                value: "Wide pull ups");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "MuscleGroupExercises",
                keyColumn: "Id",
                keyValue: 3L,
                column: "Name",
                value: "Pull ups");
        }
    }
}
