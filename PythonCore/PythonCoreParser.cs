
namespace PythonCore;

public sealed class PythonCoreParser(string sourceBuffer, int tabSize = 8, bool isInteractive = false)
{
    private readonly String _buffer = sourceBuffer;
    private int _index = 0;
    private int _symbolStartPos = 0;

    private Stack<char> _parenthsiStack = new Stack<char>();
    private Stack<int> _indentStack = new Stack<int>();
    
    private int _pending = 0;
    private bool _atBOL = true;
    private bool _isBlankLine = false;

    private List<Trivia> triviaList = new List<Trivia>();


    private Tuple<int, int> Position => new Tuple<int, int>(_symbolStartPos, _index);

    public Symbol Symbol { get; private set; }

    
    // Lexical analyzer - Get next valid symbol for parser rules ///////////////////////////////////////////////////////
    public void Advance()
    {
        if (_atBOL) /* Check if we are at beginning of line and need to check for indentation level */
        {
            _atBOL = _isBlankLine = false;
            var col = 0;

            /* Calculate indentation in form of whitespace */
            while (_buffer[_index] == ' ' || _buffer[_index] == '\t' || _buffer[_index] == '\v')
            {
                if (_buffer[_index] == ' ')
                {
                    col++;
                    triviaList.Add(new WhiteSpaceTrivia(_index, _index + 1));
                    _index++;
                }
                else if (_buffer[_index] == '\t')
                {
                    col = col + (tabSize / tabSize + 1);
                    triviaList.Add(new TabulatorTrivia(_index, _index + 1));
                    _index++;
                }
                else
                {
                    col = 0;
                    triviaList.Add(new VerticalTabulatorTrivia(_index, _index + 1));
                    _index++;
                }
            }

            /* Handle empty lines with comments or just newline */
            switch (_buffer[_index])
            {
                case '#':
                    if (isInteractive)
                    {
                        col = 0;
                        _isBlankLine = false;
                    }
                    else _isBlankLine = true;
                    break;
                case '\r':
                case '\n':
                    if (col == 0 && isInteractive)
                    {
                        _isBlankLine = false;
                    }
                    else if (isInteractive)
                    {
                        _isBlankLine = false;
                        col = 0;
                    }
                    else _isBlankLine = true;
                    break;
            }

            /* Analyze for indent or delimiter(s) */
            if (!_isBlankLine && _parenthsiStack.Count == 0)
            {
                if (_indentStack.Count == 0) _indentStack.Push(0); /* Make sure the stack has atleast one element with value 0 */

                if (col < _indentStack.Peek())
                {
                    while (true)
                    {
                        if (_indentStack.Count > 1 && col < _indentStack.Peek())
                        {
                            _pending--;
                            _indentStack.Pop();
                        }

                        if (_indentStack.Count == 1 || col == _indentStack.Peek()) break;
                    }

                    if (col != _indentStack.Peek()) throw new SyntaxError(_index, "Inconsistant indentation levels!");
                }
                else if (col > _indentStack.Peek())
                {
                    _indentStack.Push(col);
                    _pending++;
                }
            }
        }

        /* Return any pending indent or dedent(s) */
        if (_pending < 0)
        {
            _pending++;
            Symbol = new PyDedent(triviaList.ToArray());

            triviaList = new List<Trivia>(); /* Clear after added to symbol */
            return;
        }

        if (_pending > 0)
        {
            _pending--;
            Symbol = new PyIndent(triviaList.ToArray());

            triviaList = new List<Trivia>(); /* Clear after added to symbol */
            return;
        }

        AdvanceNextSymbol(); /* Analyze for all real symbols */
        
        triviaList = new List<Trivia>(); /* Clear after added to symbol */
    }

