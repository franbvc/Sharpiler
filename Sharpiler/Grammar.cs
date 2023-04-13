namespace Sharpiler
{
    public static class Grammar
    {
        public static readonly char[] Vocabulary =
        {
            '+', '-', '*', '/', '(', ')', ' ', '=', '_', '\n', '\r', '>', '<',
            '!', '&', '|',
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
            'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
            'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'
        };

        public static readonly char[] Numbers =
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
        };

        public static readonly char[] Letters =
        {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
            'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
            'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'
        };
        
        public static readonly char[] IdentifierChars = 
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '_',
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
            'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
            'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'
        };

        public static readonly char[] Symbols =
        {
            '+', '-', '('
        };

        public static bool ValidChar(char toTest)
        {
            return Vocabulary.Contains(toTest);
        }

        public static bool IsNumber(char toTest)
        {
            return Numbers.Contains(toTest);
        }
        
        public static bool IsLetter(char toTest)
        {
            return Letters.Contains(toTest);
        }

        public static bool IsIdentifierChar(char toTest)
        {
            return IdentifierChars.Contains(toTest);
        }
    }
}