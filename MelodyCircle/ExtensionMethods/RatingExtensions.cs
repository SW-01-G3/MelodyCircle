using MelodyCircle.Models;

namespace MelodyCircle.ExtensionMethods
{
    public static class RatingExtensions
    {
        public static double CalculateAverageRating(this List<UserRating> ratings)
        {
            if (ratings == null || ratings.Count < 1)
            {
                return 0; // No ratings
            }

            int totalRating = 0;
            foreach (var rating in ratings)
            {
                totalRating += rating.Value;
            }

            return (double)totalRating / ratings.Count;
        }

        public static double CalculateAverageRating(this List<TutorialRating> ratings)
        {
            if (ratings == null || ratings.Count < 1)
            {
                return 0; // No ratings
            }

            int totalRating = 0;
            foreach (var rating in ratings)
            {
                totalRating += rating.Value;
            }

            return (double)totalRating / ratings.Count;
        }

        public static double CalculateAverageRating(this List<CollaborationRating> ratings)
        {
            if (ratings == null || ratings.Count < 1)
            {
                return 0; // No ratings
            }

            int totalRating = 0;
            foreach (var rating in ratings)
            {
                totalRating += rating.Value;
            }

            return (double)totalRating / ratings.Count;
        }
    }
}
