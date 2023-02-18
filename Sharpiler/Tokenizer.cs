namespace Sharpiler;

public class Token
{
    public string Type { get; set; }
    public int Value { get; set; }

    public Token(string type, int value = 0)
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
            throw new LexicalException($"Invalid char '{_source[_position]}'");

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
        }

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

        Next = new Token("INT", int.Parse(currentToken));
    }
}