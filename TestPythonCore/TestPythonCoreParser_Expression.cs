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
        Assert.Equal(5, res.EndPos);
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
        Assert.Equal(4, res.EndPos);
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
        Assert.Equal(4, res.EndPos);
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
        Assert.Equal(8, res.EndPos);
        Assert.IsType<PyName>((res as NameLiteralNode)?.Element);
    }







    [Fact]
    public void TestExpressionRulePrimaryDotName()
    {
        var parser = new PythonCoreParser("x.__init__\r\n");
        parser.Advance();
        var res = parser.ParsePrimaryExpression();

        var required = new PrimaryExpressionNode(
            0, 10, 
            new NameLiteralNode(0, 1, new PyName(0, 1, "x", [])),
            [
                new DotNameNode(1, 10, new PyDot(1, 2, []), new PyName(2, 10, "__init__", []))
            ]
            );

        Assert.Equivalent(required, res, strict: true);
    }

    [Fact]
    public void TestExpressionRulePrimaryDotNameTwice()
    {
        var parser = new PythonCoreParser("x.__init__.ax\r\n");
        parser.Advance();
        var res = parser.ParsePrimaryExpression();

        var required = new PrimaryExpressionNode(
            0, 13,
            new NameLiteralNode(0, 1, new PyName(0, 1, "x", [])),
            [
                new DotNameNode(1, 10, new PyDot(1, 2, []), new PyName(2, 10, "__init__", [])),
                new DotNameNode(10, 13, new PyDot(10, 11, []), new PyName(11, 13, "ax", []))
            ]
        );

        Assert.Equivalent(required, res, strict: true);
    }
}