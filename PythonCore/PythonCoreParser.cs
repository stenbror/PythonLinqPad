﻿
using System.Runtime.InteropServices;
using System.Xml.Linq;
using Microsoft.VisualBasic;

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

    private bool _isStarExcept = false;
    private bool _seenDefaultExcept = false;

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
            Symbol = new PyDedent();
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
                        triviaList = new List<Trivia>();
                        return;
                    }

                    if (_isBlankLine)
                    {
                        triviaList.Add(new NewlineTrivia(_symbolStartPos, _index, '\r', ' '));
                        goto _again;
                    }
                    Symbol = new PyNewline(_symbolStartPos, _index, '\r', ' ', triviaList.ToArray());
                    _atBOL = true;
                    triviaList = new List<Trivia>();
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

    private ExpressionNode ParseAtomSetOrDictionary()
    {
        var pos = Position;
        if (Symbol is not PyLeftCurly) throw new SyntaxError(Position.Item1, "Expecting '{' in dictionary or set!");
        var symbol1 = Symbol;
        Advance();
        if (Symbol is PyRightCurly)
        {
            var symbol2 = Symbol;
            Advance();

            return new DictionaryLiteralNode(pos.Item1, Position.Item1, symbol1, [], [], symbol2);
        }

        var isDict = true;
        var elements = new List<ExpressionNode>();
        var separators = new List<Symbol>();
        var pos2 = Position;

        if (Symbol is PyMul)
        {
            isDict = false;
            elements.Add(ParseStaredNameExpressions());
        }
        else if (Symbol is PyPower) elements.Add(ParseDoubleStarKeyValuePairExpressionNode());
        else
        {
            var key = ParseExpression();
            if (Symbol is PyColon)
            {
                var symbol3 = Symbol;
                Advance();
                var value2 = ParseExpression();

                var tmp = new KeyValueElementExpressionNode(pos2.Item1, Position.Item1, key, symbol3, value2);
                elements.Add(tmp);
            }
            else
            {
                isDict = false;
                elements.Add(key);
            }
        }

        if (Symbol is PyAsync || Symbol is PyFor)
        {
            while (Symbol is PyAsync || Symbol is PyFor) elements.Add(ParseForClauseExpression());
        }
        else
        {
            while (Symbol is PyComma)
            {
                separators.Add(Symbol);
                Advance();
                if (Symbol is PyComma) throw new SyntaxError(Position.Item1, "Expecting element in dictionary or set!");
                if (Symbol is not PyRightCurly)
                {
                    elements.Add( isDict ? ParseDoubleStarKeyValuePairExpressionNode() : ParseStaredNameExpressions() );
                }
            }
        }

        if (Symbol is not PyRightCurly) throw new SyntaxError(Position.Item1, "Expecting '}' in dictionary or set!");
        var symbol4 = Symbol;
        Advance();

        if (isDict)
        {
            return new DictionaryLiteralNode(pos.Item1, Position.Item1, symbol1, elements.ToArray(),
                separators.ToArray(), symbol4);
        }
        else
        {
            return new SetLiteralExpressionNode(pos.Item1, Position.Item1, symbol1, elements.ToArray(), separators.ToArray(), symbol4);
        }
    }

    private ExpressionNode ParseDoubleStarKeyValuePairExpressionNode()
    {
        if (Symbol is PyPower)
        {
            var pos = Position;
            var symbol1 = Symbol;
            Advance();
            var right = ParseBitwiseOrExpression();

            return new DoubleStarDictionaryExpressionNode(pos.Item1, Position.Item1, symbol1, right);
        }

        return ParseKeyValueExpression();
    }

    private ExpressionNode ParseKeyValueExpression()
    {
        var pos = Position;
        var left = ParseExpression();
        if (Symbol is not PyColon) throw new SyntaxError(Position.Item1, "Expecting ':' in dictionary element!");
        var symbol1 = Symbol;
        Advance();
        var right = ParseExpression();

        return new KeyValueElementExpressionNode(pos.Item1, Position.Item1, left, symbol1, right);
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
            nodes.Add( Symbol is PyMul ? ParseStarNamedExpression() : ParseSlice() );
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

    // Grammar rule: stared expression /////////////////////////////////////////////////////////////////////////////////
    public ExpressionNode ParseNamedExpression()
    {
        var pos = Position;
        var left = ParseExpression();

        if (left is NameLiteralNode)
        {
            if (Symbol is PyColonAssign)
            {
                var symbol1 = (left as NameLiteralNode)!.Element;
                var symbol2 = Symbol;
                Advance();
                var right = ParseExpression();

                return new NamedExpressionNode(pos.Item1, Position.Item1, symbol1, symbol2, right);
                
            }
        }

        return left;
    }

    // Grammar rule: stared expression /////////////////////////////////////////////////////////////////////////////////
    public ExpressionNode ParseStarNamedExpression()
    {
        if (Symbol is PyMul)
        {
            var pos = Position;
            var symbol1 = Symbol;
            Advance();

            var right = ParseBitwiseOrExpression();

            return new StarExpressionNode(pos.Item1, Position.Item1, symbol1, right);
        }

        return ParseNamedExpression();
    }

    // Grammar rule: stared name expressions /////////////////////////////////////////////////////////////////////////
    public ExpressionNode ParseStaredNameExpressions()
    {
        var pos = Position;
        var element = ParseStarNamedExpression();

        if (Symbol is PyComma)
        {
            var nodes = new List<ExpressionNode>();
            var separators = new List<Symbol>();

            nodes.Add(element);

            while (Symbol is PyComma)
            {
                separators.Add(Symbol);
                Advance();

                if (Symbol is PyComma) throw new SyntaxError(Position.Item1, "Missing element in expression list!");

                if (Symbol is PyIn) continue; // Check later, what should be checked here!

                nodes.Add(ParseStarNamedExpression());
            }

            return new StarNamedExpressionsNode(pos.Item1, Position.Item1, nodes.ToArray(), separators.ToArray());
        }

        return element;
    }

    // Grammar rule: star expressions //////////////////////////////////////////////////////////////////////////////////
    public ExpressionNode ParseStarExpression()
    {
        if (Symbol is PyMul)
        {
            var pos = Position;
            var symbol1 = Symbol;
            Advance();

            var right = ParseBitwiseOrExpression();

            return new StarExpressionNode(pos.Item1, Position.Item1, symbol1, right);
        }

        return ParseExpression();
    }

    // Grammar rule: star expressions //////////////////////////////////////////////////////////////////////////////////
    public ExpressionNode ParseStarExpressions()
    {
        var pos = Position;
        var element = ParseStarExpression();

        if (Symbol is PyComma)
        {
            var nodes = new List<ExpressionNode>();
            var separators = new List<Symbol>();

            nodes.Add(element);

            while (Symbol is PyComma)
            {
                separators.Add(Symbol);
                Advance();

                if (Symbol is PyComma) throw new SyntaxError(Position.Item1, "Missing element in expression list!");

                if (Symbol is PyIn) continue;

                nodes.Add(ParseStarExpression());
            }

            return new StarExpressionsNode(pos.Item1, Position.Item1, nodes.ToArray(), separators.ToArray());
        }

        return element;
    }

    // Grammar rule: Yield expression //////////////////////////////////////////////////////////////////////////////////
    public ExpressionNode ParseYieldExpression()
    {
        var pos = Position;
        if (Symbol is not PyYield) throw new SyntaxError(Position.Item1, "Expecting 'yield' in yield expression!");
        var symbol1 = Symbol;
        Advance();

        if (Symbol is PyFrom)
        {
            var symbol2 = Symbol;
            Advance();
            var right = ParseExpression();

            return new YieldFromExpressionNode(pos.Item1, Position.Item1, symbol1, symbol2, right);
        }

        var right2 = ParseStarExpressions();

        return new YieldExpressionNode(pos.Item1, Position.Item1, symbol1, right2);
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

            nodes.Add(element);

            while (Symbol is PyComma)
            {
                separators.Add(Symbol);
                Advance();

                if (Symbol is PyComma) throw new SyntaxError(Position.Item1, "Missing element in expression list!");

                if (Symbol is PySemiColon || Symbol is PyNewline) continue;

                nodes.Add(ParseExpression());
            }

            return new ExpressionsNode(pos.Item1, Position.Item1, nodes.ToArray(), separators.ToArray());
        }

        return element;
    }

    // Grammar rule: lambda definition /////////////////////////////////////////////////////////////////////////////////
    public LambdaExpressionNode ParseLambDefExpression()
    {
        var pos = Position;
        var symbol1 = Symbol;
        Advance();

        ExpressionNode? args = Symbol switch
        {
            PyColon => null,
            _ => ParseLambdaParams()
        };

        if (Symbol is not PyColon) throw new SyntaxError(Position.Item1,  "Missing ':' in 'lambda' expression!");
        var symbol2 = Symbol;
        Advance();

        var right = ParseExpression();

        return new LambdaExpressionNode(pos.Item1, Position.Item1, symbol1, args, symbol2, right);
    }

    private ExpressionNode ParseLambdaParams()
    {
        throw new NotImplementedException();
    }






    // Grammar rule: if clause /////////////////////////////////////////////////////////////////////////////////////////
    public ExpressionNode ParseIfClauseExpression()
    {
        var pos = Position;
        if (Symbol is not PyIf) throw new SyntaxError(Position.Item1, "Expecting 'if' in generator expression!");
        var symbol1 = Symbol;
        Advance();
        var right = ParseDisjunctionExpression();

        return new GeneratorIfExpressionNode(pos.Item1, Position.Item1, symbol1, right);
    }

    // Grammar rule: for clause ////////////////////////////////////////////////////////////////////////////////////////
    public ExpressionNode ParseForClauseExpression()
    {
        var pos = Position;

        if (Symbol is PyFor)
        {
            var symbol1 = Symbol;
            Advance();
            var left = ParseStarTargetsExpression();
            if (Symbol is not PyIn) throw new SyntaxError(Position.Item1, "Expecting 'in' in generator expression!");
            var symbol2 = Symbol;
            Advance();
            var right = ParseDisjunctionExpression();
            var nodes = new List<ExpressionNode>();

            while (Symbol is PyIf) nodes.Add(ParseIfClauseExpression());

            return new GeneratorForExpressionNode(pos.Item1, Position.Item1, symbol1, left, symbol2, right, nodes.ToArray());
        }
        else if (Symbol is PyAsync)
        {
            var symbol0 = Symbol;
            Advance();

            var symbol1 = Symbol;
            Advance();
            var left = ParseStarTargetsExpression();
            if (Symbol is not PyIn) throw new SyntaxError(Position.Item1, "Expecting 'in' in generator expression!");
            var symbol2 = Symbol;
            Advance();
            var right = ParseDisjunctionExpression();
            var nodes = new List<ExpressionNode>();

            while (Symbol is PyIf) nodes.Add(ParseIfClauseExpression());

            return new GeneratorAsyncForExpressionNode(pos.Item1, Position.Item1, symbol0, symbol1, left, symbol2, right, nodes.ToArray());
        }
        else throw new SyntaxError(Position.Item1, "Expecting 'for' or 'async' in generator expression!");
    }

    // Grammar rule: clauses ///////////////////////////////////////////////////////////////////////////////////////////
    public ExpressionNode ParseClausesExpression()
    {
        var pos = Position;
        
        if (Symbol is not PyFor && Symbol is not PyAsync) throw new SyntaxError(Position.Item1, "Expecting 'for' or 'async' in generator expression!");
        
        var element = ParseForClauseExpression();

        if (Symbol is not PyFor && Symbol is not PyAsync)
        {
            var nodes = new List<ExpressionNode>();
            
            while (Symbol is not PyFor && Symbol is not PyAsync) nodes.Add(ParseForClauseExpression());

            return new GeneratorGroupExpressionNode(pos.Item1, Position.Item1, nodes.ToArray());
        }

        return element;
    }

    private ExpressionNode ParseStarTargetsExpression()
    {
        throw new NotImplementedException();
    }



    // Later! //////////////////////////////////////////////////////////////////////////////////////////////////////////

    public ExpressionNode? ParseArguments()
    {
        return null;
    }







    // Grammar rule: stmts /////////////////////////////////////////////////////////////////////////////////////////////
    public StatementNode ParseStmts()
    {
        var pos = Position;
        var elements = new List<StatementNode>();

        while (Symbol is not PyEOF) elements.Add(ParseStmt());

        return elements.Count == 1 ? elements[0] :
            new StmtsNode(pos.Item1, Position.Item1, elements.ToArray());
    }

    // Grammar rule: stmt //////////////////////////////////////////////////////////////////////////////////////////////
    public StatementNode ParseStmt()
    {
        if (Symbol is PyName)
        {
            if ((Symbol as PyName)?.Id == "match")
            {
                var name = Symbol as PyName;
                Symbol = new PyMatch(name!.StartPos, name.EndPos, name.Trivias);

                return ParseCompoundStmt();
            }
            return ParseSimpleStmts();
        }

        return Symbol switch
        {
            PyDef => ParseCompoundStmt(),
            PyMatrice => ParseCompoundStmt(),
            PyIf => ParseCompoundStmt(),
            PyClass => ParseCompoundStmt(),
            PyWith => ParseCompoundStmt(),
            PyFor => ParseCompoundStmt(),
            PyTry => ParseCompoundStmt(),
            PyWhile => ParseCompoundStmt(),
            PyAsync => ParseCompoundStmt(),
            _ => ParseSimpleStmts()
        };
    }
        
    // Grammar rule: simple stmts //////////////////////////////////////////////////////////////////////////////////////
    public StatementNode ParseSimpleStmts()
    {
        var pos = Position;
        var elements = new List<StatementNode>();
        var separators = new List<Symbol>();
        elements.Add(ParseSimpleStmt());

        while (Symbol is PySemiColon)
        {
            separators.Add(Symbol);
            Advance();
            if (Symbol is PySemiColon) throw new SyntaxError(Position.Item1, "Missing statement!");
            if (Symbol is not PyNewline && Symbol is not PyEOF) elements.Add(ParseSimpleStmt());
        }

        if (Symbol is not PyNewline) throw new SyntaxError(Position.Item1, "Expecting NEWLINE after statement!");
        var symbol1 = Symbol;
        Advance();

        return new SimpleStmtsNode(pos.Item1, Position.Item1, elements.ToArray(), separators.ToArray(), symbol1);
    }

    // Grammar rule: simple stmt ///////////////////////////////////////////////////////////////////////////////////////
    public StatementNode ParseSimpleStmt()
    {
        if ((Symbol as PyName)?.Id == "type") /* Convert name to type symbol */
        {
            var name = Symbol as PyName;
            Symbol = new PyType(name!.StartPos, name.EndPos, name.Trivias);

            return ParseTypeAliasStmt();
        }

        return Symbol switch
        {
            PyMul => ParseStarExpressionStmt(),
            PyReturn => ParseReturnStmt(),
            PyImport => ParseImportStmt(),
            PyFrom => ParseImportStmt(),
            PyRaise => ParseRaiseStmt(),
            PyPass => ParsePassStmt(),
            PyDel => ParseDelStmt(),
            PyYield => ParseYieldStmt(),
            PyAssert => ParseAssertStmt(),
            PyBreak => ParseBreakStmt(),
            PyContinue => ParseContinueStmt(),
            PyGlobal => ParseGlobalStmt(),
            PyNonlocal => ParseNonlocalStmt(),
            _ => ParseAssignmentStmt()
        };
    }

    // Grammar rule: assignment stmt //////////////////////////////////////////////////////////////////////////////////
    public StatementNode ParseAssignmentStmt()
    {
        throw new NotImplementedException();
    }

    // Grammar rule: star expressions //////////////////////////////////////////////////////////////////////////////////
    public StatementNode ParseStarExpressionStmt()
    {
        throw new NotImplementedException();
    }

    // Grammar rule: return stmts //////////////////////////////////////////////////////////////////////////////////////
    public StatementNode ParseReturnStmt()
    {
        var pos = Position;
        if (Symbol is not PyReturn) throw new SyntaxError(Position.Item1, "Expecting 'return' in return statement!");
        var symbol1 = Symbol;
        Advance();

        var right = Symbol is PySemiColon || Symbol is PyNewline ? null : ParseStarExpressions();

        return new ReturnNode(pos.Item1, Position.Item1, symbol1, right);
    }

    // Grammar rule: import stmt ///////////////////////////////////////////////////////////////////////////////////////
    public StatementNode ParseImportStmt() =>
        Symbol switch
        {
            PyImport => ParseImportNameStmt(),
            PyFrom => ParseImportFromStmt(),
            _ => throw new SyntaxError(Position.Item1, "")
        };

    private StatementNode ParseImportNameStmt()
    {
        var pos = Position;
        if (Symbol is not PyImport) throw new SyntaxError(Position.Item1, "Expecting 'import' in import statement!");
        var symbol1 = Symbol;
        Advance();
        var right = ParseDottedAsNames();

        return new ImportNameNode(pos.Item1, Position.Item1, symbol1, right);
    }

    private StatementNode ParseImportFromStmt()
    {
        var pos = Position;
        if (Symbol is not PyFrom) throw new SyntaxError(Position.Item1, "Expecting 'from' in import statement!");
        var symbol1 = Symbol;
        Advance();
        StatementNode left = null;
        var dots = new List<Symbol>();

        while (Symbol is PyDot || Symbol is PyElipsis)
        {
            dots.Add(Symbol);
            Advance();
        }

        if (Symbol is PyImport && dots.Count == 0) throw new SyntaxError(Position.Item1, "Missing 'from' part of import statement!");
        if (Symbol is not PyImport) left = ParseDottedName();
        if (Symbol is not PyImport) throw new SyntaxError(Position.Item1, "Missing 'import' part of from import statement!");

        var symbol2 = Symbol; /* 'import' */
        Advance();

        Symbol? symbol3 = null, symbol4 = null;
        StatementNode? right = null;

        if (Symbol is PyMul)
        {
            symbol3 = Symbol;
            Advance();
        }
        else if (Symbol is PyLeftParen)
        {
            symbol3 = Symbol;
            Advance();
            right = ParseImportFromAsNames();

            if (Symbol is not PyRightParen) throw new SyntaxError(Position.Item1, "Missing ')' in import statement!");
            symbol4 = Symbol;
            Advance();
        }
        else
        {
            right = ParseImportFromAsNames();
        }

        return new ImportFromStmtNode(pos.Item1, Position.Item1, symbol1, dots.ToArray(), left, symbol2, symbol3, right, symbol4);
    }

    private StatementNode ParseImportFromAsNames()
    {
        var pos = Position;
        var elements = new List<StatementNode>();
        var separators = new List<Symbol>();

        elements.Add(ParseImportFromAsName());

        while (Symbol is PyComma)
        {
            separators.Add(Symbol);
            Advance();
            elements.Add(ParseImportFromAsName());
        }

        return elements.Count == 1 ? elements[0] : new ImportFromAsNamesNode(pos.Item1, Position.Item1, elements.ToArray(), separators.ToArray());
    }

    private StatementNode ParseImportFromAsName()
    {
        var pos = Position;

        if (Symbol is not PyName) throw new SyntaxError(Position.Item1, "Missing NAME literal in import from as statement!");
        var symbol1 = Symbol;
        Advance();

        if (Symbol is PyAs)
        {
            var symbol2 = Symbol;
            Advance();

            if (Symbol is not PyName) throw new SyntaxError(Position.Item1, "Missing NAME literal in import from as statement!");
            var symbol3 = Symbol;
            Advance();

            return new ImportFromAsNode(pos.Item1, Position.Item1, symbol1, symbol2, symbol3);
        }

        return new ImportFromNode(pos.Item1, Position.Item1, symbol1);
    }

    private StatementNode ParseDottedAsNames()
    {
        var pos = Position;
        var elements = new List<StatementNode>();
        var separators = new List<Symbol>();

        elements.Add(ParseDottedAsName());

        while (Symbol is PyComma)
        {
            separators.Add(Symbol);
            Advance();
            elements.Add(ParseDottedAsName());
        }

        return elements.Count == 1 ? elements[0] : new DottedAsNamesNode(pos.Item1, Position.Item1, elements.ToArray(), separators.ToArray());
    }

    private StatementNode ParseDottedAsName()
    {
        var pos = Position;

        var left = ParseDottedName();
        
        if (Symbol is PyAs)
        {
            var symbol1 = Symbol;
            Advance();

            if (Symbol is not PyName) throw new SyntaxError(Position.Item1, "Missing NAME literal in import as statement!");
            var symbol2 = Symbol;
            Advance();

            return new DottedAsNameNode(pos.Item1, Position.Item1, left, symbol1, symbol2);
        }

        return left;
    }

    private StatementNode ParseDottedName()
    {
        var pos = Position;
        var elements = new List<Symbol>();
        var separators = new List<Symbol>();

        if (Symbol is not PyName) throw new SyntaxError(Position.Item1, "Missing NAME literal in import statement!");
        elements.Add(Symbol);
        Advance();

        while (Symbol is PyDot)
        {
            separators.Add(Symbol);
            Advance();

            if (Symbol is not PyName) throw new SyntaxError(Position.Item1, "Missing NAME literal in import statement!");
            elements.Add(Symbol);
            Advance();
        }

        return new DottedNameNode(pos.Item1, Position.Item1, elements.ToArray(), separators.ToArray());
    }

    // Grammar rule: raise stmt ////////////////////////////////////////////////////////////////////////////////////////
    public StatementNode ParseRaiseStmt()
    {
        var pos = Position;
        if (Symbol is not PyRaise) throw new SyntaxError(Position.Item1, "Expecting 'raise' in raise statement!");
        var symbol1 = Symbol;
        Advance();

        if (Symbol is PyNewline || Symbol is PySemiColon) return new RaiseNode(pos.Item1, Position.Item1, symbol1);

        var left = ParseExpression();

        if (Symbol is PyFrom)
        {
            var symbol2 = Symbol;
            Advance();
            var right = ParseExpression();

            return new RaiseFromNode(pos.Item1, Position.Item1, symbol1, left, symbol2, right);
        }

        return new RaiseElementNode(pos.Item1, Position.Item1, symbol1, left);
    }

    // Grammar rule: pass stmt /////////////////////////////////////////////////////////////////////////////////////////
    public StatementNode ParsePassStmt()
    {
        var pos = Position;
        var symbol1 = Symbol;
        Advance();

        return new PassStmtNode(pos.Item1, Position.Item1, symbol1);
    }

    // Grammar rule: del stmt //////////////////////////////////////////////////////////////////////////////////////////
    public StatementNode ParseDelStmt()
    {
        var pos = Position;
        if (Symbol is not PyDel) throw new SyntaxError(Position.Item1, "Expecting 'del' in del statement!");
        var symbol1 = Symbol;
        Advance();

        var right = ParseDelTargets();

        return new DelStatementNode(pos.Item1, Position.Item1, symbol1, right);
    }

    private StatementNode ParseDelTargets()
    {
        var pos = Position;
        var elements = new List<StatementNode>();
        var separators = new List<Symbol>();

        elements.Add(ParseDelTarget());

        while (Symbol is PyComma)
        {
            separators.Add(Symbol);
            Advance();

            elements.Add(ParseDelTarget());
        }

        return new DelTargetsNode(pos.Item1, Position.Item1, elements.ToArray(), separators.ToArray());
    }

    private StatementNode ParseDelTarget()
    {
        throw new NotImplementedException();
    }

    // Grammar rule: yield stmt ////////////////////////////////////////////////////////////////////////////////////////
    public StatementNode ParseYieldStmt()
    {
        var pos = Position;
        var right = ParseYieldExpression();

        return new YieldStmtNode(pos.Item1, Position.Item1, right);
    }

    // Grammar rule: assert stmt ///////////////////////////////////////////////////////////////////////////////////////
    public StatementNode ParseAssertStmt()
    {
        var pos = Position;
        if (Symbol is not PyAssert) throw new SyntaxError(Position.Item1, "Expecting 'assert' in assert statement!");
        var symbol1 = Symbol;
        Advance();

        var left = ParseExpression();

        if (Symbol is PyComma)
        {
            var symbol2 = Symbol;
            Advance();

            var right = ParseExpression();

            return new AssertNode(pos.Item1, Position.Item1, symbol1, left, symbol2, right);
        }

        return new AssertSingleNode(pos.Item1, Position.Item1, symbol1, left);
    }

    // Grammar rule: break stmt ////////////////////////////////////////////////////////////////////////////////////////
    public StatementNode ParseBreakStmt()
    {
        var pos = Position;
        var symbol1 = Symbol;
        Advance();

        return new BreakStmtNode(pos.Item1, Position.Item1, symbol1);
    }

    // Grammar rule: continue stmt /////////////////////////////////////////////////////////////////////////////////////
    public StatementNode ParseContinueStmt()
    {
        var pos = Position;
        var symbol1 = Symbol;
        Advance();

        return new ContinueStmtNode(pos.Item1, Position.Item1, symbol1);
    }

    // Grammar rule: global stmt ///////////////////////////////////////////////////////////////////////////////////////
    public StatementNode ParseGlobalStmt()
    {
        var pos = Position;

        if (Symbol is not PyGlobal) throw new SyntaxError(Position.Item1, "Expecting 'global' in global statement!");
        var symbol1 = Symbol;
        Advance();

        var nodes = new List<Symbol>();
        var separators = new List<Symbol>();

        if (Symbol is not PyName) throw new SyntaxError(Position.Item1, "Expecting NAME literal in global statement!");
        nodes.Add(Symbol);
        Advance();

        while (Symbol is PyComma)
        {
            separators.Add(Symbol);
            Advance();

            if (Symbol is not PyName) throw new SyntaxError(Position.Item1, "Expecting NAME literal in global statement!");
            nodes.Add(Symbol);
            Advance();
        }

        return new GlobalNode(pos.Item1, Position.Item1, symbol1, nodes.ToArray(), separators.ToArray());
    }

    // Grammar rule: nonlocal stmt /////////////////////////////////////////////////////////////////////////////////////
    public StatementNode ParseNonlocalStmt()
    {
        var pos = Position;

        if (Symbol is not PyNonlocal) throw new SyntaxError(Position.Item1, "Expecting 'nonlocal' in global statement!");
        var symbol1 = Symbol;
        Advance();

        var nodes = new List<Symbol>();
        var separators = new List<Symbol>();

        if (Symbol is not PyName) throw new SyntaxError(Position.Item1, "Expecting NAME literal in nonlocal statement!");
        nodes.Add(Symbol);
        Advance();

        while (Symbol is PyComma)
        {
            separators.Add(Symbol);
            Advance();

            if (Symbol is not PyName) throw new SyntaxError(Position.Item1, "Expecting NAME literal in nonlocal statement!");
            nodes.Add(Symbol);
            Advance();
        }

        return new NonlocalNode(pos.Item1, Position.Item1, symbol1, nodes.ToArray(), separators.ToArray());
    }

    // Grammar rule: type alias stmt ///////////////////////////////////////////////////////////////////////////////////
    public StatementNode ParseTypeAliasStmt()
    {
        var pos = Position;
        if (Symbol is not PyType) throw new SyntaxError(Position.Item1, "Expecting 'type' in type alias statement!");
        var symbol1 = Symbol;
        Advance();

        if (Symbol is not PyName) throw new SyntaxError(Position.Item1, "Expecting NAME literal in type alias statement!");
        var name = Symbol;
        Advance();

        StatementNode? param = Symbol switch
                                        {
                                            PyLeftBracket => ParseTypeParams(),
                                            _ => null
                                        };
        if (Symbol is not PyAssign) throw new SyntaxError(Position.Item1, "Expecting '=' in type alias statement!");
        var symbol2 = Symbol;
        Advance();

        var right = ParseExpression();

        return new TypeAliasNode(pos.Item1, Position.Item1, symbol1, name, param, symbol2, right);
    }

    private StatementNode ParseTypeParams()
    {
        var pos = Position;
        Symbol symbol1, symbol2;
        
        if (Symbol is not PyLeftBracket) throw new SyntaxError(Position.Item1, "Expecting '[' in type alias!");
        symbol1 = Symbol;
        Advance();
     
        var right = ParseTypeParamSeq();

        if (Symbol is not PyRightBracket) throw new SyntaxError(Position.Item1, "Expecting ']' in type alias!");
        symbol2 = Symbol;
        Advance();

        return new TypeParamsNode(pos.Item1, Position.Item1, symbol1, right, symbol2);
    }

    private StatementNode ParseTypeParamSeq()
    {
        var pos = Position;
        var nodes = new List<StatementNode>();
        var separators = new List<Symbol>();

        nodes.Add(ParseTypeParam());

        while (Symbol is PyComma)
        {
            separators.Add(Symbol);
            Advance();
            if (Symbol is not PyRightBracket) nodes.Add(ParseTypeParam());
        }

        return new TypeParamSequenceNode(pos.Item1, Position.Item1, nodes.ToArray(), separators.ToArray());
    }

    private StatementNode ParseTypeParam()
    {
        var pos = Position;
        if (Symbol is PyName)
        {
            var symbol1 = Symbol;
            Advance();
            if (Symbol is PyColon)
            {
                var symbol2 = Symbol;
                Advance();
                var right = ParseExpression();

                return new TypeParameterTypedNode(pos.Item1, Position.Item1, symbol1, symbol2, right);
            }

            return new TypeParameterNode(pos.Item1, Position.Item1, symbol1);
        }
        else if (Symbol is PyMul)
        {
            var symbol1 = Symbol;
            Advance();
            if (Symbol is not PyName) throw new SyntaxError(Position.Item1, "Expecting NAME literal after '*' in parameter!");
            var symbol2 = Symbol;
            Advance();

            return new TypeStarParameterNode(pos.Item1, Position.Item1, symbol1, symbol2);
        }
        else if (Symbol is PyPower)
        {
            var symbol1 = Symbol;
            Advance();
            if (Symbol is not PyName) throw new SyntaxError(Position.Item1, "Expecting NAME literal after '**' in parameter!");
            var symbol2 = Symbol;
            Advance();

            return new TypePowerParameterNode(pos.Item1, Position.Item1, symbol1, symbol2);
        }
        else throw new SyntaxError(Position.Item1, "Expecting parameters like NAME, '*' or '**'!");
    }

    // Grammar rule: compound stmts //////////////////////////////////////////////////////////////////////////////////////
    public StatementNode ParseCompoundStmt() =>
        Symbol switch
        {
            PyMatch => ParseMatchStatement(),
            PyWhile => ParseWhileStatement(),
            PyTry => ParseTryStatement(),
            PyFor => ParseForStatement(),
            PyWith => ParseWithStatement(),
            PyClass => ParseClassStatement(),
            PyIf => ParseIfStatement(),
            PyAsync => ParseAsyncStatement(),
            PyDef => ParseDefStatement(),
            PyMatrice => ParseDecoratedStatement(),
            _ => throw new SyntaxError(Position.Item1, "Unknown or missing statement!")
        };

    // Grammar rule: match statement /////////////////////////////////////////////////////////////////////////////////
    public StatementNode ParseMatchStatement()
    {
        var pos = Position;
        if (Symbol is not PyMatch) throw new SyntaxError(Position.Item1, "Expecting 'match' in match statement!");
        var symbol1 = Symbol;
        Advance();

        var left = ParseStaredNameExpressions();

        if (Symbol is not PyColon) throw new SyntaxError(Position.Item1, "Expecting ':' in match statement!");
        var symbol2 = Symbol;
        Advance();

        if (Symbol is not PyNewline) throw new SyntaxError(Position.Item1, "Expecting block start in match statement!");
        var symbol3 = Symbol;
        Advance();

        if (Symbol is not PyIndent) throw new SyntaxError(Position.Item1, "Expecting block start in match statement!");
        var symbol4 = Symbol;
        Advance();

        var elements = new List<StatementNode>();

        do
        {
            elements.Add(ParseCaseBlock());
        } while (Symbol is PyName && (Symbol as PyName)!.Id == "case");

        if (Symbol is not PyDedent) throw new SyntaxError(Position.Item1, "Expecting block end in match statement!");
        var symbol5 = Symbol;
        Advance();

        return new MatchStatementNode(pos.Item1, Position.Item1, symbol1, left, symbol2, symbol3, symbol4, elements.ToArray(), symbol5);
    }

    private StatementNode ParseCaseBlock()
    {
        var pos = Position;
        if (Symbol is not PyName) throw new SyntaxError(Position.Item1, "Expecting 'case' in match statement!");
        if ((Symbol as PyName)!.Id != "case") throw new SyntaxError(Position.Item1, "Expecting 'case' in match statement!");

        var symbol1 = Symbol;
        Advance();

        var left = ParsePatterns();

        var guard = Symbol is PyIf ? ParseGuard() : null;

        if (Symbol is not PyColon) throw new SyntaxError(Position.Item1, "Expecting ':' in case element of match statement!");
        var symbol2 = Symbol;
        Advance();

        var right = ParseBlockStatement();

        return new MatchCaseStatementNode(pos.Item1, Position.Item1, 
                new PyCase(symbol1.StartPos, symbol1.EndPos, (symbol1 as PyName)!.Trivias), // Convert name with 'case' to a case symbol.
                left,
                guard,
                symbol2,
                right
            );
    }

    private StatementNode ParseGuard()
    {
        var pos = Position;
        if (Symbol is not PyIf) throw new SyntaxError(Position.Item1, "Expecting 'if' in guard statement!");
        var symbol1 = Symbol;
        Advance();

        var right = ParseNamedExpression();

        return new GuardStatementNode(pos.Item1, Position.Item1, symbol1, right);
    }

    private StatementNode ParsePatterns()
    {
        var pos = Position;

        if (Symbol is PyMul) ; // Fix here

        var left = ParseOrPatterns();

        if (Symbol is PyAs)
        {
            var symbol1 = Symbol;
            Advance();

            if (Symbol is PyName && (Symbol as PyName)!.Id != "_")
            {
                var symbol2 = Symbol;
                Advance();
                if (Symbol is PyDot || Symbol is PyLeftParen || Symbol is PyAssign)
                    throw new SyntaxError(Position.Item1, "Only a single name literal after 'as' in pattern!");

                return new MatchAsPatternNode(pos.Item1, Position.Item1, left, symbol1, symbol2);
            }

            throw new SyntaxError(Position.Item1, "Expecting name literal for 'as' part in pattern!");
        }

        return left;
    }

    private StatementNode ParseOrPatterns()
    {
        var pos = Position;
        var elements = new List<StatementNode>();
        var separators = new List<Symbol>();

        elements.Add(ParseClosedPattern());

        while (Symbol is PyBitOr)
        {
            separators.Add(Symbol);
            Advance();

            elements.Add(ParseClosedPattern());
        }

        return elements.Count == 1 ? elements[0] : new MatchOrPatternsNode(pos.Item1, Position.Item1, elements.ToArray(), separators.ToArray());
    }

    private StatementNode ParseClosedPattern()
    {
        var pos = Position;

        if (Symbol is PyString)
        {
            var symbol10 = Symbol;
            Advance();

            return new MatchStringCasePatternNode(pos.Item1, Position.Item1, symbol10);
        }
        else if (Symbol is PyNone)
        {
            var symbol20 = Symbol;
            Advance();

            return new MatchNoneCasePatternNode(pos.Item1, Position.Item1, symbol20);
        }
        else if (Symbol is PyTrue)
        {
            var symbol30 = Symbol;
            Advance();

            return new MatchTrueCasePatternNode(pos.Item1, Position.Item1, symbol30);
        }
        else if (Symbol is PyFalse)
        {
            var symbol40 = Symbol;
            Advance();

            return new MatchFalseCasePatternNode(pos.Item1, Position.Item1, symbol40);
        }
        else if (Symbol is PyNumber || Symbol is PyMinus)
        {
            if (Symbol is PyMinus)
            {
                var symbol50 = Symbol;
                Advance();
                if (Symbol is not PyNumber) throw new SyntaxError(Position.Item1, "Expecting number after '-' in number pattern!");

                var symbol51 = Symbol;
                Advance();

                return new MatchSignedNumberCasePatternNode(pos.Item1, Position.Item1, symbol50, symbol51);
            }

            var symbol52 = Symbol;
            Advance();

            if (Symbol is PyPlus || Symbol is PyMinus)
            {
                var symbol53 = Symbol;
                Advance();

                if (Symbol is not PyNumber) throw new SyntaxError(Position.Item1, "Expecting number in number pattern after '+' or '-'");

                var symbol54 = Symbol;
                Advance();

                return new MatchImaginaryNumberCasePatternNode(pos.Item1, Position.Item1, symbol52, symbol53, symbol54);
            }

            return new MatchNumberCasePatternNode(pos.Item1, Position.Item1, symbol52);
        }
        else if (Symbol is PyName)
        {
            if ((Symbol as PyName)!.Id == "_")
            {
                var symbolDefault = new PyDefault(Symbol.StartPos, Symbol.EndPos, (Symbol as PyName)!.Trivias);
                Advance();

                return new MatchDefaultCasePatternNode(pos.Item1, Position.Item1, symbolDefault);
            }

            var symbol60 = Symbol;
            Advance();

            if (Symbol is not PyDot && Symbol is not PyAssign && Symbol is not PyLeftParen)
            {
                return new MatchCaptureTargetCasePatternNode(pos.Item1, Position.Item1, symbol60);
            }

            StatementNode? left = null;

            if (Symbol is PyDot)
            {
                var elements = new List<Symbol>();
                var separators = new List<Symbol>();

                while (Symbol is PyDot)
                {
                    separators.Add(Symbol);
                    Advance();

                    if (Symbol is not PyName) throw new SyntaxError(Position.Item1, "Expecting NAME literaø after '.' in pattern!");
                    elements.Add(Symbol);
                    Advance();
                }

                left = new MatchDottedNameCasePatternNode(pos.Item1, Position.Item1, symbol60, elements.ToArray(), separators.ToArray());
            }

            if (Symbol is PyLeftParen) // Class pattern
            {

            }

            return left!;
        }
        else if (Symbol is PyLeftBracket || Symbol is PyLeftParen) // Sequence '(' can be group
        {

        }
        else if (Symbol is PyLeftCurly) // Mappings
        {
            var symbol80 = Symbol;
            Advance();

            if (Symbol is PyRightCurly)
            {
                var symbol81 = Symbol;
                Advance();

                return new MatchMappingCasePatternNode(pos.Item1, Position.Item1, symbol80, [], [], symbol81);
            }

            var elements = new List<StatementNode>();
            var separators = new List<Symbol>();

            if (Symbol is PyPower)
            {
                elements.Add(ParseDoubleStarPattern());
                if (Symbol is PyComma)
                {
                    separators.Add(Symbol);
                    Advance();
                }
            }
            else
            {
                elements.Add(ParseItemsPattern());

                while (Symbol is PyComma)
                {
                    separators.Add(Symbol);
                    Advance();

                    if (Symbol is PyRightCurly) break;
                    if (Symbol is PyPower)
                    {
                        elements.Add(ParseDoubleStarPattern());
                        if (Symbol is PyComma)
                        {
                            separators.Add(Symbol);
                            Advance();
                        }

                        break;
                    }

                    elements.Add(ParseItemsPattern());
                }
            }

            if (Symbol is not PyRightCurly) throw new SyntaxError(Position.Item1, "Expecting '}' in mapping pattern!");
            var symbol82 = Symbol;
            Advance();

            return new MatchMappingCasePatternNode(pos.Item1, Position.Item1, symbol80, elements.ToArray(), separators.ToArray(), symbol82);
        }

        throw new SyntaxError(Position.Item1, "Unknown pattern!");
    }

    private StatementNode ParseDoubleStarPattern()
    {
        var pos = Position;
        var symbol1 = Symbol;
        Advance();

        if (Symbol is not PyName) throw new SyntaxError(Position.Item1, "Expecting NAME literal after '**' in pattern!");
        var symbol2 = Symbol;
        Advance();

        if (Symbol is PyDot || Symbol is PyLeftParen || Symbol is PyAssign) throw new SyntaxError(Position.Item1, "Pattern '**' NAME is followed by illegal symbols!");

        return new DoubleStarPatternNode(pos.Item1, Position.Item1, symbol1, symbol2);
    }

    private StatementNode ParseItemsPattern()
    {
        var pos = Position;
        var left = ParseLiteralExpr();

        if (Symbol is not PyColon) throw new SyntaxError(Position.Item1, "Expecting ':' in key/value pattern!");
        var symbol1 = Symbol;
        Advance();

        var right = ParsePatterns();

        return new KeyValuePatternNode(pos.Item1, Position.Item1, left, symbol1, right);
    }

    private StatementNode ParseLiteralExpr()
    {
        var pos = Position;

        if (Symbol is PyString)
        {
            var symbol10 = Symbol;
            Advance();

            return new MatchStringCasePatternNode(pos.Item1, Position.Item1, symbol10);
        }
        else if (Symbol is PyNone)
        {
            var symbol20 = Symbol;
            Advance();

            return new MatchNoneCasePatternNode(pos.Item1, Position.Item1, symbol20);
        }
        else if (Symbol is PyTrue)
        {
            var symbol30 = Symbol;
            Advance();

            return new MatchTrueCasePatternNode(pos.Item1, Position.Item1, symbol30);
        }
        else if (Symbol is PyFalse)
        {
            var symbol40 = Symbol;
            Advance();

            return new MatchFalseCasePatternNode(pos.Item1, Position.Item1, symbol40);
        }
        else if (Symbol is PyNumber || Symbol is PyMinus)
        {
            if (Symbol is PyMinus)
            {
                var symbol50 = Symbol;
                Advance();
                if (Symbol is not PyNumber) throw new SyntaxError(Position.Item1, "Expecting number after '-' in number pattern!");

                var symbol51 = Symbol;
                Advance();

                return new MatchSignedNumberCasePatternNode(pos.Item1, Position.Item1, symbol50, symbol51);
            }

            var symbol52 = Symbol;
            Advance();

            if (Symbol is PyPlus || Symbol is PyMinus)
            {
                var symbol53 = Symbol;
                Advance();

                if (Symbol is not PyNumber) throw new SyntaxError(Position.Item1, "Expecting number in number pattern after '+' or '-'");

                var symbol54 = Symbol;
                Advance();

                return new MatchImaginaryNumberCasePatternNode(pos.Item1, Position.Item1, symbol52, symbol53, symbol54);
            }

            return new MatchNumberCasePatternNode(pos.Item1, Position.Item1, symbol52);
        }
        else if (Symbol is PyName)
        {
            var elements = new List<Symbol>();
            var separators = new List<Symbol>();

            var symbol60 = Symbol;
            Advance();

            while (Symbol is PyDot)
            {
                separators.Add(Symbol);
                Advance();

                if (Symbol is not PyName) throw new SyntaxError(Position.Item1, "Expecting NAME literal after '.' in pattern!");
                elements.Add(Symbol);
                Advance();
            }

            return new MatchDottedNameCasePatternNode(pos.Item1, Position.Item1, symbol60, elements.ToArray(), separators.ToArray());
        }

        throw new SyntaxError(Position.Item1, "Illegal pattern!");
    }




    // Grammar rule: while statement /////////////////////////////////////////////////////////////////////////////////
    public StatementNode ParseWhileStatement()
    {
        var pos = Position;

        if (Symbol is not PyWhile) throw new SyntaxError(Position.Item1, "Expecting 'while' in while statement!");
        var symbol1 = Symbol;
        Advance();

        var left = ParseNamedExpression();

        if (Symbol is not PyColon) throw new SyntaxError(Position.Item1, "Expecting ':' in while statement!");
        var symbol2 = Symbol;
        Advance();

        var right = ParseBlockStatement();

        var elsePart = Symbol is PyElse ? ParseElseStatement() : null;

        return new WhileStatementNode(pos.Item1, Position.Item1, symbol1, left, symbol2, right, elsePart);
    }

    // Grammar rule: try statement ///////////////////////////////////////////////////////////////////////////////////
    public StatementNode ParseTryStatement()
    {
        var pos = Position;
        _isStarExcept = false;
        _seenDefaultExcept = false;

        if (Symbol is not PyTry) throw new SyntaxError(Position.Item1, "Expecting 'try' for try statement!");
        var symbol1 = Symbol;
        Advance();

        if (Symbol is not PyColon) throw new SyntaxError(Position.Item1, "Expecting ':' for try statement!");
        var symbol2 = Symbol;
        Advance();

        var left = ParseBlockStatement();

        if (Symbol is PyFinally)
        {
            var right = ParseFinallyBlock();

            return new TryFinallyStatementBlockNode(pos.Item1, Position.Item1, symbol1, symbol2, left, right);
        }

        if (Symbol is not PyExcept) throw new SyntaxError(Position.Item1, "Must have one or more 'except' statement, when you dont have 'finally'!");
        var elements = new List<StatementNode>();
        while (Symbol is PyExcept) elements.Add(ParseExceptBlock());

        var elsePart = Symbol is PyElse ? ParseElseStatement() : null;

        var finallyPart = Symbol is PyFinally ? ParseFinallyBlock() : null; 

        return new TryExceptFinallyStatementBlockNode(pos.Item1, Position.Item1, symbol1, symbol2, left, elements.ToArray(), elsePart, finallyPart);
    }

    private StatementNode ParseExceptBlock()
    {
        var pos = Position;
        if (_seenDefaultExcept) throw new SyntaxError(Position.Item1, "Only one default except statement allowed in try statement!");

        if (Symbol is not PyExcept) throw new SyntaxError(Position.Item1, "Expecting 'except' for except statement!");
        var symbol1 = Symbol;
        Advance();

        if (Symbol is PyMul)
        {
            _isStarExcept = true;
            var symbol2 = Symbol;
            Advance();

            var left = ParseExpression();

            if (Symbol is PyAs)
            {
                var symbol3 = Symbol;
                Advance();

                if (Symbol is not PyName) throw new SyntaxError(Position.Item1, "Expecting NAME literal for as part of except statement!");
                var symbol4 = Symbol;
                Advance();

                if (Symbol is not PyColon) throw new SyntaxError(Position.Item1, "Expecting ':' for except statement!");
                var symbol5 = Symbol;
                Advance();

                var right = ParseBlockStatement();

                return new StarExceptStatementNode(pos.Item1, Position.Item1, symbol1, symbol2, left, symbol3, symbol4, symbol5, right);
            }

            if (Symbol is not PyColon) throw new SyntaxError(Position.Item1, "Expecting ':' for except statement!");
            var symbol6 = Symbol;
            Advance();

            var right2 = ParseBlockStatement();

            _seenDefaultExcept = true;

            return new StarExceptStatementNode(pos.Item1, Position.Item1, symbol1, symbol2, left, null, null, symbol6, right2);
        }
        else if (Symbol is PyColon)
        {
            var symbol2 = Symbol;
            Advance();

            var right = ParseBlockStatement();

            return new DefaultExceptStatementNode(pos.Item1, Position.Item1, symbol1, symbol2, right );
        }
        else
        {
            if (_isStarExcept == true) throw new SyntaxError(Position.Item1, "Mixing star except and except is not allowed!");

            var left = ParseExpression();

            if (Symbol is PyAs)
            {
                var symbol3 = Symbol;
                Advance();

                if (Symbol is not PyName) throw new SyntaxError(Position.Item1, "Expecting NAME literal for as part of except statement!");
                var symbol4 = Symbol;
                Advance();

                if (Symbol is not PyColon) throw new SyntaxError(Position.Item1, "Expecting ':' for except statement!");
                var symbol5 = Symbol;
                Advance();

                var right = ParseBlockStatement();

                return new ExceptStatementNode(pos.Item1, Position.Item1, symbol1, left, symbol3, symbol4, symbol5, right);
            }

            if (Symbol is not PyColon) throw new SyntaxError(Position.Item1, "Expecting ':' for except statement!");
            var symbol6 = Symbol;
            Advance();

            var right2 = ParseBlockStatement();

            return new ExceptStatementNode(pos.Item1, Position.Item1, symbol1, left, null, null, symbol6, right2);
        }
    }

    private StatementNode ParseFinallyBlock()
    {
        var pos = Position;
        if (Symbol is not PyFinally) throw new SyntaxError(Position.Item1, "Expecting 'finally' for finally statement!");
        var symbol1 = Symbol;
        Advance();

        if (Symbol is not PyColon) throw new SyntaxError(Position.Item1, "Expecting ':' for finally statement!");
        var symbol2 = Symbol;
        Advance();

        var right = ParseBlockStatement();

        return new FinallyStatementNode(pos.Item1, Position.Item1, symbol1, symbol2, right);
    }

    // Grammar rule: for statement ///////////////////////////////////////////////////////////////////////////////////
    public StatementNode ParseForStatement()
    {
        throw new NotImplementedException();
    }

    // Grammar rule: with statement //////////////////////////////////////////////////////////////////////////////////
    public StatementNode ParseWithStatement()
    {
        throw new NotImplementedException();
    }

    // Grammar rule: class statement /////////////////////////////////////////////////////////////////////////////////
    public StatementNode ParseClassStatement()
    {
        throw new NotImplementedException();
    }

    // Grammar rule: if statement ////////////////////////////////////////////////////////////////////////////////////
    public StatementNode ParseIfStatement()
    {
        var pos = Position;
        if (Symbol is not PyIf) throw new SyntaxError(Position.Item1, "Expecting 'if' in if statement!");
        var symbol1 = Symbol;
        Advance();

        var left = ParseNamedExpression();

        if (Symbol is not PyColon) throw new SyntaxError(Position.Item1, "Expecting ':' in if statement!");
        var symbol2 = Symbol;
        Advance();

        var right = ParseBlockStatement();


        if (Symbol is PyElif || Symbol is PyElse)
        {
            var elements = new List<StatementNode>();
            while (Symbol is PyElif) elements.Add(ParseElifStatement());

            var elsePart = Symbol is PyElse ? ParseElseStatement() : null;

            return new IfStatementNode(pos.Item1, Position.Item1, symbol1, left, symbol2, right, elements.ToArray(), elsePart);
        }

        return new IfStatementNode(pos.Item1, Position.Item1, symbol1, left, symbol2, right, [], null);
    }

    private StatementNode ParseElifStatement()
    {
        var pos = Position;
        if (Symbol is not PyElif) throw new SyntaxError(Position.Item1, "Expecting 'elif' in elif statement!");
        var symbol1 = Symbol;
        Advance();

        var left = ParseNamedExpression();

        if (Symbol is not PyColon) throw new SyntaxError(Position.Item1, "Expecting ':' in elif statement!");
        var symbol2 = Symbol;
        Advance();

        var right = ParseBlockStatement();

        return new ElifStatementNode(pos.Item1, Position.Item1, symbol1, left, symbol2, right);
    }

    private StatementNode ParseElseStatement()
    {
        var pos = Position;
        if (Symbol is not PyElse) throw new SyntaxError(Position.Item1, "Expecting 'else' in else statement!");
        var symbol1 = Symbol;
        Advance();
        if (Symbol is not PyColon) throw new SyntaxError(Position.Item1, "Expecting ':' in else statement!");
        var symbol2 = Symbol;
        Advance();
        var right = ParseBlockStatement();

        return new ElseStatementNode(pos.Item1, Position.Item1, symbol1, symbol2, right);
    }

    // Grammar rule: async statement /////////////////////////////////////////////////////////////////////////////////
    public StatementNode ParseAsyncStatement()
    {
        throw new NotImplementedException();
    }

    // Grammar rule: def statement ///////////////////////////////////////////////////////////////////////////////////
    public StatementNode ParseDefStatement()
    {
        throw new NotImplementedException();
    }

    // Grammar rule: decorated statement /////////////////////////////////////////////////////////////////////////////
    public StatementNode ParseDecoratedStatement()
    {
        throw new NotImplementedException();
    }

    // Grammar rule: block statement ///////////////////////////////////////////////////////////////////////////////////
    public StatementNode ParseBlockStatement()
    {
        if (Symbol is PyNewline)
        {
            var pos = Position;
            var symbol1 = Symbol;
            Advance();

            var right = ParseStmts();

            if (Symbol is not PyDedent) throw new SyntaxError(Position.Item1, "Missing Dedent in code block!");
            var symbol2 = Symbol;
            Advance();

            return new BlockNode(pos.Item1, Position.Item1, symbol1, right, symbol2);
        }

        return ParseSimpleStmts();
    }





    // File input as start rule for parsing ////////////////////////////////////////////////////////////////////////////
    public StatementNode? ParseFileInput()
    {
        Advance();
        if (Symbol is PyEOF) return null; // Empty file returns null.

        return ParseStmts();
    }
}
