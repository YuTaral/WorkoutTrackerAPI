﻿using FitnessAppAPI.Data.Models;
using FitnessAppAPI.Data.Services.Exercises.Models;
using FitnessAppAPI.Data.Services.MuscleGroups.Models;
using FitnessAppAPI.Data;
using FitnessAppAPI.Data.Services.Workouts.Models;
using FitnessAppAPI.Data.Services.User.Models;

namespace FitnessAppAPI.Common
{
    /// <summary>
    ///     Class to hold the models mappings
    /// </summary>
    public static class ModelMapper
    {
        /// <summary>
        ///     Map the Workout to WorkoutModel
        /// </summary>
        public static WorkoutModel MapToWorkoutModel(Workout workout, FitnessAppAPIContext DBAccess)
        {

            if (workout == null)
            {
                return GetEmptyWorkoutModel();
            }

            return new WorkoutModel
            {
                Id = workout.Id,
                Name = workout.Name,
                Date = workout.Date,
                Template = workout.Template == "Y",
                Exercises = DBAccess.Exercises.Where(e => e.WorkoutId == workout.Id)
                                              .Select(e => MapToExerciseModel(e, DBAccess))
                                              .ToList()
            };
        }

        /// <summary>
        ///     Map the Exercise to ExerciseModel
        /// </summary>
        public static ExerciseModel MapToExerciseModel(Exercise exercise, FitnessAppAPIContext DBAccess)
        {
            if (exercise == null)
            {
                return GetEmptyExerciseModel();
            }

            // Get the MuscleGroups results and convert them to Enumerable, to avoid errors that the Entity Framerwork
            // cannot translate the method into SQL when MapToMuscleGroupModel() is called
            var muscleGroupEnum = DBAccess.MuscleGroups.Where(mg => mg.Id == exercise.MuscleGroupId).AsEnumerable();
            var muscleGroup = muscleGroupEnum.Select(mg => MapToMuscleGroupModel(mg)).FirstOrDefault(GetEmptyMuscleGroupModel());

            // Get the Sets results and convert them to Enumerable, to avoid errors that the Entity Framerwork
            // cannot translate the method into SQL when MapToSetModel() is called
            var setsEnumerable = DBAccess.Sets.Where(s => s.ExerciseId == exercise.Id).AsEnumerable();
            var sets = setsEnumerable.Select(s => MapToSetModel(s)).ToList();

            return new ExerciseModel
            {
                Id = exercise.Id,
                Name = exercise.Name,
                MuscleGroup = muscleGroup,
                Sets = sets
            };
        }

        /// <summary>
        ///     Map the MGExercise to ExerciseModel
        /// </summary>
        public static ExerciseModel MapToExerciseModel(MGExercise MGExercise, FitnessAppAPIContext DBAccess)
        {
            if (MGExercise == null)
            {
                return GetEmptyExerciseModel();
            }

            // Get the MuscleGroups results and convert them to Enumerable, to avoid errors that the Entity Framerwork
            // cannot translate the method into SQL when MapToMuscleGroupModel() is called
            var muscleGroupEnum = DBAccess.MuscleGroups.Where(mg => mg.Id == MGExercise.MuscleGroupId).AsEnumerable();
            var muscleGroup = muscleGroupEnum.Select(mg => MapToMuscleGroupModel(mg)).FirstOrDefault(GetEmptyMuscleGroupModel());

            // Get the Sets results and convert them to Enumerable, to avoid errors that the Entity Framerwork
            // cannot translate the method into SQL when MapToSetModel() is called
            var setEnum = DBAccess.Sets.Where(s => s.ExerciseId == MGExercise.Id).AsEnumerable();
            var sets = setEnum.Select(s => MapToSetModel(s)).ToList();

            return new ExerciseModel
            {
                Id = MGExercise.Id,
                Name = MGExercise.Name,
                MuscleGroup = muscleGroup,
                Sets = sets
            };
        }

        /// <summary>
        ///     Map the MGExercises to MGExerciseModel
        /// </summary>
        public static MGExerciseModel MapToMGExerciseModel(MGExercise MGExercise)
        {
            if (MGExercise == null)
            {
                return GetEmptyMGExerciseModel();
            }

            return new MGExerciseModel
            {
                Id = MGExercise.Id,
                Name = MGExercise.Name,
                Description = MGExercise.Description,
                MuscleGroupId = MGExercise.MuscleGroupId
            };
        }

