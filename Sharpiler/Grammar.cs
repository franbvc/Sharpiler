namespace Sharpiler
{
    public class Grammar
    {
        public readonly Char[] Vocabulary =
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '+', '-', ' '
        };

        public readonly Char[] Terminals =
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ' '
        };

        public readonly Char[] AvailableNumbers =
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
        };

        public readonly Char[] AvailableSymbols =
        {
            '+', '-'
        };
    }
}