    private void AdvanceNextSymbol()
    {

        _again:
        try
        {
            /* Remove whitespace and add as trivia */
            while (_buffer[_index] == ' ' || _buffer[_index] == '\t')
            {
                triviaList.Add(
                    _buffer[_index] switch
                    {
                        ' ' => new WhiteSpaceTrivia(_index, _index + 1),
                        _ => new TabulatorTrivia(_index, _index + 1)
                    } );
                _index++;
            }

            _symbolStartPos = _index; // Save current position as start of next symbol to analyze

            /* Handle newline */
            if (_buffer[_index] == '\r' || _buffer[_index] == '\n')
            {
                if (_buffer[_index] == '\r')
                {
                    _index++;
                    if (_buffer[_index] == '\n')
                    {
                        _index++;
                        if (_isBlankLine)
                        {
                            triviaList.Add(new NewlineTrivia(_symbolStartPos, _index, '\r', '\n'));
                            goto _again;
                        }
                        Symbol = new PyNewline(_symbolStartPos, _index, '\r', '\n', triviaList.ToArray());
                        _atBOL = true;
                        return;
                    }

                    if (_isBlankLine)
                    {
                        triviaList.Add(new NewlineTrivia(_symbolStartPos, _index, '\r', ' '));
                        goto _again;
                    }
                    Symbol = new PyNewline(_symbolStartPos, _index, '\r', ' ', triviaList.ToArray());
                    _atBOL = true;
                    return;
                }

                _index++;
                if (_isBlankLine)
                {
                    triviaList.Add(new NewlineTrivia(_symbolStartPos, _index, ' ', '\n'));
                    goto _again;
                }
                Symbol = new PyNewline(_symbolStartPos, _index, ' ', '\n', triviaList.ToArray());
                _atBOL = true;
                return;
            }

            /* Handle comment or type comment */
            if (_buffer[_index] == '#')
            {
                while (_buffer[_index] != '\r' && _buffer[_index] != '\n') _index++;

                if (_buffer[_index] == '\r')
                {
                    _index++;
                    if (_buffer[_index] == '\n')
                    {
                        _index++;
                    }
                }
                else _index++;

                var text = _buffer.Substring(_symbolStartPos, _index - _symbolStartPos);

                if (text.StartsWith("# type:"))
                {
                    Symbol = new PyTypeString(_symbolStartPos, _index, text, triviaList.ToArray());
                    return;
                }

                triviaList.Add(new CommentTrivia(_symbolStartPos, _index, text));
                goto _again;
            }

            /* Handle line continuation */
            if (_buffer[_index] == '\\')
            {
                _index++;
                triviaList.Add(new LineContinuationTrivia(_symbolStartPos, _index));
                
                if (_buffer[_index] == '\r')
                {
                    _index++;
                    if (_buffer[_index] == '\n')
                    {
                        _index++;
                        triviaList.Add(new NewlineTrivia(_symbolStartPos + 1, _index, '\r', '\n'));
                    }
                    else triviaList.Add(new NewlineTrivia(_symbolStartPos + 1, _index, '\r', ' '));
                }
                else if (_buffer[_index] == '\n')
                {
                    _index++;
                    triviaList.Add(new NewlineTrivia(_symbolStartPos + 1, _index, ' ', '\n'));
                }
                else
                {
                    throw new SyntaxError(_index, "Expecting newline after '\\' in source code!");
                }

                goto _again;
            }

            /* Operator or delimiter */
            switch (_buffer[_index])
            {
                case '+':
                    _index++;
                    if (_buffer[_index] == '=')
                    {
                        _index++;
                        Symbol = new PyPlusAssign(_symbolStartPos, _index, triviaList.ToArray());
                    }
                    else
                    {
                        Symbol = new PyPlus(_symbolStartPos, _index, triviaList.ToArray());
                    }

                    return;

                case '-':
                    _index++;
                    if (_buffer[_index] == '=')
                    {
                        _index++;
                        Symbol = new PyMinusAssign(_symbolStartPos, _index, triviaList.ToArray());
                    }
                    else if (_buffer[_index] == '>')
                    {
                        _index++;
                        Symbol = new PyArrow(_symbolStartPos, _index, triviaList.ToArray());
                    }
                    else
                    {
                        Symbol = new PyMinus(_symbolStartPos, _index, triviaList.ToArray());
                    }

                    return;

                case '*':
                    _index++;
                    if (_buffer[_index] == '*')
                    {
                        _index++;
                        if (_buffer[_index] == '=')
                        {
                            _index++;
                            Symbol = new PyPowerAssign(_symbolStartPos, _index, triviaList.ToArray());
                        }
                        else
                        {
                            Symbol = new PyPower(_symbolStartPos, _index, triviaList.ToArray());
                        }
                    }
                    else if (_buffer[_index] == '=')
                    {
                        _index++;
                        Symbol = new PyMulAssign(_symbolStartPos, _index, triviaList.ToArray());
                    }
                    else
                    {
                        Symbol = new PyMul(_symbolStartPos, _index, triviaList.ToArray());
                    }

                    return;

                case '/':
                    _index++;
                    if (_buffer[_index] == '/')
                    {
                        _index++;
                        if (_buffer[_index] == '=')
                        {
                            _index++;
                            Symbol = new PyFloorDivAssign(_symbolStartPos, _index, triviaList.ToArray());
                        }
                        else
                        {
                            Symbol = new PyFloorDiv(_symbolStartPos, _index, triviaList.ToArray());
                        }
                    }
                    else if (_buffer[_index] == '=')
                    {
                        _index++;
                        Symbol = new PyDivAssign(_symbolStartPos, _index, triviaList.ToArray());
                    }
                    else
                    {
                        Symbol = new PyDiv(_symbolStartPos, _index, triviaList.ToArray());
                    }

                    return;

                case '%':
                    _index++;
                    if (_buffer[_index] == '=')
                    {
                        _index++;
                        Symbol = new PyModuloAssign(_symbolStartPos, _index, triviaList.ToArray());
                    }
                    else
                    {
                        Symbol = new PyModulo(_symbolStartPos, _index, triviaList.ToArray());
                    }

                    return;

                case '@':
                    _index++;
                    if (_buffer[_index] == '=')
                    {
                        _index++;
                        Symbol = new PyMatriceAssign(_symbolStartPos, _index, triviaList.ToArray());
                    }
                    else
                    {
                        Symbol = new PyMatrice(_symbolStartPos, _index, triviaList.ToArray());
                    }

                    return;

                case '&':
                    _index++;
                    if (_buffer[_index] == '=')
                    {
                        _index++;
                        Symbol = new PyBitAndAssign(_symbolStartPos, _index, triviaList.ToArray());
                    }
                    else
                    {
                        Symbol = new PyBitAnd(_symbolStartPos, _index, triviaList.ToArray());
                    }

                    return;

                case '|':
                    _index++;
                    if (_buffer[_index] == '=')
                    {
                        _index++;
                        Symbol = new PyBitOrAssign(_symbolStartPos, _index, triviaList.ToArray());
                    }
                    else
                    {
                        Symbol = new PyBitOr(_symbolStartPos, _index, triviaList.ToArray());
                    }

                    return;

                case '^':
                    _index++;
                    if (_buffer[_index] == '=')
                    {
                        _index++;
                        Symbol = new PyBitXorAssign(_symbolStartPos, _index, triviaList.ToArray());
                    }
                    else
                    {
                        Symbol = new PyBitXor(_symbolStartPos, _index, triviaList.ToArray());
                    }

                    return;

                case ':':
                    _index++;
                    if (_buffer[_index] == '=')
                    {
                        _index++;
                        Symbol = new PyColonAssign(_symbolStartPos, _index, triviaList.ToArray());
                    }
                    else
                    {
                        Symbol = new PyColon(_symbolStartPos, _index, triviaList.ToArray());
                    }

                    return;

                case '=':
                    _index++;
                    if (_buffer[_index] == '=')
                    {
                        _index++;
                        Symbol = new PyEqual(_symbolStartPos, _index, triviaList.ToArray());
                    }
                    else
                    {
                        Symbol = new PyAssign(_symbolStartPos, _index, triviaList.ToArray());
                    }

                    return;

                case '!':
                    _index++;
                    if (_buffer[_index] == '=')
                    {
                        _index++;
                        Symbol = new PyNotEqual(_symbolStartPos, _index, triviaList.ToArray());
                    }
                    else
                    {
                        throw new SyntaxError(_index, "Expecting '!=', but found only '!'");
                    }

                    return;

                case '~':
                    _index++;
                    Symbol = new PyBitInvert(_symbolStartPos, _index, triviaList.ToArray());

                    return;

                case '<':
                    _index++;
                    if (_buffer[_index] == '<')
                    {
                        _index++;
                        if (_buffer[_index] == '=')
                        {
                            _index++;
                            Symbol = new PyShiftLeftAssign(_symbolStartPos, _index, triviaList.ToArray());
                        }
                        else
                        {
                            Symbol = new PyShiftLeft(_symbolStartPos, _index, triviaList.ToArray());
                        }
                    }
                    else if (_buffer[_index] == '=')
                    {
                        _index++;
                        Symbol = new PyLessEqual(_symbolStartPos, _index, triviaList.ToArray());
                    }
                    else
                    {
                        Symbol = new PyLess(_symbolStartPos, _index, triviaList.ToArray());
                    }

                    return;

                case '>':
                    _index++;
                    if (_buffer[_index] == '>')
                    {
                        _index++;
                        if (_buffer[_index] == '=')
                        {
                            _index++;
                            Symbol = new PyShiftRightAssign(_symbolStartPos, _index, triviaList.ToArray());
                        }
                        else
                        {
                            Symbol = new PyShiftRight(_symbolStartPos, _index, triviaList.ToArray());
                        }
                    }
                    else if (_buffer[_index] == '=')
                    {
                        _index++;
                        Symbol = new PyGreaterEqual(_symbolStartPos, _index, triviaList.ToArray());
                    }
                    else
                    {
                        Symbol = new PyGreater(_symbolStartPos, _index, triviaList.ToArray());
                    }

                    return;

                case ',':
                    _index++;
                    Symbol = new PyComma(_symbolStartPos, _index, triviaList.ToArray());

                    return;

                case ';':
                    _index++;
                    Symbol = new PySemiColon(_symbolStartPos, _index, triviaList.ToArray());

                    return;

                case '.':
                    _index++;
                    if (_buffer[_index] == '.')
                    {
                        _index++;
                        if (_buffer[_index] == '.')
                        {
                            _index++;
                            Symbol = new PyElipsis(_symbolStartPos, _index, triviaList.ToArray());
                        }
                        else
                        {
                            throw new Exception();
                        }
                    }
                    else if (Char.IsAsciiDigit(_buffer[_index]))
                    {
                        _index--;
                        break;
                    }
                    else
                    {
                        Symbol = new PyDot(_symbolStartPos, _index, triviaList.ToArray());
                    }

                    return;

                case '(':
                    _index++;
                    _parenthsiStack.Push('(');
                    Symbol = new PyLeftParen(_symbolStartPos, _index, triviaList.ToArray());

                    return;

                case '[':
                    _index++;
                    _parenthsiStack.Push('[');
                    Symbol = new PyLeftBracket(_symbolStartPos, _index, triviaList.ToArray());

                    return;

                case '{':
                    _index++;
                    _parenthsiStack.Push('{');
                    Symbol = new PyLeftCurly(_symbolStartPos, _index, triviaList.ToArray());

                    return;

                case ')':
                    _index++;
                    if (_parenthsiStack.Peek() != '(') throw new Exception();
                    _parenthsiStack.Pop();
                    Symbol = new PyRightParen(_symbolStartPos, _index, triviaList.ToArray());

                    return;

                case ']':
                    _index++;
                    if (_parenthsiStack.Peek() != '[') throw new Exception();
                    _parenthsiStack.Pop();
                    Symbol = new PyRightBracket(_symbolStartPos, _index, triviaList.ToArray());

                    return;

                case '}':
                    _index++;
                    if (_parenthsiStack.Peek() != '{') throw new Exception();
                    _parenthsiStack.Pop();
                    Symbol = new PyRightCurly(_symbolStartPos, _index, triviaList.ToArray());

                    return;
            }

            /* Number */
            if (char.IsAsciiDigit(_buffer[_index]) || _buffer[_index] == '.')
            {
                if (_buffer[_index] == '0' || _buffer[_index] == '.')
                {
                    _index++;
                    switch (_buffer[_index])
                    {
                        case 'b':
                        case 'B':
                            _index++;
                            while (true)
                            {
                                if (_buffer[_index] == '_') _index++;
                                if (_buffer[_index] != '0' && _buffer[_index] != '1') throw new SyntaxError(_index, "Expecting only '0' or '1' in binary number!");

                                while (_buffer[_index] == '0' || _buffer[_index] == '1') _index++;

                                if (_buffer[_index] != '_') break;
                            }

                            if (char.IsAsciiDigit(_buffer[_index])) throw new SyntaxError(_index, "Expecting only '0' or '1' in binary number!");
                            break;
                        case 'o':
                        case 'O':
                            _index++;
                            while (true)
                            {
                                if (_buffer[_index] == '_') _index++;
                                if (_buffer[_index] < '0' && _buffer[_index] > '7') throw new SyntaxError(_index, "Expecting digits between '0' and '7' in octet number!");

                                while (_buffer[_index] >= '0' && _buffer[_index] <= '7') _index++;

                                if (_buffer[_index] != '_') break;
                            }

                            if (char.IsAsciiDigit(_buffer[_index])) throw new SyntaxError(_index, "Expecting digits between '0' and '7' in octet number!");
                            break;
                        case 'x':
                        case 'X':
                            _index++;
                            while (true)
                            {
                                if (_buffer[_index] == '_') _index++;
                                if (!char.IsAsciiHexDigit(_buffer[_index])) throw new SyntaxError(_index, "Expecting hexdigits!");

                                while (char.IsAsciiHexDigit(_buffer[_index])) _index++;

                                if (_buffer[_index] != '_') break;
                            }

                            if (char.IsAsciiDigit(_buffer[_index])) throw new SyntaxError(_index, "Expecting hexdigits!");
                            break;
                        default:
                            var nonzero = false;

                            if (_buffer[_index] != '.')
                            {
                                while (true)
                                {
                                    while (char.IsAsciiDigit(_buffer[_index]))
                                    {
                                        if (_buffer[_index] != '0') nonzero = true;
                                        _index++;
                                    }

                                    if (_buffer[_index] != '_') break;
                                    _index++;
                                    if (!char.IsAsciiDigit(_buffer[_index])) throw new SyntaxError(_index, "Expecting digit in number!");
                                }
                            }

                            if (_buffer[_index] == '.')
                            {
                                _index++;
                                while (true)
                                {
                                    while (char.IsAsciiDigit(_buffer[_index])) _index++;
                                    if (_buffer[_index] != '_') break;
                                    _index++;
                                    if (!char.IsAsciiDigit(_buffer[_index])) throw new SyntaxError(_index, "Expecting digit in number!");
                                }
                            }

                            if (_buffer[_index] == 'e' || _buffer[_index] == 'E')
                            {
                                _index++;
                                if (_buffer[_index] == '+' || _buffer[_index] == '-')
                                {
                                    _index++;
                                    if (!char.IsAsciiDigit(_buffer[_index])) throw new SyntaxError(_index, "Expecting digit in number!");
                                }

                                if (!char.IsAsciiDigit(_buffer[_index])) throw new SyntaxError(_index, "Expecting digit in number!");
                                while (true)
                                {
                                    while (char.IsAsciiDigit(_buffer[_index])) _index++;
                                    if (_buffer[_index] != '_') break;
                                    _index++;
                                    if (!char.IsAsciiDigit(_buffer[_index])) throw new SyntaxError(_index, "Expecting digit in number!");
                                }
                            }

                            if (_buffer[_index] == 'j' || _buffer[_index] == 'J') _index++;

                            break;
                    }
                }
                else
                {
                    if (_buffer[_index] != '.')
                    {
                        while (true)
                        {
                            while (char.IsAsciiDigit(_buffer[_index])) _index++;
                            if (_buffer[_index] != '_') break;
                            _index++;
                            if (!char.IsAsciiDigit(_buffer[_index])) throw new SyntaxError(_index, "Expecting digit in number!");
                        }
                    }

                    if (_buffer[_index] == '.')
                    {
                        _index++;
                        while (true)
                        {
                            while (char.IsAsciiDigit(_buffer[_index])) _index++;
                            if (_buffer[_index] != '_') break;
                            _index++;
                            if (!char.IsAsciiDigit(_buffer[_index])) throw new SyntaxError(_index, "Expecting digit in number!");
                        }
                    }

                    if (_buffer[_index] == 'e' || _buffer[_index] == 'E')
                    {
                        _index++;
                        if (_buffer[_index] == '+' || _buffer[_index] == '-')
                        {
                            _index++;
                            if (!char.IsAsciiDigit(_buffer[_index])) throw new SyntaxError(_index, "Expecting digit in number!");
                        }

                        if (!char.IsAsciiDigit(_buffer[_index])) throw new SyntaxError(_index, "Expecting digit in number!");
                        while (true)
                        {
                            while (char.IsAsciiDigit(_buffer[_index])) _index++;
                            if (_buffer[_index] != '_') break;
                            _index++;
                            if (!char.IsAsciiDigit(_buffer[_index])) throw new SyntaxError(_index, "Expecting digit in number!");
                        }
                    }

                    if (_buffer[_index] == 'j' || _buffer[_index] == 'J') _index++;
                }

                Symbol = new PyNumber(_symbolStartPos, _index,
                    _buffer.Substring(_symbolStartPos, _index - _symbolStartPos), triviaList.ToArray());

                return;
            }

            /* Reserved keyword or name literal */
            if (_buffer[_index] == '_' || char.IsLetter(_buffer[_index]))
            {
                while (char.IsLetterOrDigit(_buffer[_index]) || _buffer[_index] == '_') _index++;

                Symbol = IsReservedKeyword();

                if (Symbol is PyName)
                {
                    var text = Symbol as PyName;

                    switch (text?.Id)
                    {
                        case "r":
                        case "u":
                        case "R":
                        case "U":
                        case "f":
                        case "F":
                        case "fr":
                        case "Fr":
                        case "fR":
                        case "FR":
                        case "rf":
                        case "rF":
                        case "Rf":
                        case "RF":
                            if (_buffer[_index] != '"' && _buffer[_index] != '\'') return;
                            break;
                        default:
                            return;
                    }
                }
                else return;
            }

            /* String */
            if (_buffer[_index] == '"' || _buffer[_index] == '\'')
            {
                var quote = _buffer[_index++];
                var isTripple = false;
                var isEmpty = false;

                if (_buffer[_index] == quote)
                {
                    _index++;
                    if (_buffer[_index] == quote)
                    {
                        _index++;
                        isTripple = true;
                    }
                    else
                    {
                        isEmpty = true;
                    }
                }

                if (!isEmpty)
                {
                    while (true)
                    {
                        if (isTripple)
                        {
                            if (_buffer[_index] == quote)
                            {
                                _index++;
                                if (_buffer[_index] == quote)
                                {
                                    _index++;
                                    if (_buffer[_index] == quote)
                                    {
                                        _index++;
                                        break;
                                    }
                                    else _index++;
                                }
                                else _index++;
                            }
                            else _index++;
                        }
                        else
                        {
                            if (_buffer[_index] == quote)
                            {
                                _index++;
                                break;
                            }

                            if (_buffer[_index] == '\r' || _buffer[_index] == '\n')
                            {
                                throw new SyntaxError(_index, "No newline inside single quote strings allowed!");
                            }

                            _index++;
                        }
                    }
                }

                Symbol = new PyString(_symbolStartPos, _index,
                    _buffer.Substring(_symbolStartPos, _index - _symbolStartPos), triviaList.ToArray());

                return;
            }

            /* Unknow character that is not a symbol */
            throw new SyntaxError(_index, $"Illegal character '{_buffer[_index]}' found in source code!");

        }
        catch (SyntaxError e)
        {
            throw e;
        }
        catch 
        {
            Symbol = new PyEOF(_index, triviaList.ToArray());
        }

    }

