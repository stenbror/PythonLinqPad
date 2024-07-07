using PythonCore;

namespace TestPythonCore;

public class TestPythonCoreParser_LexicalAnalyzer
{
    [Fact]
    public void TestLexicalAnalyzerPlusAssign()
    {
        var parser = new PythonCoreParserLexicalAnalyzer("+=");
        parser.Advance();

        Assert.IsType<PyPlusAssign>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(2, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerPlus()
    {
        var parser = new PythonCoreParserLexicalAnalyzer("+ ");
        parser.Advance();

        Assert.IsType<PyPlus>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(1, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerMinusAssign()
    {
        var parser = new PythonCoreParserLexicalAnalyzer("-=");
        parser.Advance();

        Assert.IsType<PyMinusAssign>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(2, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerArrow()
    {
        var parser = new PythonCoreParserLexicalAnalyzer("->");
        parser.Advance();

        Assert.IsType<PyArrow>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(2, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerMinus()
    {
        var parser = new PythonCoreParserLexicalAnalyzer("- ");
        parser.Advance();

        Assert.IsType<PyMinus>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(1, parser.Symbol.EndPos);
    }
}