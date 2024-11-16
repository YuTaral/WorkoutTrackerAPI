using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessAppAPI.Migrations
{
    /// <inheritdoc />
    public partial class addedMuscleGroupIdToExercise : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exercises_MGExercises_MGExerciseId",
                table: "Exercises");

            migrationBuilder.RenameColumn(
                name: "MGExerciseId",
                table: "Exercises",
                newName: "MuscleGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_Exercises_MGExerciseId",
                table: "Exercises",
                newName: "IX_Exercises_MuscleGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exercises_MuscleGroups_MuscleGroupId",
                table: "Exercises",
                column: "MuscleGroupId",
                principalTable: "MuscleGroups",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exercises_MuscleGroups_MuscleGroupId",
                table: "Exercises");

            migrationBuilder.RenameColumn(
                name: "MuscleGroupId",
                table: "Exercises",
                newName: "MGExerciseId");

            migrationBuilder.RenameIndex(
                name: "IX_Exercises_MuscleGroupId",
                table: "Exercises",
                newName: "IX_Exercises_MGExerciseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exercises_MGExercises_MGExerciseId",
                table: "Exercises",
                column: "MGExerciseId",
                principalTable: "MGExercises",
                principalColumn: "Id");
        }
    }
}