    private Symbol IsReservedKeyword() =>
    
        _buffer.Substring(_symbolStartPos, _index - _symbolStartPos) switch
        {
            "False" => new PyFalse(_symbolStartPos, _index, triviaList.ToArray()),
            "None" => new PyNone(_symbolStartPos, _index, triviaList.ToArray()),
            "True" => new PyTrue(_symbolStartPos, _index, triviaList.ToArray()),
            "and" => new PyAnd(_symbolStartPos, _index, triviaList.ToArray()),
            "as" => new PyAs(_symbolStartPos, _index, triviaList.ToArray()),
            "assert" => new PyAssert(_symbolStartPos, _index, triviaList.ToArray()),
            "async" => new PyAsync(_symbolStartPos, _index, triviaList.ToArray()),
            "await" => new PyAwait(_symbolStartPos, _index, triviaList.ToArray()),
            "break" => new PyBreak(_symbolStartPos, _index, triviaList.ToArray()),
            "class" => new PyClass(_symbolStartPos, _index, triviaList.ToArray()),
            "continue" => new PyContinue(_symbolStartPos, _index, triviaList.ToArray()),
            "def" => new PyDef(_symbolStartPos, _index, triviaList.ToArray()),
            "del" => new PyDel(_symbolStartPos, _index, triviaList.ToArray()),
            "elif" => new PyElif(_symbolStartPos, _index, triviaList.ToArray()),
            "else" => new PyElse(_symbolStartPos, _index, triviaList.ToArray()),
            "except" => new PyExcept(_symbolStartPos, _index, triviaList.ToArray()),
            "finally" => new PyFinally(_symbolStartPos, _index, triviaList.ToArray()),
            "for" => new PyFor(_symbolStartPos, _index, triviaList.ToArray()),
            "from" => new PyFrom(_symbolStartPos, _index, triviaList.ToArray()),
            "global" => new PyGlobal(_symbolStartPos, _index, triviaList.ToArray()),
            "if" => new PyIf(_symbolStartPos, _index, triviaList.ToArray()),
            "import" => new PyImport(_symbolStartPos, _index, triviaList.ToArray()),
            "in" => new PyIn(_symbolStartPos, _index, triviaList.ToArray()),
            "is" => new PyIs(_symbolStartPos, _index, triviaList.ToArray()),
            "lambda" => new PyLambda(_symbolStartPos, _index, triviaList.ToArray()),
            "nonlocal" => new PyNonlocal(_symbolStartPos, _index, triviaList.ToArray()),
            "not" => new PyNot(_symbolStartPos, _index, triviaList.ToArray()),
            "or" => new PyOr(_symbolStartPos, _index, triviaList.ToArray()),
            "pass" => new PyPass(_symbolStartPos, _index, triviaList.ToArray()),
            "raise" => new PyRaise(_symbolStartPos, _index, triviaList.ToArray()),
            "return" => new PyReturn(_symbolStartPos, _index, triviaList.ToArray()),
            "try" => new PyTry(_symbolStartPos, _index, triviaList.ToArray()),
            "with" => new PyWith(_symbolStartPos, _index, triviaList.ToArray()),
            "while" => new PyWhile(_symbolStartPos, _index, triviaList.ToArray()),
            "yield" => new PyYield(_symbolStartPos, _index, triviaList.ToArray()),
            _ => new PyName(_symbolStartPos, _index, _buffer.Substring(_symbolStartPos, _index - _symbolStartPos), triviaList.ToArray())
        };
    


