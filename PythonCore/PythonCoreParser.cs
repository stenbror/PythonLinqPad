namespace PythonCore;
public sealed class PythonCoreParser
{

    private Symbol _symbol = new Symbol(0, 0);


    // Grammar rule: Atom //////////////////////////////////////////////////////////////////////////////////////////////
    public ExpressionNode ParseAtom() => 
    
        _symbol switch
        {
            PyNone => ParseAtomNone(),
            PyFalse => ParseAtomFalse(),
            PyTrue => ParseAtomTrue(),
            _ => throw new NotImplementedException()
        };

    private NoneLiteralNode ParseAtomNone()
    {
        return new NoneLiteralNode(0, 0, _symbol);
    }

    private FalseLiteralNode ParseAtomFalse()
    {
        return new FalseLiteralNode(0, 0, _symbol);
    }

    private TrueLiteralNode ParseAtomTrue()
    {
        return new TrueLiteralNode(0, 0, _symbol);
    }

}
