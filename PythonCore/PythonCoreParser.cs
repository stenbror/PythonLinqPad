﻿using System;

namespace PythonCore;

public sealed class PythonCoreParser(string sourceBuffer)
{

    public Symbol Symbol { get; private set; } = new PyEOF(0);

    private readonly String _buffer = sourceBuffer;
    private int _index = 0;
    private int _symbolStartPos = 0;

    private Stack<char> _parenthsiStack = new Stack<char>();


    private Tuple<int, int> Position => new Tuple<int, int>(_symbolStartPos, _index);
    

    public void Advance()
    {
        try
        {
_again:
            _symbolStartPos = _index; // Save current position as start of next symbol to analyze

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
                        throw new Exception();
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
                    else if (Char.IsAsciiDigit(_buffer[_index])) break;
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
                if (_buffer[_index] == '0')
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
                                if (_buffer[_index] != '0' && _buffer[_index] != '1') throw new Exception();
                                
                                while (_buffer[_index] == '0' || _buffer[_index] == '1') _index++;

                                if (_buffer[_index] != '_') break;
                            }
                            if (char.IsAsciiDigit(_buffer[_index])) throw new Exception();
                            break;
                        case 'o':
                        case 'O':
                            _index++;
                            while (true)
                            {
                                if (_buffer[_index] == '_') _index++;
                                if (_buffer[_index] < '0' && _buffer[_index] > '7') throw new Exception();

                                while (_buffer[_index] >= '0' && _buffer[_index] <= '7') _index++;

                                if (_buffer[_index] != '_') break;
                            }
                            if (char.IsAsciiDigit(_buffer[_index])) throw new Exception();
                            break;
                        case 'x':
                        case 'X':
                            break;
                        default:
                            break;
                    }

                }
                else
                {

                }

                Symbol = new PyNumber(_symbolStartPos, _index, _buffer.Substring(_symbolStartPos, _index - _symbolStartPos));

                return;
            }

            /* String */
            if (_buffer[_index] == '"' || _buffer[_index] == '\'')
            {

                Symbol = new PyString(_symbolStartPos, _index, _buffer.Substring(_symbolStartPos, _index - _symbolStartPos));

                return;
            }

            /* Reserved keyword or name literal */
            if (_buffer[_index] == '_' || char.IsLetter(_buffer[_index]))
            {
                while (char.IsLetterOrDigit(_buffer[_index]) || _buffer[_index] == '_') _index++;

                Symbol = IsReservedKeyword();
                _index++;

                return;
            }

            /* Unknow character that is not a symbol */
            throw new Exception();

        }
        catch (Exception e)
        {
            Symbol = new PyEOF(_index);
        }

    }

    private Symbol IsReservedKeyword() =>
    
        _buffer.Substring(_symbolStartPos, _index - _symbolStartPos) switch
        {
            "False" => new PyFalse(_symbolStartPos, _index - _symbolStartPos),
            "None" => new PyNone(_symbolStartPos, _index - _symbolStartPos),
            "True" => new PyTrue(_symbolStartPos, _index - _symbolStartPos),
            "and" => new PyAnd(_symbolStartPos, _index - _symbolStartPos),
            "as" => new PyAs(_symbolStartPos, _index - _symbolStartPos),
            "assert" => new PyAssert(_symbolStartPos, _index - _symbolStartPos),
            "async" => new PyAsync(_symbolStartPos, _index - _symbolStartPos),
            "await" => new PyAwait(_symbolStartPos, _index - _symbolStartPos),
            "break" => new PyBreak(_symbolStartPos, _index - _symbolStartPos),
            "class" => new PyClass(_symbolStartPos, _index - _symbolStartPos),
            "continue" => new PyContinue(_symbolStartPos, _index - _symbolStartPos),
            "def" => new PyDef(_symbolStartPos, _index - _symbolStartPos),
            "del" => new PyDel(_symbolStartPos, _index - _symbolStartPos),
            "elif" => new PyElif(_symbolStartPos, _index - _symbolStartPos),
            "else" => new PyElse(_symbolStartPos, _index - _symbolStartPos),
            "except" => new PyExcept(_symbolStartPos, _index - _symbolStartPos),
            "finally" => new PyFinally(_symbolStartPos, _index - _symbolStartPos),
            "for" => new PyFor(_symbolStartPos, _index - _symbolStartPos),
            "from" => new PyFrom(_symbolStartPos, _index - _symbolStartPos),
            "global" => new PyGlobal(_symbolStartPos, _index - _symbolStartPos),
            "if" => new PyIf(_symbolStartPos, _index - _symbolStartPos),
            "import" => new PyImport(_symbolStartPos, _index - _symbolStartPos),
            "in" => new PyIn(_symbolStartPos, _index - _symbolStartPos),
            "is" => new PyIs(_symbolStartPos, _index - _symbolStartPos),
            "lambda" => new PyLambda(_symbolStartPos, _index - _symbolStartPos),
            "nonlocal" => new PyNonlocal(_symbolStartPos, _index - _symbolStartPos),
            "not" => new PyNot(_symbolStartPos, _index - _symbolStartPos),
            "or" => new PyOr(_symbolStartPos, _index - _symbolStartPos),
            "pass" => new PyPass(_symbolStartPos, _index - _symbolStartPos),
            "raise" => new PyRaise(_symbolStartPos, _index - _symbolStartPos),
            "return" => new PyReturn(_symbolStartPos, _index - _symbolStartPos),
            "try" => new PyTry(_symbolStartPos, _index - _symbolStartPos),
            "with" => new PyWith(_symbolStartPos, _index - _symbolStartPos),
            "while" => new PyWhile(_symbolStartPos, _index - _symbolStartPos),
            "yield" => new PyYield(_symbolStartPos, _index - _symbolStartPos),
            _ => new PyName(_symbolStartPos, _index - _symbolStartPos, _buffer.Substring(_symbolStartPos, _index - _symbolStartPos))
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
            _ => throw new NotImplementedException()
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

}