        /// <summary>
        ///     Map the MuscleGroup to MuscleGroupModel
        /// </summary>
        public static MuscleGroupModel MapToMuscleGroupModel(MuscleGroup muscleGroup)
        {
            if (muscleGroup == null)
            {
                return GetEmptyMuscleGroupModel();
            }

            return new MuscleGroupModel
            {
                Id = muscleGroup.Id,
                Name = muscleGroup.Name,
                ImageName = muscleGroup.ImageName
            };
        }

        /// <summary>
        ///     Map the MuscleGroup to MuscleGroupModel
        /// </summary>
        public static SetModel MapToSetModel(Set set)
        {
            if (set == null)
            {
                return GetEmptySetModel();
            }

            return new SetModel
            {
                Id = set.Id,
                Reps = set.Reps,
                Weight = set.Weight,
                Completed = set.Completed
            };
        }

        /// <summary>
        ///     Map the User and UserDefaultValue to UserModel
        /// </summary>
        public static UserModel MapToUserModel(User user, UserDefaultValue? defaultValues, string weightUnit)
        {
            var defaultValuesModel = GetEmptyUserDefaultValuesModel();

            if (defaultValues != null) {
                defaultValuesModel.Id = defaultValues.Id;
                defaultValuesModel.Sets = defaultValues.Sets;
                defaultValuesModel.Reps = defaultValues.Reps;
                defaultValuesModel.Weight = defaultValues.Weight;
                defaultValuesModel.WeightUnitText = weightUnit;
            }

            return new UserModel
            {
                Id = user.Id,
                Email = user.Email,
                DefaultValues = defaultValuesModel
            };
        }

        /// <summary>
        ///     Map the UserDefaultValue to UserDefaultValuesModel
        /// </summary>
        public static UserDefaultValuesModel MapToUserDefaultValuesModel(UserDefaultValue? defaultValues, FitnessAppAPIContext DBAccess)
        {
            if (defaultValues == null)
            {
                return GetEmptyUserDefaultValuesModel();
            }

            var unitText = DBAccess.WeightUnits.Where(w => w.Code == defaultValues.WeightUnitCode)
                                               .Select(w => w.Text).FirstOrDefault();

            if (unitText == null)
            {
                // Must not happen
                unitText = "";
            }

            return new UserDefaultValuesModel
            {
                Id = defaultValues.Id,
                Sets = defaultValues.Sets,
                Reps = defaultValues.Reps,
                Weight = defaultValues.Weight,
                WeightUnitText = unitText
            };
        }

        /// <summary>
        ///    Return empty MuscleGroupModel
        /// </summary>
        public static ExerciseModel GetEmptyExerciseModel()
        {
            return new ExerciseModel
            {
                Id = 0,
                Name = "Unknown",
                MuscleGroup = GetEmptyMuscleGroupModel(),
                Sets = { }
            };
        }

        /// <summary>
        ///    Return empty MuscleGroupModel
        /// </summary>
        private static MGExerciseModel GetEmptyMGExerciseModel()
        {
            return new MGExerciseModel
            {
                Id = 0,
                Name = "Unknown",
                Description = "Unknown",
                MuscleGroupId = 0
            };
        }

        /// <summary>
        ///    Return empty MuscleGroupModel
        /// </summary>
        private static MuscleGroupModel GetEmptyMuscleGroupModel()
        {
            return new MuscleGroupModel
            {
                Id = 0,
                Name = "Unknown",
                ImageName = ""
            };
        }

        /// <summary>
        ///    Return empty SetModel
        /// </summary>
        private static SetModel GetEmptySetModel()
        {
            return new SetModel
            {
                Id = 0,
                Reps = 0,
                Weight = 0,
                Completed = false
            };
        }

        /// <summary>
        ///    Return empty WorkoutModel
        /// </summary>
        private static WorkoutModel GetEmptyWorkoutModel()
        {
            return new WorkoutModel {
                Id = 0,
                Name = "Unknown",
                Date = DateTime.UtcNow,
                Template = false,
                Exercises = { }
            };
        }

        /// <summary>
        ///    Return empty WorkoutModel
        /// </summary>
        private static UserDefaultValuesModel GetEmptyUserDefaultValuesModel()
        {
            return new UserDefaultValuesModel
            {
                Id = 0,
                Sets = 0,
                Reps = 0,
                Weight = 0,
                WeightUnitText = ""
            };
        }
    } 
}
