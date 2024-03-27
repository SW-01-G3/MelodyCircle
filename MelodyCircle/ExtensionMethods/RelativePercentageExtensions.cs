namespace MelodyCircle.ExtensionMethods
{
    public static class RelativePercentageExtensions
    {
        public static double CalculateCompletionPercentage<T>(this IEnumerable<T> completedItems, int totalItems, int decimalPlaces)
        {
            if (totalItems == 0)
            {
                return 0; // Retorna 0 se não houver itens totais
            }

            int completedCount = completedItems.Count();
            double completionPercentage = (double)completedCount / totalItems * 100;
            return Math.Round(completionPercentage, decimalPlaces);
        }
    }
}
