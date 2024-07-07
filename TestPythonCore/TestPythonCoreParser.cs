using PythonCore;

namespace TestPythonCore;

public class UnitTest1
{
    [Fact]
    public void TestLexicalAnalyzerPlusAssign()
    {
        var parser = new PythonCoreParser("+=");
        parser.Advance();

        Assert.IsType<PyPlusAssign>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(2, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerPlus()
    {
        var parser = new PythonCoreParser("+ ");
        parser.Advance();

        Assert.IsType<PyPlus>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(1, parser.Symbol.EndPos);
    }
}