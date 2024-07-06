namespace PythonCore;
public sealed class PythonCoreParser
{

    private Symbol _symbol = new Symbol(0, 0);

    public Tuple<uint, uint> CurrentSymbolPosition { get; private set; } = new(0, 0); 



    private void Advance() { }


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

    private ElipsisLiteralNode ParseAtomName()
    {
        var pos = CurrentSymbolPosition;
        var res = new ElipsisLiteralNode(pos.Item1, pos.Item2, _symbol);
        Advance();
        return res;
    }

    private ElipsisLiteralNode ParseAtomNumber()
    {
        var pos = CurrentSymbolPosition;
        var res = new ElipsisLiteralNode(pos.Item1, pos.Item2, _symbol);
        Advance();
        return res;
    }

    private ElipsisLiteralNode ParseAtomString()
    {
        var pos = CurrentSymbolPosition;
        var res = new ElipsisLiteralNode(pos.Item1, pos.Item2, _symbol);
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
