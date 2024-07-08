using PythonCore;

namespace TestPythonCore;


public class TestPythonCoreParserExpression
{
    [Fact]
    public void TestExpressionRuleAtomElipsis()
    {
        var parser = new PythonCoreParser("...");
        parser.Advance();
        var res = parser.ParseAtom();

        Assert.IsType<ElipsisLiteralNode>(res);
        Assert.Equal(0, res.StartPos);
        Assert.Equal(3, res.EndPos);
        Assert.IsType<PyElipsis>((res as ElipsisLiteralNode)?.Element);
    }

    [Fact]
    public void TestExpressionRuleAtomFalse()
    {
        var parser = new PythonCoreParser("False ");
        parser.Advance();
        var res = parser.ParseAtom();

        Assert.IsType<FalseLiteralNode>(res);
        Assert.Equal(0, res.StartPos);
        Assert.Equal(6, res.EndPos);
        Assert.IsType<PyFalse>((res as FalseLiteralNode)?.Element);
    }

    [Fact]
    public void TestExpressionRuleAtomNone()
    {
        var parser = new PythonCoreParser("None ");
        parser.Advance();
        var res = parser.ParseAtom();

        Assert.IsType<NoneLiteralNode>(res);
        Assert.Equal(0, res.StartPos);
        Assert.Equal(5, res.EndPos);
        Assert.IsType<PyNone>((res as NoneLiteralNode)?.Element);
    }

    [Fact]
    public void TestExpressionRuleAtomTrue()
    {
        var parser = new PythonCoreParser("True ");
        parser.Advance();
        var res = parser.ParseAtom();

        Assert.IsType<TrueLiteralNode>(res);
        Assert.Equal(0, res.StartPos);
        Assert.Equal(5, res.EndPos);
        Assert.IsType<PyTrue>((res as TrueLiteralNode)?.Element);
    }

    [Fact]
    public void TestExpressionRuleAtomName()
    {
        var parser = new PythonCoreParser("__init__ ");
        parser.Advance();
        var res = parser.ParseAtom();

        Assert.IsType<NameLiteralNode>(res);
        Assert.Equal(0, res.StartPos);
        Assert.Equal(9, res.EndPos);
        Assert.IsType<PyName>((res as NameLiteralNode)?.Element);
    }
}