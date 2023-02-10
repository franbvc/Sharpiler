using System;
using System.Text.RegularExpressions;

// usar throw ou raise para avisar erro
namespace Sharpiler
{
    class Analysis
    {
        private readonly string _inputCode;
        readonly Grammar _grammar = new Grammar();

        public Analysis(string code)
        {
            _inputCode = code;
        }

        public string[] ParseInput(string code)
        {
            List<string> parsedInputArray = new List<string>();
            bool lastWasNumber = false;

            foreach (char c in code)
            {
                if (_grammar.AvailableNumbers.Contains(c))
                {
                    if (lastWasNumber && parsedInputArray.Count >= 1)
                    {
                        parsedInputArray[^1] += c;
                    }
                    else
                    {
                        parsedInputArray.Add(Char.ToString(c));
                    }

                    lastWasNumber = true;
                    continue;
                }

                lastWasNumber = false;

                if (_grammar.AvailableSymbols.Contains(c))
                {
                    parsedInputArray.Add(Char.ToString(c));
                }
            }

            return parsedInputArray.ToArray();
        }

        public bool Lexical()
        {
            foreach (char c in _inputCode)
            {
                if (_grammar.Vocabulary.Contains(c))
                {
                    continue;
                }

                throw new LexicalException(String.Format("Invalid char '{0}'", c));
            }

            return true;
        }

        public bool Syntax()
        {
            string[] parsedInput = ParseInput(_inputCode);

            if (_grammar.AvailableSymbols.Contains(parsedInput[0][0]) ||
                _grammar.AvailableSymbols.Contains(parsedInput[^1][0]))
            {
                throw new SyntaxException("Symbol at first or last position");
            }

            for (int i = 1; i < parsedInput.Length; i++)
            {
                if (_grammar.AvailableSymbols.Contains(parsedInput[i][0]) &&
                    _grammar.AvailableSymbols.Contains(parsedInput[i - 1][0]))
                {
                    throw new SyntaxException("Consecutive Symbols");
                }

                if (_grammar.AvailableNumbers.Contains(parsedInput[i][0]) &&
                    _grammar.AvailableNumbers.Contains(parsedInput[i - 1][0]))
                {
                    throw new SyntaxException("Consecutive Numbers");
                }
            }

            return true;
        }

        public int Execution()
        {
            string stripCode = _inputCode.Replace(" ", "");

            string[] inputNumbers = stripCode.Split(_grammar.AvailableSymbols, StringSplitOptions.RemoveEmptyEntries);
            string[] inputSymbols = stripCode.Split(_grammar.AvailableNumbers, StringSplitOptions.RemoveEmptyEntries);

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