    // Grammar rule: Atom //////////////////////////////////////////////////////////////////////////////////////////////
    public ExpressionNode ParseAtom() => 
    
        Symbol switch
        {
            PyNone => ParseAtomNone(),
            PyFalse => ParseAtomFalse(),
            PyTrue => ParseAtomTrue(),
            PyElipsis => ParseAtomElipsis(),
            PyName => ParseAtomName(),
            PyNumber => ParseAtomNumber(),
            PyString => ParseAtomString(),
            PyLeftParen => ParseAtomTuple(),
            PyLeftBracket => ParseAtomList(),
            PyLeftCurly => ParseAtomSetOrDictionary(),
            _ => throw new SyntaxError(_symbolStartPos, "Unknown or missing literal!")
        };

    private NoneLiteralNode ParseAtomNone()
    {
        var pos = Position;
        var res = new NoneLiteralNode(pos.Item1, pos.Item2, Symbol);
        Advance();
        return res;
    }

    private FalseLiteralNode ParseAtomFalse()
    {
        var pos = Position;
        var res = new FalseLiteralNode(pos.Item1, pos.Item2, Symbol);
        Advance();
        return res;
    }

    private TrueLiteralNode ParseAtomTrue()
    {
        var pos = Position;
        var res = new TrueLiteralNode(pos.Item1, pos.Item2, Symbol);
        Advance();
        return res;
    }

