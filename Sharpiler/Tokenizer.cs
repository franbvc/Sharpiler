namespace Sharpiler;

public class Token
{
    public string Type { get; set; }
    public dynamic Value { get; set; }

    public Token(string type, dynamic value = null!)
    {
        Type = type;
        Value = value;
    }
}

public class Tokenizer
{
    private readonly string _source;
    private int _position;
    public Token Next { get; private set; } = new Token("START");

    public Tokenizer(string source)
    {
        this._source = source;
    }

    public bool IsNextInt()
    {
        return Next.Type == "INT";
    }

    public bool IsNextFactorSymbol()
    {
        return Next.Type is "INT" or "STRING" or "MINUS" or "PLUS" or "NOT" or "LPAREN" or "IDENTIFIER" or "READ";
    }

    private Token SelectInt()
    {
        string currentToken = char.ToString(_source[_position]);

        while (true)
        {
            _position += 1;
            if (_position >= _source.Length) break;

            char currentChar = _source[_position];
            if (currentChar == ' ') break;

            if (!Grammar.ValidChar(currentChar))
                throw new LexicalException($"Invalid char '{currentChar}'");

            if (Grammar.IsNumber(currentChar))
            {
                currentToken += currentChar;
                continue;
            }

            break;
        }

        return new Token("INT", int.Parse(currentToken));
    }

    private Token SelectString()
    {
        string currentToken = char.ToString(_source[_position]);

        while (true)
        {
            _position += 1;
            if (_position >= _source.Length) throw new SyntaxException("Missing closing quote for string");
            
            char currentChar = _source[_position];
            if (currentChar == '"')
                break;

            currentToken += currentChar;
        }

        _position++;
        return new Token("STRING", currentToken);
    }

    private Token SelectWord()
    {
        string currentToken = char.ToString(_source[_position]);

        while (true)
        {
            _position += 1;
            if (_position >= _source.Length) break;

            char currentChar = _source[_position];
            if (currentChar is ' ' or '=' or '(') break;

            if (!Grammar.ValidChar(currentChar))
                throw new LexicalException($"Invalid char '{currentChar}'");

            if (Grammar.IsIdentifierChar(currentChar))
            {
                currentToken += currentChar;
                continue;
            }

            break;
        }

        return currentToken switch
        {
            "Int" => new Token("TYPE", "Int"),
            "String" => new Token("TYPE", "String"),
            "println" => new Token("PRINT"),
            "readline" => new Token("READ"),
            "while" => new Token("WHILE"),
            "end" => new Token("END"),
            "if" => new Token("IF"),
            "else" => new Token("ELSE"),
            "function" => new Token("FUNCTION"),
            "return" => new Token("RETURN"),
            _ => new Token("IDENTIFIER", currentToken)
        };
    }

    public void SelectNext()
    {
        if (_position >= _source.Length)
        {
            Next = new Token("EOF");
            return;
        }

        while (_source[_position] == ' ')
        {
            _position += 1;
            if (_position < _source.Length) continue;
            Next = new Token("EOF");
            return;
        }

        if (!Grammar.ValidChar(_source[_position]))
        {
            throw new LexicalException($"Invalid char '{_source[_position]}'");
        }

        switch (_source[_position])
        {
            case '+':
                Next = new Token("PLUS");
                _position += 1;
                return;
            case '-':
                Next = new Token("MINUS");
                _position += 1;
                return;
            case '*':
                Next = new Token("MULT");
                _position += 1;
                return;
            case '/':
                Next = new Token("DIV");
                _position += 1;
                return;
            case '(':
                Next = new Token("LPAREN");
                _position += 1;
                return;
            case ')':
                Next = new Token("RPAREN");
                _position += 1;
                return;
            case '=':
                if (_source[_position + 1] == '=')
                {
                    Next = new Token("EQ");
                    _position += 2;
                    return;
                }

                Next = new Token("ASSIGN");
                _position += 1;
                return;
            case ',':
                Next = new Token("COMMA");
                _position += 1;
                return;
            case '>':
                Next = new Token("GT");
                _position += 1;
                return;
            case '<':
                Next = new Token("LT");
                _position += 1;
                return;
            case '!':
                Next = new Token("NOT");
                _position += 1;
                return;
            case '.':
                Next = new Token("DOT");
                _position += 1;
                return;
            case '&':
                if (_source[_position + 1] == '&')
                {
                    Next = new Token("AND");
                    _position += 2;
                    return;
                }

                throw new LexicalException($"Invalid char '{_source[_position]}'");

            case '|':
                if (_source[_position + 1] == '|')
                {
                    Next = new Token("OR");
                    _position += 2;
                    return;
                }

                throw new LexicalException($"Invalid char '{_source[_position]}'");
            
            case '"':
                _position++;
                Next = SelectString();
                return;
            
            case ':':
                if (_source[_position + 1] == ':')
                {
                    Next = new Token("SCOPE");
                    _position += 2;
                    return;
                }

                throw new LexicalException($"Invalid char '{_source[_position]}'");

            case '\r':
            case '\n':
                Next = new Token("NEWLINE");
                _position += 1;
                return;
        }

        if (Grammar.IsNumber(_source[_position]))
        {
            Next = SelectInt();
            return;
        }

        if (Grammar.IsLetter(_source[_position]))
            Next = SelectWord();
    }
}