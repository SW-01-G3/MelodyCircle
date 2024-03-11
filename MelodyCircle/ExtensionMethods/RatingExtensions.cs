using MelodyCircle.Models;

namespace MelodyCircle.ExtensionMethods
{
    public static class RatingExtensions
    {
        public static double CalculateAverageRating(this List<int> ratings)
        {
            if (ratings == null || ratings.Count == 0)
            {
                return 0; // No ratings
            }

            int totalRating = 0;
            foreach (var rating in ratings)
            {
                totalRating += rating;
            }

            return (double)totalRating / ratings.Count;
        }
    }
}