    private ElipsisLiteralNode ParseAtomElipsis()
    {
        var pos = Position;
        var res = new ElipsisLiteralNode(pos.Item1, pos.Item2, Symbol);
        Advance();
        return res;
    }

    private NameLiteralNode ParseAtomName()
    {
        var pos = Position;
        var res = new NameLiteralNode(pos.Item1, pos.Item2, Symbol);
        Advance();
        return res;
    }

    private NumberLiteralNode ParseAtomNumber()
    {
        var pos = Position;
        var res = new NumberLiteralNode(pos.Item1, pos.Item2, Symbol);
        Advance();
        return res;
    }

    private StringLiteralNode ParseAtomString()
    {
        var pos = Position;
        var res = new StringLiteralNode(pos.Item1, pos.Item2, Symbol);
        Advance();
        return res;
    }

    private ElipsisLiteralNode ParseAtomTuple()
    {
        var pos = Position;
        var res = new ElipsisLiteralNode(pos.Item1, pos.Item2, Symbol);
        Advance();
        return res;
    }

    private ElipsisLiteralNode ParseAtomList()
    {
        var pos = Position;
        var res = new ElipsisLiteralNode(pos.Item1, pos.Item2, Symbol);
        Advance();
        return res;
    }

    private ElipsisLiteralNode ParseAtomSetOrDictionary()
    {
        var pos = Position;
        var res = new ElipsisLiteralNode(pos.Item1, pos.Item2, Symbol);
        Advance();
        return res;
    }

