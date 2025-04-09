namespace RankCalculator.Services
{
    public class RankService
    {
        public double Calculate(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return 0;
            }

            int nonAlphabetic = text.Count(c => !char.IsLetter(c));
            int totalCharacters = text.Length;

            double rank = (double)nonAlphabetic / totalCharacters;
            return Math.Round(rank, 1);
        }
    }
}
