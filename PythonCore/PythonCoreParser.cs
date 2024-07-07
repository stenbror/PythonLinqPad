namespace PythonCore;
public sealed class PythonCoreParser(string sourceBuffer)
{

    private Symbol _symbol = new PyEOF(0);

    public Tuple<int, int> CurrentSymbolPosition { get; private set; } = new(0, 0);
    private String _buffer = sourceBuffer;
    private int _index = 0;
    private int _symbolStartPos = 0;


    private void Advance() { }

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
            _ => new PyName(_symbolStartPos, _index - _symbolStartPos, _buffer.Substring(_symbolStartPos, _index - _symbolStartPos))
        };
    


    // Grammar rule: Atom //////////////////////////////////////////////////////////////////////////////////////////////
    public ExpressionNode ParseAtom() => 
    
        _symbol switch
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
        var pos = CurrentSymbolPosition;
        var res = new NoneLiteralNode(pos.Item1, pos.Item2, _symbol);
        Advance();
        return res;
    }

    private FalseLiteralNode ParseAtomFalse()
    {
        var pos = CurrentSymbolPosition;
        var res = new FalseLiteralNode(pos.Item1, pos.Item2, _symbol);
        Advance();
        return res;
    }

    private TrueLiteralNode ParseAtomTrue()
    {
        var pos = CurrentSymbolPosition;
        var res = new TrueLiteralNode(pos.Item1, pos.Item2, _symbol);
        Advance();
        return res;
    }

    private ElipsisLiteralNode ParseAtomElipsis()
    {
        var pos = CurrentSymbolPosition;
        var res = new ElipsisLiteralNode(pos.Item1, pos.Item2, _symbol);
        Advance();
        return res;
    }

    private NameLiteralNode ParseAtomName()
    {
        var pos = CurrentSymbolPosition;
        var res = new NameLiteralNode(pos.Item1, pos.Item2, _symbol);
        Advance();
        return res;
    }

    private NumberLiteralNode ParseAtomNumber()
    {
        var pos = CurrentSymbolPosition;
        var res = new NumberLiteralNode(pos.Item1, pos.Item2, _symbol);
        Advance();
        return res;
    }

    private StringLiteralNode ParseAtomString()
    {
        var pos = CurrentSymbolPosition;
        var res = new StringLiteralNode(pos.Item1, pos.Item2, _symbol);
        Advance();
        return res;
    }

    private ElipsisLiteralNode ParseAtomTuple()
    {
        var pos = CurrentSymbolPosition;
        var res = new ElipsisLiteralNode(pos.Item1, pos.Item2, _symbol);
        Advance();
        return res;
    }

    private ElipsisLiteralNode ParseAtomList()
    {
        var pos = CurrentSymbolPosition;
        var res = new ElipsisLiteralNode(pos.Item1, pos.Item2, _symbol);
        Advance();
        return res;
    }

    private ElipsisLiteralNode ParseAtomSetOrDictionary()
    {
        var pos = CurrentSymbolPosition;
        var res = new ElipsisLiteralNode(pos.Item1, pos.Item2, _symbol);
        Advance();
        return res;
    }

}