    // Grammar rule: primary ///////////////////////////////////////////////////////////////////////////////////////////

    public ExpressionNode ParsePrimaryExpression()
    {
        var pos = Position;
        var left = ParseAtom();
        var nodes = new List<ExpressionNode>();

        while (true)
        {
            switch (Symbol)
            {
                case PyDot:
                    {
                        var symbol = Symbol;
                        var pos2 = Position;
                        Advance();
                        var name = Symbol switch
                        {
                            PyName => Symbol,
                            _ => throw new SyntaxError(Position.Item1, "Expecting literal name after '.' in expression!")
                        };
                        Advance();
                        nodes.Add(new DotNameNode(pos2.Item1, Position.Item1, symbol, name));
                    }
                    break;
                case PyLeftParen:
                    {
                        var symbol1 = Symbol;
                        var pos2 = Position;
                        Advance();
                        var right = Symbol switch
                        {
                            PyRightParen => ParseArguments(),
                            _ => null
                        };
                        var symbol2 = Symbol switch
                        {
                            PyRightParen => Symbol,
                            _ => throw new SyntaxError(Position.Item1, "Missing ')' in primnary expression!")
                        };
                        Advance();
                        nodes.Add(new CallNode(pos2.Item1, Position.Item1, symbol1, right, symbol2));
                    }
                    break;
                case PyLeftBracket:
                    {
                        var symbol1 = Symbol;
                        var pos2 = Position;
                        Advance();
                        var right = ParseSlices();
                        var symbol2 = Symbol switch
                        {
                            PyRightBracket => Symbol,
                            _ => throw new SyntaxError(Position.Item1, "Missing ']' in primnary expression!")
                        };
                        Advance();
                        nodes.Add(new IndexNode(pos2.Item1, Position.Item1, symbol1, right, symbol2));
                    }
                    break;
                default:
                    return nodes.Count == 0 ? left : new PrimaryExpressionNode(pos.Item1, Position.Item1, left, nodes.ToArray());
            }
        }
    }

    private ExpressionNode ParseSlices()
    {
        var pos = Position;
        var nodes = new List<ExpressionNode>();
        var separators = new List<Symbol>();
        nodes.Add(ParseSlice());
        while (Symbol is PyComma)
        {
            separators.Add(Symbol);
            Advance();
            if (Symbol is PyRightBracket) break;
            nodes.Add( Symbol is PyMul ? ParseStaredExpression() : ParseSlice() );
        }

        return new SlicesNode(pos.Item1, Position.Item1, nodes.ToArray(), separators.ToArray());
    }

    private ExpressionNode ParseSlice()
    {
        var pos = Position;
        ExpressionNode? left = null, right = null, next = null;
        Symbol? symbol1 = null, symbol2 = null;
        left = Symbol switch
                        {
                            PyColon => null,
                            _ => ParseExpression()
                        };
        if (left is NameLiteralNode) /* Check for nammed expression */
        {
            var named = left as NameLiteralNode;
            if (Symbol is PyColonAssign)
            {
                var symbol3 = Symbol;
                Advance();
                var right2 = ParseExpression();

                return new NamedExpression(pos.Item1, Position.Item1, named, symbol3, right2);
            }
        }

        if (Symbol is PyColon)
        {
            symbol1 = Symbol;
            Advance();
        }
        else if (left == null) throw new SyntaxError(Position.Item1, "Missing ':' in index!");

        if (Symbol is not PyColon && Symbol is not PyComma && Symbol is not PyRightBracket) right = ParseExpression();

        if (Symbol is PyColon)
        {
            symbol2 = Symbol;
            Advance();

            if (Symbol is not PyComma && Symbol is not PyRightBracket) next = ParseExpression();
        }

        return new SliceNode(pos.Item1, Position.Item1, left, symbol1, right, symbol2, next);
    }


    // Grammar rule: await expression ////////////////////////////////////////////////////////////////////////////////
    public ExpressionNode ParseAwaitExpression()
    {
        if (Symbol is PyAwait)
        {
            var pos = Position;
            var symbol = Symbol;
            Advance();
            var right = ParsePrimaryExpression();

            return new AwaitExpressionNode(pos.Item1, Position.Item1, symbol, right);
        }

        return ParsePrimaryExpression();
    }

    // Grammar rule: power expression //////////////////////////////////////////////////////////////////////////////////
    public ExpressionNode ParsePowerExpression()
    {
        var pos = Position;
        var left = ParseAwaitExpression();

        if (Symbol is PyPower)
        {
            var symbol = Symbol;
            Advance();
            var right = ParseFactorExpression();

            return new PowerExpressionNode(pos.Item1, Position.Item1, left, symbol, right);
        }

        return left;
    }

