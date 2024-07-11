
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


    private Tuple<int, int> Position => new Tuple<int, int>(_symbolStartPos, _index);

    public Symbol Symbol { get; private set; } = new PyEOF(0);

    
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
                    _index++;
                }
                else if (_buffer[_index] == '\t')
                {
                    col = col + (tabSize / tabSize + 1);
                    _index++;
                }
                else
                {
                    col = 0;
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
            Symbol = new PyDedent();
            return;
        }

        if (_pending > 0)
        {
            _pending--;
            Symbol = new PyIndent();

            return;
        }

        AdvanceNextSymbol(); /* Analyze for all real symbols */
    }

    private void AdvanceNextSymbol()
    {

        _again:
        try
        {
            /* Remove whitespace and later add as trivia */
            while (_buffer[_index] == ' ' || _buffer[_index] == '\t')
            {
                // Add code to save trivia for whitespace  later!
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
                        if (_isBlankLine) goto _again;
                        Symbol = new PyNewline(_symbolStartPos, _index, '\r', '\n');
                        _atBOL = true;
                        return;
                    }

                    if (_isBlankLine) goto _again;
                    Symbol = new PyNewline(_symbolStartPos, _index, '\r', ' ');
                    _atBOL = true;
                    return;
                }

                _index++;
                if (_isBlankLine) goto _again;
                Symbol = new PyNewline(_symbolStartPos, _index, ' ', '\n');
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
                    Symbol = new PyTypeString(_symbolStartPos, _index, text);
                    return;
                }

                goto _again;
            }

            /* Handle line continuation */
            if (_buffer[_index] == '\\')
            {
                _index++;
                if (_buffer[_index] == '\r')
                {
                    _index++;
                    if (_buffer[_index] == '\n')
                    {
                        _index++;
                    }
                }
                else if (_buffer[_index] == '\n')
                {
                    _index++;
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
                        Symbol = new PyPlusAssign(_symbolStartPos, _index);
                    }
                    else
                    {
                        Symbol = new PyPlus(_symbolStartPos, _index);
                    }

                    return;

                case '-':
                    _index++;
                    if (_buffer[_index] == '=')
                    {
                        _index++;
                        Symbol = new PyMinusAssign(_symbolStartPos, _index);
                    }
                    else if (_buffer[_index] == '>')
                    {
                        _index++;
                        Symbol = new PyArrow(_symbolStartPos, _index);
                    }
                    else
                    {
                        Symbol = new PyMinus(_symbolStartPos, _index);
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
                            Symbol = new PyPowerAssign(_symbolStartPos, _index);
                        }
                        else
                        {
                            Symbol = new PyPower(_symbolStartPos, _index);
                        }
                    }
                    else if (_buffer[_index] == '=')
                    {
                        _index++;
                        Symbol = new PyMulAssign(_symbolStartPos, _index);
                    }
                    else
                    {
                        Symbol = new PyMul(_symbolStartPos, _index);
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
                            Symbol = new PyFloorDivAssign(_symbolStartPos, _index);
                        }
                        else
                        {
                            Symbol = new PyFloorDiv(_symbolStartPos, _index);
                        }
                    }
                    else if (_buffer[_index] == '=')
                    {
                        _index++;
                        Symbol = new PyDivAssign(_symbolStartPos, _index);
                    }
                    else
                    {
                        Symbol = new PyDiv(_symbolStartPos, _index);
                    }

                    return;

                case '%':
                    _index++;
                    if (_buffer[_index] == '=')
                    {
                        _index++;
                        Symbol = new PyModuloAssign(_symbolStartPos, _index);
                    }
                    else
                    {
                        Symbol = new PyModulo(_symbolStartPos, _index);
                    }

                    return;

                case '@':
                    _index++;
                    if (_buffer[_index] == '=')
                    {
                        _index++;
                        Symbol = new PyMatriceAssign(_symbolStartPos, _index);
                    }
                    else
                    {
                        Symbol = new PyMatrice(_symbolStartPos, _index);
                    }

                    return;

                case '&':
                    _index++;
                    if (_buffer[_index] == '=')
                    {
                        _index++;
                        Symbol = new PyBitAndAssign(_symbolStartPos, _index);
                    }
                    else
                    {
                        Symbol = new PyBitAnd(_symbolStartPos, _index);
                    }

                    return;

                case '|':
                    _index++;
                    if (_buffer[_index] == '=')
                    {
                        _index++;
                        Symbol = new PyBitOrAssign(_symbolStartPos, _index);
                    }
                    else
                    {
                        Symbol = new PyBitOr(_symbolStartPos, _index);
                    }

                    return;

                case '^':
                    _index++;
                    if (_buffer[_index] == '=')
                    {
                        _index++;
                        Symbol = new PyBitXorAssign(_symbolStartPos, _index);
                    }
                    else
                    {
                        Symbol = new PyBitXor(_symbolStartPos, _index);
                    }

                    return;

                case ':':
                    _index++;
                    if (_buffer[_index] == '=')
                    {
                        _index++;
                        Symbol = new PyColonAssign(_symbolStartPos, _index);
                    }
                    else
                    {
                        Symbol = new PyColon(_symbolStartPos, _index);
                    }

                    return;

                case '=':
                    _index++;
                    if (_buffer[_index] == '=')
                    {
                        _index++;
                        Symbol = new PyEqual(_symbolStartPos, _index);
                    }
                    else
                    {
                        Symbol = new PyAssign(_symbolStartPos, _index);
                    }

                    return;

                case '!':
                    _index++;
                    if (_buffer[_index] == '=')
                    {
                        _index++;
                        Symbol = new PyNotEqual(_symbolStartPos, _index);
                    }
                    else
                    {
                        throw new SyntaxError(_index, "Expecting '!=', but found only '!'");
                    }

                    return;

                case '~':
                    _index++;
                    Symbol = new PyBitInvert(_symbolStartPos, _index);

                    return;

                case '<':
                    _index++;
                    if (_buffer[_index] == '<')
                    {
                        _index++;
                        if (_buffer[_index] == '=')
                        {
                            _index++;
                            Symbol = new PyShiftLeftAssign(_symbolStartPos, _index);
                        }
                        else
                        {
                            Symbol = new PyShiftLeft(_symbolStartPos, _index);
                        }
                    }
                    else if (_buffer[_index] == '=')
                    {
                        _index++;
                        Symbol = new PyLessEqual(_symbolStartPos, _index);
                    }
                    else
                    {
                        Symbol = new PyLess(_symbolStartPos, _index);
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
                            Symbol = new PyShiftRightAssign(_symbolStartPos, _index);
                        }
                        else
                        {
                            Symbol = new PyShiftRight(_symbolStartPos, _index);
                        }
                    }
                    else if (_buffer[_index] == '=')
                    {
                        _index++;
                        Symbol = new PyGreaterEqual(_symbolStartPos, _index);
                    }
                    else
                    {
                        Symbol = new PyGreater(_symbolStartPos, _index);
                    }

                    return;

                case ',':
                    _index++;
                    Symbol = new PyComma(_symbolStartPos, _index);

                    return;

                case ';':
                    _index++;
                    Symbol = new PySemiColon(_symbolStartPos, _index);

                    return;

                case '.':
                    _index++;
                    if (_buffer[_index] == '.')
                    {
                        _index++;
                        if (_buffer[_index] == '.')
                        {
                            _index++;
                            Symbol = new PyElipsis(_symbolStartPos, _index);
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
                        Symbol = new PyDot(_symbolStartPos, _index);
                    }

                    return;

                case '(':
                    _index++;
                    _parenthsiStack.Push('(');
                    Symbol = new PyLeftParen(_symbolStartPos, _index);

                    return;

                case '[':
                    _index++;
                    _parenthsiStack.Push('[');
                    Symbol = new PyLeftBracket(_symbolStartPos, _index);

                    return;

                case '{':
                    _index++;
                    _parenthsiStack.Push('{');
                    Symbol = new PyLeftCurly(_symbolStartPos, _index);

                    return;

                case ')':
                    _index++;
                    if (_parenthsiStack.Peek() != '(') throw new Exception();
                    _parenthsiStack.Pop();
                    Symbol = new PyRightParen(_symbolStartPos, _index);

                    return;

                case ']':
                    _index++;
                    if (_parenthsiStack.Peek() != '[') throw new Exception();
                    _parenthsiStack.Pop();
                    Symbol = new PyRightBracket(_symbolStartPos, _index);

                    return;

                case '}':
                    _index++;
                    if (_parenthsiStack.Peek() != '{') throw new Exception();
                    _parenthsiStack.Pop();
                    Symbol = new PyRightCurly(_symbolStartPos, _index);

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
                    _buffer.Substring(_symbolStartPos, _index - _symbolStartPos));

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
                    _buffer.Substring(_symbolStartPos, _index - _symbolStartPos));

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
            Symbol = new PyEOF(_index);
        }

    }

    private Symbol IsReservedKeyword() =>
    
        _buffer.Substring(_symbolStartPos, _index - _symbolStartPos) switch
        {
            "False" => new PyFalse(_symbolStartPos, _index),
            "None" => new PyNone(_symbolStartPos, _index),
            "True" => new PyTrue(_symbolStartPos, _index),
            "and" => new PyAnd(_symbolStartPos, _index),
            "as" => new PyAs(_symbolStartPos, _index),
            "assert" => new PyAssert(_symbolStartPos, _index),
            "async" => new PyAsync(_symbolStartPos, _index),
            "await" => new PyAwait(_symbolStartPos, _index),
            "break" => new PyBreak(_symbolStartPos, _index),
            "class" => new PyClass(_symbolStartPos, _index),
            "continue" => new PyContinue(_symbolStartPos, _index),
            "def" => new PyDef(_symbolStartPos, _index),
            "del" => new PyDel(_symbolStartPos, _index),
            "elif" => new PyElif(_symbolStartPos, _index),
            "else" => new PyElse(_symbolStartPos, _index),
            "except" => new PyExcept(_symbolStartPos, _index),
            "finally" => new PyFinally(_symbolStartPos, _index),
            "for" => new PyFor(_symbolStartPos, _index),
            "from" => new PyFrom(_symbolStartPos, _index),
            "global" => new PyGlobal(_symbolStartPos, _index),
            "if" => new PyIf(_symbolStartPos, _index),
            "import" => new PyImport(_symbolStartPos, _index),
            "in" => new PyIn(_symbolStartPos, _index),
            "is" => new PyIs(_symbolStartPos, _index),
            "lambda" => new PyLambda(_symbolStartPos, _index),
            "nonlocal" => new PyNonlocal(_symbolStartPos, _index),
            "not" => new PyNot(_symbolStartPos, _index),
            "or" => new PyOr(_symbolStartPos, _index),
            "pass" => new PyPass(_symbolStartPos, _index),
            "raise" => new PyRaise(_symbolStartPos, _index),
            "return" => new PyReturn(_symbolStartPos, _index),
            "try" => new PyTry(_symbolStartPos, _index),
            "with" => new PyWith(_symbolStartPos, _index),
            "while" => new PyWhile(_symbolStartPos, _index),
            "yield" => new PyYield(_symbolStartPos, _index),
            _ => new PyName(_symbolStartPos, _index, _buffer.Substring(_symbolStartPos, _index - _symbolStartPos))
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
                    break;
                case PyLeftParen:
                    break;
                case PyLeftBracket:
                    break;
                default:
                    return nodes.Count == 0 ? left : new PrimaryExpressionNode(pos.Item1, Position.Item1, left, nodes.ToArray());
            }
        }
    }
}
