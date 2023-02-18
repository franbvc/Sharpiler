namespace Sharpiler
{
    public static class Grammar
    {
        public static readonly char[] Vocabulary =
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '+', '-', ' '
        };

        public static readonly char[] Numbers =
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
        };

        public static readonly char[] Symbols =
        {
            '+', '-'
        };

        public static bool ValidChar(char toTest)
        {
            return Vocabulary.Contains(toTest);
        }

        public static bool IsNumber(char toTest)
        {
            return Numbers.Contains(toTest);
        }
    }
}