    // Grammar rule: factor expression /////////////////////////////////////////////////////////////////////////////////
    public ExpressionNode ParseFactorExpression() =>
        Symbol switch
        {
            PyPlus => ParseUnaryPlusExpression(),
            PyMinus => ParseUnaryMinusExpression(),
            PyBitInvert => ParseUnaryBitInvertExpression(),
            _ => ParsePowerExpression()
        };

    private UnaryPlusExpressionNode ParseUnaryPlusExpression()
    {
        var pos = Position;
        var symbol = Symbol;
        Advance();
        var right = ParseFactorExpression();

        return new UnaryPlusExpressionNode(pos.Item1, Position.Item1, symbol, right);
    }

    private UnaryMinusExpressionNode ParseUnaryMinusExpression()
    {
        var pos = Position;
        var symbol = Symbol;
        Advance();
        var right = ParseFactorExpression();

        return new UnaryMinusExpressionNode(pos.Item1, Position.Item1, symbol, right);
    }

    private UnaryBitInvertExpressionNode ParseUnaryBitInvertExpression()
    {
        var pos = Position;
        var symbol = Symbol;
        Advance();
        var right = ParseFactorExpression();

        return new UnaryBitInvertExpressionNode(pos.Item1, Position.Item1, symbol, right);
    }

    // Grammar rule: term expression ///////////////////////////////////////////////////////////////////////////////////
    public ExpressionNode ParseTermExpression()
    {
        var pos = Position;
        var res = ParseFactorExpression();

        while (Symbol is PyMul || Symbol is PyDiv || Symbol is PyFloorDiv || Symbol is PyModulo || Symbol is PyMatrice)
        {
            if (Symbol is PyMul)
            {
                var symbol = Symbol;
                Advance();
                var right = ParseFactorExpression();
                res = new MulExpressionNode(pos.Item1, Position.Item1, res, symbol, right);
            } 
            else if (Symbol is PyDiv)
            {
                var symbol = Symbol;
                Advance();
                var right = ParseFactorExpression();
                res = new DivExpressionNode(pos.Item1, Position.Item1, res, symbol, right);
            }
            else if (Symbol is PyFloorDiv)
            {
                var symbol = Symbol;
                Advance();
                var right = ParseFactorExpression();
                res = new FloorDivExpressionNode(pos.Item1, Position.Item1, res, symbol, right);
            }
            else if (Symbol is PyModulo)
            {
                var symbol = Symbol;
                Advance();
                var right = ParseFactorExpression();
                res = new ModuloExpressionNode(pos.Item1, Position.Item1, res, symbol, right);
            }
            else
            {
                var symbol = Symbol;
                Advance();
                var right = ParseFactorExpression();
                res = new MatriceExpressionNode(pos.Item1, Position.Item1, res, symbol, right);
            }
        }

        return res;
    }

    // Grammar rule: sum expression ////////////////////////////////////////////////////////////////////////////////////
    public ExpressionNode ParseSumExpression()
    {
        var pos = Position;
        var res = ParseTermExpression();

        while (Symbol is PyPlus || Symbol is PyMinus)
        {
            if (Symbol is PyPlus)
            {
                var symbol = Symbol;
                Advance();
                var right = ParseTermExpression();
                res = new PlusExpressionNode(pos.Item1, Position.Item1, res, symbol, right);
            }
            else
            {
                var symbol = Symbol;
                Advance();
                var right = ParseTermExpression();
                res = new MinusExpressionNode(pos.Item1, Position.Item1, res, symbol, right);
            }
        }

        return res;
    }

    // Grammar rule: shift expression //////////////////////////////////////////////////////////////////////////////////
    public ExpressionNode ParseShiftExpression()
    {
        var pos = Position;
        var res = ParseSumExpression();

        while (Symbol is PyShiftLeft || Symbol is PyShiftRight)
        {
            if (Symbol is PyShiftLeft)
            {
                var symbol = Symbol;
                Advance();
                var right = ParseSumExpression();
                res = new ShiftLeftExpressionNode(pos.Item1, Position.Item1, res, symbol, right);
            }
            else
            {
                var symbol = Symbol;
                Advance();
                var right = ParseSumExpression();
                res = new ShiftRightExpressionNode(pos.Item1, Position.Item1, res, symbol, right);
            }
        }

        return res;
    }

    // Grammar rule: bitwise and expression ////////////////////////////////////////////////////////////////////////////
    public ExpressionNode ParseBitwiseAndExpression()
    {
        var pos = Position;
        var res = ParseShiftExpression();

        while (Symbol is PyBitAnd)
        {
            var symbol = Symbol;
            Advance();
            var right = ParseShiftExpression();
            res = new BitwiseAndExpressionNode(pos.Item1, Position.Item1, res, symbol, right);
        }

        return res;
    }

    // Grammar rule: bitwise xor expression ////////////////////////////////////////////////////////////////////////////
    public ExpressionNode ParseBitwiseXorExpression()
    {
        var pos = Position;
        var res = ParseBitwiseAndExpression();

        while (Symbol is PyBitXor)
        {
            var symbol = Symbol;
            Advance();
            var right = ParseBitwiseAndExpression();
            res = new BitwiseXorExpressionNode(pos.Item1, Position.Item1, res, symbol, right);
        }

        return res;
    }

    // Grammar rule: bitwise or expression /////////////////////////////////////////////////////////////////////////////
    public ExpressionNode ParseBitwiseOrExpression()
    {
        var pos = Position;
        var res = ParseBitwiseXorExpression();

        while (Symbol is PyBitOr)
        {
            var symbol = Symbol;
            Advance();
            var right = ParseBitwiseXorExpression();
            res = new BitwiseOrExpressionNode(pos.Item1, Position.Item1, res, symbol, right);
        }

        return res;
    }

