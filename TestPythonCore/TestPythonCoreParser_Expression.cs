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
    }
}