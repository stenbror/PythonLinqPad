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

    [Fact]
    public void TestLexicalAnalyzerShiftLeftAssign()
    {
        var parser = new PythonCoreParser("<<=");
        parser.Advance();

        Assert.IsType<PyShiftLeftAssign>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(3, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerShiftLeft()
    {
        var parser = new PythonCoreParser("<< ");
        parser.Advance();

        Assert.IsType<PyShiftLeft>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(2, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerLessEqual()
    {
        var parser = new PythonCoreParser("<=");
        parser.Advance();

        Assert.IsType<PyLessEqual>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(2, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerLess()
    {
        var parser = new PythonCoreParser("< ");
        parser.Advance();

        Assert.IsType<PyLess>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(1, parser.Symbol.EndPos);
    }







    [Fact]
    public void TestLexicalAnalyzerShiftRightAssign()
    {
        var parser = new PythonCoreParser(">>=");
        parser.Advance();

        Assert.IsType<PyShiftRightAssign>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(3, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerShiftRight()
    {
        var parser = new PythonCoreParser(">> ");
        parser.Advance();

        Assert.IsType<PyShiftRight>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(2, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerGreaterEqual()
    {
        var parser = new PythonCoreParser(">=");
        parser.Advance();

        Assert.IsType<PyGreaterEqual>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(2, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerGreater()
    {
        var parser = new PythonCoreParser("> ");
        parser.Advance();

        Assert.IsType<PyGreater>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(1, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerComma()
    {
        var parser = new PythonCoreParser(",");
        parser.Advance();

        Assert.IsType<PyComma>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(1, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerSemiColon()
    {
        var parser = new PythonCoreParser(";");
        parser.Advance();

        Assert.IsType<PySemiColon>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(1, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerDot()
    {
        var parser = new PythonCoreParser(". ");
        parser.Advance();

        Assert.IsType<PyDot>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(1, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerElipsis()
    {
        var parser = new PythonCoreParser("...");
        parser.Advance();

        Assert.IsType<PyElipsis>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(3, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerLeftParen()
    {
        var parser = new PythonCoreParser("(");
        parser.Advance();

        Assert.IsType<PyLeftParen>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(1, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerLeftBracket()
    {
        var parser = new PythonCoreParser("[");
        parser.Advance();

        Assert.IsType<PyLeftBracket>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(1, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerLeftCurly()
    {
        var parser = new PythonCoreParser("{");
        parser.Advance();

        Assert.IsType<PyLeftCurly>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(1, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerRightParen()
    {
        var parser = new PythonCoreParser("()");
        parser.Advance();
        parser.Advance();

        Assert.IsType<PyRightParen>(parser.Symbol);
        Assert.Equal(1, parser.Symbol.StartPos);
        Assert.Equal(2, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerRightBracket()
    {
        var parser = new PythonCoreParser("[]");
        parser.Advance();
        parser.Advance();

        Assert.IsType<PyRightBracket>(parser.Symbol);
        Assert.Equal(1, parser.Symbol.StartPos);
        Assert.Equal(2, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerRightCurly()
    {
        var parser = new PythonCoreParser("{}");
        parser.Advance();
        parser.Advance();

        Assert.IsType<PyRightCurly>(parser.Symbol);
        Assert.Equal(1, parser.Symbol.StartPos);
        Assert.Equal(2, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerReservedKeywordFalse()
    {
        var parser = new PythonCoreParser("False ");
        parser.Advance();
        
        Assert.IsType<PyFalse>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(5, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerReservedKeywordNone()
    {
        var parser = new PythonCoreParser("None ");
        parser.Advance();

        Assert.IsType<PyNone>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(4, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerReservedKeywordTrue()
    {
        var parser = new PythonCoreParser("True ");
        parser.Advance();

        Assert.IsType<PyTrue>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(4, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerReservedKeywordAnd()
    {
        var parser = new PythonCoreParser("and ");
        parser.Advance();

        Assert.IsType<PyAnd>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(3, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerReservedKeywordAs()
    {
        var parser = new PythonCoreParser("as ");
        parser.Advance();

        Assert.IsType<PyAs>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(2, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerReservedKeywordAssert()
    {
        var parser = new PythonCoreParser("assert ");
        parser.Advance();

        Assert.IsType<PyAssert>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(6, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerReservedKeywordAsync()
    {
        var parser = new PythonCoreParser("async ");
        parser.Advance();

        Assert.IsType<PyAsync>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(5, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerReservedKeywordAwait()
    {
        var parser = new PythonCoreParser("await ");
        parser.Advance();

        Assert.IsType<PyAwait>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(5, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerReservedKeywordBreak()
    {
        var parser = new PythonCoreParser("break ");
        parser.Advance();

        Assert.IsType<PyBreak>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(5, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerReservedKeywordClass()
    {
        var parser = new PythonCoreParser("class ");
        parser.Advance();

        Assert.IsType<PyClass>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(5, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerReservedKeywordContinue()
    {
        var parser = new PythonCoreParser("continue ");
        parser.Advance();

        Assert.IsType<PyContinue>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(8, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerReservedKeywordDef()
    {
        var parser = new PythonCoreParser("def ");
        parser.Advance();

        Assert.IsType<PyDef>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(3, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerReservedKeywordDel()
    {
        var parser = new PythonCoreParser("del ");
        parser.Advance();

        Assert.IsType<PyDel>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(3, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerReservedKeywordElif()
    {
        var parser = new PythonCoreParser("elif ");
        parser.Advance();

        Assert.IsType<PyElif>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(4, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerReservedKeywordElse()
    {
        var parser = new PythonCoreParser("else ");
        parser.Advance();

        Assert.IsType<PyElse>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(4, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerReservedKeywordExcept()
    {
        var parser = new PythonCoreParser("except ");
        parser.Advance();

        Assert.IsType<PyExcept>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(6, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerReservedKeywordFinally()
    {
        var parser = new PythonCoreParser("finally ");
        parser.Advance();

        Assert.IsType<PyFinally>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(7, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerReservedKeywordFor()
    {
        var parser = new PythonCoreParser("for ");
        parser.Advance();

        Assert.IsType<PyFor>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(3, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerReservedKeywordFrom()
    {
        var parser = new PythonCoreParser("from ");
        parser.Advance();

        Assert.IsType<PyFrom>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(4, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerReservedKeywordGlobal()
    {
        var parser = new PythonCoreParser("global ");
        parser.Advance();

        Assert.IsType<PyGlobal>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(6, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerReservedKeywordIf()
    {
        var parser = new PythonCoreParser("if ");
        parser.Advance();

        Assert.IsType<PyIf>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(2, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerReservedKeywordImport()
    {
        var parser = new PythonCoreParser("import ");
        parser.Advance();

        Assert.IsType<PyImport>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(6, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerReservedKeywordIn()
    {
        var parser = new PythonCoreParser("in ");
        parser.Advance();

        Assert.IsType<PyIn>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(2, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerReservedKeywordIs()
    {
        var parser = new PythonCoreParser("is ");
        parser.Advance();

        Assert.IsType<PyIs>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(2, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerReservedKeywordLambda()
    {
        var parser = new PythonCoreParser("lambda ");
        parser.Advance();

        Assert.IsType<PyLambda>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(6, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerReservedKeywordNonLocal()
    {
        var parser = new PythonCoreParser("nonlocal ");
        parser.Advance();

        Assert.IsType<PyNonlocal>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(8, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerReservedKeywordNot()
    {
        var parser = new PythonCoreParser("not ");
        parser.Advance();

        Assert.IsType<PyNot>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(3, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerReservedKeywordOr()
    {
        var parser = new PythonCoreParser("or ");
        parser.Advance();

        Assert.IsType<PyOr>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(2, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerReservedKeywordPass()
    {
        var parser = new PythonCoreParser("pass ");
        parser.Advance();

        Assert.IsType<PyPass>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(4, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerReservedKeywordRaise()
    {
        var parser = new PythonCoreParser("raise ");
        parser.Advance();

        Assert.IsType<PyRaise>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(5, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerReservedKeywordReturn()
    {
        var parser = new PythonCoreParser("return ");
        parser.Advance();

        Assert.IsType<PyReturn>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(6, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerReservedKeywordTry()
    {
        var parser = new PythonCoreParser("try ");
        parser.Advance();

        Assert.IsType<PyTry>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(3, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerReservedKeywordWhile()
    {
        var parser = new PythonCoreParser("while ");
        parser.Advance();

        Assert.IsType<PyWhile>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(5, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerReservedKeywordWith()
    {
        var parser = new PythonCoreParser("with ");
        parser.Advance();

        Assert.IsType<PyWith>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(4, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerReservedKeywordYield()
    {
        var parser = new PythonCoreParser("yield ");
        parser.Advance();

        Assert.IsType<PyYield>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(5, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerLiteralName()
    {
        var parser = new PythonCoreParser("__init__ ");
        parser.Advance();

        Assert.IsType<PyName>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(8, parser.Symbol.EndPos);
        
        var text = parser.Symbol as PyName;
        Assert.Equal("__init__", text?.Id);
    }

    [Fact]
    public void TestLexicalAnalyzerLiteralNameWithNumber()
    {
        var parser = new PythonCoreParser("KeyValue1 ");
        parser.Advance();

        Assert.IsType<PyName>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(9, parser.Symbol.EndPos);

        var text = parser.Symbol as PyName;
        Assert.Equal("KeyValue1", text?.Id);
    }

    [Fact]
    public void TestLexicalAnalyzerLiteralBinaryNumber1()
    {
        var parser = new PythonCoreParser("0b_01_01 ");
        parser.Advance();

        Assert.IsType<PyNumber>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(8, parser.Symbol.EndPos);

        var text = parser.Symbol as PyNumber;
        Assert.Equal("0b_01_01", text?.Number);
    }

    [Fact]
    public void TestLexicalAnalyzerLiteralBinaryNumber2()
    {
        var parser = new PythonCoreParser("0B0101 ");
        parser.Advance();

        Assert.IsType<PyNumber>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(6, parser.Symbol.EndPos);

        var text = parser.Symbol as PyNumber;
        Assert.Equal("0B0101", text?.Number);
    }

    [Fact]
    public void TestLexicalAnalyzerLiteralOctetNumber1()
    {
        var parser = new PythonCoreParser("0o_71_17 ");
        parser.Advance();

        Assert.IsType<PyNumber>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(8, parser.Symbol.EndPos);

        var text = parser.Symbol as PyNumber;
        Assert.Equal("0o_71_17", text?.Number);
    }

    [Fact]
    public void TestLexicalAnalyzerLiteralOctetNumber2()
    {
        var parser = new PythonCoreParser("0O7117 ");
        parser.Advance();

        Assert.IsType<PyNumber>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(6, parser.Symbol.EndPos);

        var text = parser.Symbol as PyNumber;
        Assert.Equal("0O7117", text?.Number);
    }

    [Fact]
    public void TestLexicalAnalyzerLiteralHexNumber1()
    {
        var parser = new PythonCoreParser("0x_7f_1F ");
        parser.Advance();

        Assert.IsType<PyNumber>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(8, parser.Symbol.EndPos);

        var text = parser.Symbol as PyNumber;
        Assert.Equal("0x_7f_1F", text?.Number);
    }

    [Fact]
    public void TestLexicalAnalyzerLiteralHexNumber2()
    {
        var parser = new PythonCoreParser("0X7Fff ");
        parser.Advance();

        Assert.IsType<PyNumber>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(6, parser.Symbol.EndPos);

        var text = parser.Symbol as PyNumber;
        Assert.Equal("0X7Fff", text?.Number);
    }

    [Fact]
    public void TestLexicalAnalyzerLiteralDotNumber()
    {
        var parser = new PythonCoreParser(".5 ");
        parser.Advance();

        Assert.IsType<PyNumber>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(2, parser.Symbol.EndPos);

        var text = parser.Symbol as PyNumber;
        Assert.Equal(".5", text?.Number);
    }

    [Fact]
    public void TestLexicalAnalyzerLiteralDotNumberWithExponent()
    {
        var parser = new PythonCoreParser(".5e-34J ");
        parser.Advance();

        Assert.IsType<PyNumber>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(7, parser.Symbol.EndPos);

        var text = parser.Symbol as PyNumber;
        Assert.Equal(".5e-34J", text?.Number);
    }

    [Fact]
    public void TestLexicalAnalyzerLiteralDotNumberWithExponentAndSeparators()
    {
        var parser = new PythonCoreParser(".5e-3_4J ");
        parser.Advance();

        Assert.IsType<PyNumber>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(8, parser.Symbol.EndPos);

        var text = parser.Symbol as PyNumber;
        Assert.Equal(".5e-3_4J", text?.Number);
    }

    [Fact]
    public void TestLexicalAnalyzerLiteralZeroDotZero()
    {
        var parser = new PythonCoreParser("0.0 ");
        parser.Advance();

        Assert.IsType<PyNumber>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(3, parser.Symbol.EndPos);

        var text = parser.Symbol as PyNumber;
        Assert.Equal("0.0", text?.Number);
    }

    [Fact]
    public void TestLexicalAnalyzerLiteralZeroDotNonZero()
    {
        var parser = new PythonCoreParser("0.3_4_5e+4_5j ");
        parser.Advance();

        Assert.IsType<PyNumber>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(13, parser.Symbol.EndPos);

        var text = parser.Symbol as PyNumber;
        Assert.Equal("0.3_4_5e+4_5j", text?.Number);
    }

    [Fact]
    public void TestLexicalAnalyzerLiteralNonZero()
    {
        var parser = new PythonCoreParser("1_000_000 ");
        parser.Advance();

        Assert.IsType<PyNumber>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(9, parser.Symbol.EndPos);

        var text = parser.Symbol as PyNumber;
        Assert.Equal("1_000_000", text?.Number);
    }

    [Fact]
    public void TestLexicalAnalyzerLiteralNonZero2()
    {
        var parser = new PythonCoreParser("1 ");
        parser.Advance();

        Assert.IsType<PyNumber>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(1, parser.Symbol.EndPos);

        var text = parser.Symbol as PyNumber;
        Assert.Equal("1", text?.Number);
    }

    [Fact]
    public void TestLexicalAnalyzerLiteralNonZero3()
    {
        var parser = new PythonCoreParser("1.9 ");
        parser.Advance();

        Assert.IsType<PyNumber>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(3, parser.Symbol.EndPos);

        var text = parser.Symbol as PyNumber;
        Assert.Equal("1.9", text?.Number);
    }

    [Fact]
    public void TestLexicalAnalyzerLiteralNonZero4()
    {
        var parser = new PythonCoreParser("1.9E-3_4J ");
        parser.Advance();

        Assert.IsType<PyNumber>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(9, parser.Symbol.EndPos);

        var text = parser.Symbol as PyNumber;
        Assert.Equal("1.9E-3_4J", text?.Number);
    }

    [Fact]
    public void TestLexicalAnalyzerLiteralEmptySingleQuote()
    {
        var parser = new PythonCoreParser("'' ");
        parser.Advance();

        Assert.IsType<PyString>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(2, parser.Symbol.EndPos);

        var text = parser.Symbol as PyString;
        Assert.Equal("''", text?.Text);
    }

    [Fact]
    public void TestLexicalAnalyzerLiteralEmptyMultipleQuote()
    {
        var parser = new PythonCoreParser("\"\" ");
        parser.Advance();

        Assert.IsType<PyString>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(2, parser.Symbol.EndPos);

        var text = parser.Symbol as PyString;
        Assert.Equal("\"\"", text?.Text);
    }

    [Fact]
    public void TestLexicalAnalyzerLiteralSingleQuote()
    {
        var parser = new PythonCoreParser("'Hello, World!' ");
        parser.Advance();

        Assert.IsType<PyString>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(15, parser.Symbol.EndPos);

        var text = parser.Symbol as PyString;
        Assert.Equal("'Hello, World!'", text?.Text);
    }

    [Fact]
    public void TestLexicalAnalyzerLiteralMultipleQuote()
    {
        var parser = new PythonCoreParser("\"Hello, World!\" ");
        parser.Advance();

        Assert.IsType<PyString>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(15, parser.Symbol.EndPos);

        var text = parser.Symbol as PyString;
        Assert.Equal("\"Hello, World!\"", text?.Text);
    }

    [Fact]
    public void TestLexicalAnalyzerLiteralTrippleSingleQuote()
    {
        var parser = new PythonCoreParser("'''Hello, 'W'orld!''' ");
        parser.Advance();

        Assert.IsType<PyString>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(21, parser.Symbol.EndPos);

        var text = parser.Symbol as PyString;
        Assert.Equal("'''Hello, 'W'orld!'''", text?.Text);
    }

    [Fact]
    public void TestLexicalAnalyzerLiteralTrippleMultipleQuote()
    {
        var parser = new PythonCoreParser("\"\"\"Hello, \"W\"orld!\"\"\" ");
        parser.Advance();

        Assert.IsType<PyString>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(21, parser.Symbol.EndPos);

        var text = parser.Symbol as PyString;
        Assert.Equal("\"\"\"Hello, \"W\"orld!\"\"\"", text?.Text);
    }

    [Fact]
    public void TestLexicalAnalyzerLiteralQuoteWithPrefix()
    {
        var parser = new PythonCoreParser("Fr'Hello, World!' ");
        parser.Advance();

        Assert.IsType<PyString>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(17, parser.Symbol.EndPos);

        var text = parser.Symbol as PyString;
        Assert.Equal("Fr'Hello, World!'", text?.Text);
    }

    [Fact]
    public void TestLexicalAnalyzerWhitespace()
    {
        var parser = new PythonCoreParser("not in ");

        parser.Advance();

        Assert.IsType<PyNot>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(3, parser.Symbol.EndPos);

        parser.Advance();

        Assert.IsType<PyIn>(parser.Symbol);
        Assert.Equal(4, parser.Symbol.StartPos);
        Assert.Equal(6, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerLineContinuation()
    {
        var parser = new PythonCoreParser("not\\\r\nin ");

        parser.Advance();

        Assert.IsType<PyNot>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(3, parser.Symbol.EndPos);

        parser.Advance();

        Assert.IsType<PyIn>(parser.Symbol);
        Assert.Equal(6, parser.Symbol.StartPos);
        Assert.Equal(8, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerLineContinuationWithOperator()
    {
        var parser = new PythonCoreParser("5 +\\\r\n6 ");

        parser.Advance();

        Assert.IsType<PyNumber>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(1, parser.Symbol.EndPos);

        parser.Advance();

        Assert.IsType<PyPlus>(parser.Symbol);
        Assert.Equal(2, parser.Symbol.StartPos);
        Assert.Equal(3, parser.Symbol.EndPos);

        parser.Advance();

        Assert.IsType<PyNumber>(parser.Symbol);
        Assert.Equal(6, parser.Symbol.StartPos);
        Assert.Equal(7, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerTypeComment()
    {
        var parser = new PythonCoreParser("# type: int -> int * int\r\n ");

        parser.Advance();

        Assert.IsType<PyTypeString>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(26, parser.Symbol.EndPos);

        var text = parser.Symbol as PyTypeString;

        Assert.Equal("# type: int -> int * int\r\n", text?.Comment);
    }

    [Fact]
    public void TestLexicalAnalyzerComment()
    {
        var parser = new PythonCoreParser("# This is a simple comment!\r\npass ");

        parser.Advance();

        Assert.IsType<PyPass>(parser.Symbol);
        Assert.Equal(29, parser.Symbol.StartPos);
        Assert.Equal(33, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerNewline1()
    {
        var parser = new PythonCoreParser("pass\r\npass ");

        parser.Advance();

        Assert.IsType<PyPass>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(4, parser.Symbol.EndPos);

        parser.Advance();

        Assert.IsType<PyNewline>(parser.Symbol);
        Assert.Equal(4, parser.Symbol.StartPos);
        Assert.Equal(6, parser.Symbol.EndPos);

        var symb = parser.Symbol as PyNewline;
        Assert.Equal('\r', symb?.Ch1);
        Assert.Equal('\n', symb?.Ch2);

        parser.Advance();

        Assert.IsType<PyPass>(parser.Symbol);
        Assert.Equal(6, parser.Symbol.StartPos);
        Assert.Equal(10, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerNewline2()
    {
        var parser = new PythonCoreParser("pass\rpass ");

        parser.Advance();

        Assert.IsType<PyPass>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(4, parser.Symbol.EndPos);

        parser.Advance();

        Assert.IsType<PyNewline>(parser.Symbol);
        Assert.Equal(4, parser.Symbol.StartPos);
        Assert.Equal(5, parser.Symbol.EndPos);

        var symb = parser.Symbol as PyNewline;
        Assert.Equal('\r', symb?.Ch1);
        Assert.Equal(' ', symb?.Ch2);

        parser.Advance();

        Assert.IsType<PyPass>(parser.Symbol);
        Assert.Equal(5, parser.Symbol.StartPos);
        Assert.Equal(9, parser.Symbol.EndPos);
    }

    [Fact]
    public void TestLexicalAnalyzerNewline3()
    {
        var parser = new PythonCoreParser("pass\npass ");

        parser.Advance();

        Assert.IsType<PyPass>(parser.Symbol);
        Assert.Equal(0, parser.Symbol.StartPos);
        Assert.Equal(4, parser.Symbol.EndPos);

        parser.Advance();

        Assert.IsType<PyNewline>(parser.Symbol);
        Assert.Equal(4, parser.Symbol.StartPos);
        Assert.Equal(5, parser.Symbol.EndPos);

        var symb = parser.Symbol as PyNewline;
        Assert.Equal(' ', symb?.Ch1);
        Assert.Equal('\n', symb?.Ch2);

        parser.Advance();

        Assert.IsType<PyPass>(parser.Symbol);
        Assert.Equal(5, parser.Symbol.StartPos);
        Assert.Equal(9, parser.Symbol.EndPos);
    }
}