    // Grammar rule: comparison ////////////////////////////////////////////////////////////////////////////////////////
    public ExpressionNode ParseComparisonExpression()
    {
        var pos = Position;
        var res = ParseBitwiseOrExpression();

        while (Symbol is PyLess || Symbol is PyLessEqual || Symbol is PyEqual || Symbol is PyGreater || Symbol is PyGreaterEqual || Symbol is PyNotEqual || Symbol is PyNot || Symbol is PyIs)
        {
            if (Symbol is PyLess)
            {
                var symbol1 = Symbol;
                Advance();

                var right = ParseBitwiseOrExpression();

                res = new LessExpressionNode(pos.Item1, Position.Item1, res, symbol1, right);
            }
            else if (Symbol is PyLessEqual)
            {
                var symbol1 = Symbol;
                Advance();

                var right = ParseBitwiseOrExpression();

                res = new LessEqualExpressionNode(pos.Item1, Position.Item1, res, symbol1, right);
            }
            else if (Symbol is PyEqual)
            {
                var symbol1 = Symbol;
                Advance();

                var right = ParseBitwiseOrExpression();

                res = new EqualExpressionNode(pos.Item1, Position.Item1, res, symbol1, right);
            }
            else if (Symbol is PyGreaterEqual)
            {
                var symbol1 = Symbol;
                Advance();

                var right = ParseBitwiseOrExpression();

                res = new GreaterEqualExpressionNode(pos.Item1, Position.Item1, res, symbol1, right);
            }
            else if (Symbol is PyGreater)
            {
                var symbol1 = Symbol;
                Advance();

                var right = ParseBitwiseOrExpression();

                res = new GreaterExpressionNode(pos.Item1, Position.Item1, res, symbol1, right);
            }
            else if (Symbol is PyNotEqual)
            {
                var symbol1 = Symbol;
                Advance();

                var right = ParseBitwiseOrExpression();

                res = new NotEqualExpressionNode(pos.Item1, Position.Item1, res, symbol1, right);
            }
            else if (Symbol is PyNot)
            {
                var symbol1  = Symbol;
                Advance();
                if (Symbol is not PyIn) throw new SyntaxError(Position.Item1, "Missing 'in' in 'not in' expression!");
                var symbol2 = Symbol;
                Advance();
                var right = ParseBitwiseOrExpression();

                res = new NotInExpressionNode(pos.Item1, Position.Item1, res, symbol1, symbol2, right);
            }
            else
            {
                var symbol1 = Symbol;
                Advance();
                if (Symbol is PyNot)
                {
                    var symbol2 = Symbol;
                    Advance();
                    var right = ParseBitwiseOrExpression();

                    res = new IsNotExpressionNode(pos.Item1, Position.Item1, res, symbol1, symbol2, right);
                }
                else
                {
                    var right = ParseBitwiseOrExpression();

                    res = new IsExpressionNode(pos.Item1, Position.Item1, res, symbol1, right);
                }
            }
        }

        return res;
    }

    // Grammar rule: not expression ////////////////////////////////////////////////////////////////////////////////////
    public ExpressionNode ParseInversionExpression()
    {
        if (Symbol is PyNot)
        {
            var pos = Position;
            var symbol = Symbol;
            Advance();
            var right = ParseInversionExpression();

            return new NotExpressionNode(pos.Item1, Position.Item1, symbol, right);
        }

        return ParseComparisonExpression();
    }

    // Grammar rule: and expression ////////////////////////////////////////////////////////////////////////////////////
    public ExpressionNode ParseConjunctionExpression()
    {
        var pos = Position;
        var res = ParseInversionExpression();

        while (Symbol is PyAnd)
        {
            var symbol = Symbol;
            Advance();
            var right = ParseInversionExpression();

            res = new AndExpressionNode(pos.Item1, Position.Item1, res, symbol, right);
        }

        return res;
    }

    // Grammar rule: or expression /////////////////////////////////////////////////////////////////////////////////////
    public ExpressionNode ParseDisjunctionExpression()
    {
        var pos = Position;
        var res = ParseConjunctionExpression();

        while (Symbol is PyOr)
        {
            var symbol = Symbol;
            Advance();
            var right = ParseConjunctionExpression();

            res = new OrExpressionNode(pos.Item1, Position.Item1, res, symbol, right);
        }

        return res;
    }











    // Grammar Rule: Expression ////////////////////////////////////////////////////////////////////////////////////////
    public ExpressionNode ParseExpression()
    {
        if (Symbol is PyLambda) return ParseLambDefExpression();

        var pos = Position;
        var left = ParseDisjunctionExpression();

        if (Symbol is PyIf)
        {
            var symbol1 = Symbol;
            Advance();
            var right = ParseDisjunctionExpression();
            if (Symbol is not PyElse) throw new SyntaxError(Position.Item1, "Expecting 'else' in expression!");

            var symbol2 = Symbol;
            Advance();
            var next = ParseExpression();

            return new TestExpressionNode(pos.Item1, Position.Item1, left, symbol1, right, symbol2, next);
        }

        return left;
    }

    // Grammar Rule: Expressions ///////////////////////////////////////////////////////////////////////////////////////
    public ExpressionNode ParseExpressions()
    {
        var pos = Position;
        var element = ParseExpression();

        if (Symbol is PyComma)
        {
            var nodes = new List<ExpressionNode>();
            var separators = new List<Symbol>();

            while (Symbol is PyComma)
            {
                separators.Add(Symbol);
                Advance();

                if (Symbol is PyComma) throw new SyntaxError(Position.Item1, "Missing element in expression list!");

                if (Symbol is PyIn) continue;

                nodes.Add(ParseExpression());
            }

            return new ExpressionsNode(pos.Item1, Position.Item1, nodes.ToArray(), separators.ToArray());
        }

        return element;
    }











    // Later! //////////////////////////////////////////////////////////////////////////////////////////////////////////

    public ExpressionNode? ParseArguments()
    {
        return null;
    }

    public ExpressionNode ParseStaredExpression()
    {
        return null;
    }

    public ExpressionNode ParseLambDefExpression()
    {
        return null;
    }

}
