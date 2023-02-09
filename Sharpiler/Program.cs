using System;


// usar throw ou raise para avisar erro
namespace Sharpiler
{
    class Analysis
    {
        public static readonly Char[] AvailableTokens =
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '+', '-', ' '
        };

        public static readonly Char[] AvailableNumbers =
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
        };

        public static readonly Char[] AvailableSymbols =
        {
            '+', '-'
        };

        private string _inputCode;

        public Analysis(string code)
        {
            _inputCode = code.Replace(" ", "");
        }

        public bool Lexical()
        {
            foreach (char c in _inputCode)
            {
                if (AvailableTokens.Contains(c))
                {
                    continue;
                }

                throw new LexicalException(String.Format("Invalid char '{0}'", c));
            }

            return true;
        }

        public bool Syntax()
        {
            if (AvailableSymbols.Contains(_inputCode[0]) ||
                AvailableSymbols.Contains(_inputCode[_inputCode.Length - 1]))
            {
                throw new SyntaxException("Symbol at first or last position");
            }

            for (int i = 1; i < _inputCode.Length; i++)
            {
                if (AvailableSymbols.Contains(_inputCode[i]) && AvailableSymbols.Contains(_inputCode[i - 1]))
                {
                    throw new SyntaxException("Consecutive Symbols");
                }
            }

            return true;
        }

        public int Execution()
        {
            string[] inputNumbers = _inputCode.Split(AvailableSymbols, StringSplitOptions.RemoveEmptyEntries);
            string[] inputSymbols = _inputCode.Split(AvailableNumbers, StringSplitOptions.RemoveEmptyEntries);

            int result = Int32.Parse(inputNumbers[0]);

            for (int i = 1; i < inputNumbers.Length; i++)
            {
                if (inputSymbols[i - 1] == "+")
                {
                    result += Int32.Parse(inputNumbers[i]);
                }
                else result -= Int32.Parse(inputNumbers[i]);
            }

            return result;
        }
    }

    class Test
    {
        static void Main(string[] args)
        {
            Analysis checker = new Analysis(args[0]);
            checker.Lexical();
            checker.Syntax();
            Console.WriteLine(checker.Execution());
        }
    }
}




