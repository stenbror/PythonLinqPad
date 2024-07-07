using PythonCore;

namespace TestPythonCore;

public class TestPythonCoreParserLexicalAnalyzer
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

    [Fact]
    public void TestLexicalAnalyzerMinusAssign()
    {
        var parser = new PythonCoreParser("-=");
        parser.Advance();

        Assert.IsType<PyMinusAssign>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(2, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerArrow()
    {
        var parser = new PythonCoreParser("->");
        parser.Advance();

        Assert.IsType<PyArrow>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(2, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerMinus()
    {
        var parser = new PythonCoreParser("- ");
        parser.Advance();

        Assert.IsType<PyMinus>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(1, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerPowerAssign()
    {
        var parser = new PythonCoreParser("**=");
        parser.Advance();

        Assert.IsType<PyPowerAssign>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(3, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerPower()
    {
        var parser = new PythonCoreParser("** ");
        parser.Advance();

        Assert.IsType<PyPower>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(2, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerMulAssign()
    {
        var parser = new PythonCoreParser("*=");
        parser.Advance();

        Assert.IsType<PyMulAssign>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(2, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerFloorDivAssign()
    {
        var parser = new PythonCoreParser("//=");
        parser.Advance();

        Assert.IsType<PyFloorDivAssign>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(3, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerFloorDiv()
    {
        var parser = new PythonCoreParser("// ");
        parser.Advance();

        Assert.IsType<PyFloorDiv>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(2, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerDivAssign()
    {
        var parser = new PythonCoreParser("/=");
        parser.Advance();

        Assert.IsType<PyDivAssign>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(2, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerDiv()
    {
        var parser = new PythonCoreParser("/ ");
        parser.Advance();

        Assert.IsType<PyDiv>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(1, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerModuloAssign()
    {
        var parser = new PythonCoreParser("%=");
        parser.Advance();

        Assert.IsType<PyModuloAssign>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(2, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerModulo()
    {
        var parser = new PythonCoreParser("% ");
        parser.Advance();

        Assert.IsType<PyModulo>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(1, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerMatriceAssign()
    {
        var parser = new PythonCoreParser("@=");
        parser.Advance();

        Assert.IsType<PyMatriceAssign>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(2, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerMatrice()
    {
        var parser = new PythonCoreParser("@ ");
        parser.Advance();

        Assert.IsType<PyMatrice>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(1, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerBitAndAssign()
    {
        var parser = new PythonCoreParser("&=");
        parser.Advance();

        Assert.IsType<PyBitAndAssign>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(2, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerBitAnd()
    {
        var parser = new PythonCoreParser("& ");
        parser.Advance();

        Assert.IsType<PyBitAnd>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(1, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerBitOrAssign()
    {
        var parser = new PythonCoreParser("|=");
        parser.Advance();

        Assert.IsType<PyBitOrAssign>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(2, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerBitOr()
    {
        var parser = new PythonCoreParser("| ");
        parser.Advance();

        Assert.IsType<PyBitOr>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(1, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerBitXorAssign()
    {
        var parser = new PythonCoreParser("^=");
        parser.Advance();

        Assert.IsType<PyBitXorAssign>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(2, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerBitXor()
    {
        var parser = new PythonCoreParser("^ ");
        parser.Advance();

        Assert.IsType<PyBitXor>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(1, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerColonAssign()
    {
        var parser = new PythonCoreParser(":=");
        parser.Advance();

        Assert.IsType<PyColonAssign>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(2, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerColon()
    {
        var parser = new PythonCoreParser(": ");
        parser.Advance();

        Assert.IsType<PyColon>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(1, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerEqual()
    {
        var parser = new PythonCoreParser("==");
        parser.Advance();

        Assert.IsType<PyEqual>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(2, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerAssign()
    {
        var parser = new PythonCoreParser("= ");
        parser.Advance();

        Assert.IsType<PyAssign>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(1, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerNotEqual()
    {
        var parser = new PythonCoreParser("!=");
        parser.Advance();

        Assert.IsType<PyNotEqual>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(2, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerBitInvert()
    {
        var parser = new PythonCoreParser("~");
        parser.Advance();

        Assert.IsType<PyBitInvert>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(1, parser.Symbol.EndPos);
    }
}