using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Models;
using FitnessAppAPI.Data.Services.Exercises.Models;

namespace FitnessAppAPI.Data.Services.Exercises
{
    /// <summary>
    ///     Exercise service class to implement IWorkoutService interface.
    /// </summary>
    public class ExerciseService(FitnessAppAPIContext DB) : IExerciseService
    {
        private readonly FitnessAppAPIContext DBAccess = DB;

        /// <summary>
        ///     Adds the exercise to the workout with the provided id
        /// </summary>
        /// <param name="exerciseData">
        ///     The exercise
        /// </param>
        /// <param name="workoutId">
        ///     The workout id
        /// </param>
        public bool AddExerciseToWorkout(ExerciseModel exerciseData, long workoutId)
        {
            var exercise = new Exercise
            {
                Name = exerciseData.Name,
                WorkoutId = workoutId,
                MuscleGroupId = exerciseData.MuscleGroup.Id,
            };

            DBAccess.Exercises.Add(exercise);
            DBAccess.SaveChanges();

            // Check if we need to add sets
            if (exerciseData.Sets != null && exerciseData.Sets.Count > 0)
            {
                foreach (SetModel s in exerciseData.Sets)
                {
                    var set = new Set
                    {
                        Reps = s.Reps,
                        Weight = s.Weight,
                        Completed = s.Completed,
                        ExerciseId = exercise.Id
                    };

                    DBAccess.Sets.Add(set);
                    DBAccess.SaveChanges();
                }
            }

            return true;
        }

        /// <summary>
        ///     Updates the provided exercise
        /// </summary>
        /// <param name="exercise">
        ///     The exercise
        /// </param>
        /// <param name="workoutId">
        ///     The workout id
        /// </param>
        public bool UpdateExerciseFromWorkout(ExerciseModel exerciseData, long workoutId)
        {
            // Fetch the exact exercise and it's sets
            var exercise = DBAccess.Exercises.Find(exerciseData.Id);
            var sets = DBAccess.Sets.Where(s => s.ExerciseId == exerciseData.Id).ToList();

            if (exercise == null)
            {
                return false;
            }

            if (!exercise.Name.Equals(exerciseData.Name))
            {
                // Update the exercise name if it has been changed
                exercise.Name = exerciseData.Name;
            }

            if (exerciseData.Sets != null && exerciseData.Sets.Count > 0)
            {
                // Update / add the sets
                foreach (SetModel s in exerciseData.Sets)
                {
                    if (s.Id == 0)
                    {
                        // Add new set
                        var set = new Set
                        {
                            Reps = s.Reps,
                            Weight = s.Weight,
                            Completed = s.Completed,
                            ExerciseId = exerciseData.Id
                        };

                        DBAccess.Sets.Add(set);

                    }
                    else
                    {
                        // Update the set
                        var set = DBAccess.Sets.Find(s.Id);

                        if (set != null)
                        {
                            set.Completed = s.Completed;
                            set.Reps = s.Reps;
                            set.Weight = s.Weight;
                        }
                    }
                }

                // Check if we need to remove any sets
                foreach (Set set in sets)
                {
                    if (!exerciseData.Sets.Any(x => x.Id == set.Id))
                    {
                        DBAccess.Sets.Remove(set);
                    }
                }

            }
            else
            {
                // Delete all sets for this exercise
                var setsToRemove = DBAccess.Sets.Where(s => s.ExerciseId == exercise.Id).ToList();

                foreach (Set set in setsToRemove)
                {
                    DBAccess.Sets.Remove(set);
                }

            }

            // Save all changes
            DBAccess.SaveChanges();
            return true;
        }

        /// <summary>
        ///     Deletes the exercise with the provided id
        /// </summary>
        /// <param name="exerciseId">
        ///     The exercise id
        /// </param>
        public long DeleteExerciseFromWorkout(long exerciseId)
        {
            var exercise = DBAccess.Exercises.Where(e => e.Id == exerciseId).FirstOrDefault();
            if (exercise == null)
            {
                return 0;
            }

            DBAccess.Exercises.Remove(exercise);
            DBAccess.SaveChanges();
            return exercise.WorkoutId;
        }

        /// <summary>
        ///     Adds the exercise to specific muscle group
        /// </summary>
        /// <param name="exerciseData">
        ///     The exercise
        /// </param>
        /// <param name="userId">
        ///     The user id who added the exercise
        /// </param>
        public ExerciseModel? AddExercise(MGExerciseModel exerciseData, string userId)
        {
            var exercise = new MGExercise
            {
                Name = exerciseData.Name,
                Description = exerciseData.Description,
                MuscleGroupId = exerciseData.MuscleGroupId,
                UserId = userId
            };

            DBAccess.MGExercises.Add(exercise);
            DBAccess.SaveChanges();

            // Get the MGExercises results and convert them to Enumerable, to avoid errors that the Entity Framerwork
            // cannot translate the method into SQL when MapToExerciseModel() is called
            var MGExercisesEnum = DBAccess.MGExercises.Where(e => e.Id == exercise.Id).AsEnumerable();
            var model = MGExercisesEnum.Select(e => ModelMapper.MapToExerciseModel(e, DBAccess))
                                        .FirstOrDefault(ModelMapper.GetEmptyExerciseModel());
            return model;
        }

        /// <summary>
        ///     Deletes the muscle group exercise with the provided id
        /// </summary>
        /// <param name="MGExerciseId">
        ///     The exercise id
        /// </param>
        public long DeleteExercise(long MGExerciseId)
        {
            var MGExercise = DBAccess.MGExercises.Where(e => e.Id == MGExerciseId).FirstOrDefault();

            if (MGExercise == null)
            {
                return 0;
            }

            if (MGExercise.UserId == null) {
                return -1;
            }

            DBAccess.MGExercises.Remove(MGExercise);
            DBAccess.SaveChanges();

            return MGExercise.MuscleGroupId;
        }


        /// <summary>
        ///     Fetches the exercises for the muscle group
        /// </summary>
        /// <param name="muscleGroupId">
        ///     The muscle group id
        /// </param>
        /// <param name="userId">
        ///     The the logged in user id
        /// </param>
        /// <param name="onlyForUser">
        ///     If "Y" the method will return only the user defined exercises for this muscle group,
        ///     which are considered editable and can be deleted / updated
        ///     If "N" the method will return all default and user defined exercises for this muscle group
        /// </param>
        public List<MGExerciseModel> GetExercisesForMG(long muscleGroupId, string userId, string onlyForUser) {
            if (onlyForUser == "Y")
            {
                return DBAccess.MGExercises.Where(e => e.MuscleGroupId == muscleGroupId && e.UserId == userId)
                                           .Select(e => ModelMapper.MapToMGExerciseModel(e))
                                           .ToList();
            }
            else {
                return DBAccess.MGExercises.Where(e => e.MuscleGroupId == muscleGroupId && (e.UserId == null || e.UserId == userId))
                                            .Select(e => ModelMapper.MapToMGExerciseModel(e))
                                            .ToList();
            }
        }
    }
}
