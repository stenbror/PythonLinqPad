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

    [Fact]
    public void TestExpressionRulePrimaryEmptyCall()
    {
        var parser = new PythonCoreParser("x.__init__()\r\n");
        parser.Advance();
        var res = parser.ParsePrimaryExpression();

        var required = new PrimaryExpressionNode(
            0, 12,
            new NameLiteralNode(0, 1, new PyName(0, 1, "x", [])),
            [
                new DotNameNode(1, 10, new PyDot(1, 2, []), new PyName(2, 10, "__init__", [])),
                new CallNode(
                    10, 12,
                    new PyLeftParen(10, 11, []),
                    null,
                    new PyRightParen(11, 12, [])
                    )
            ]
        );

        Assert.Equivalent(required, res, strict: true);
    }

    [Fact]
    public void TestExpressionRulePrimaryEmptyIndex()
    {
        var parser = new PythonCoreParser("x.__init__[:]\r\n");
        parser.Advance();
        var res = parser.ParsePrimaryExpression();

        var required = new PrimaryExpressionNode(
            0, 13,
            new NameLiteralNode(0, 1, new PyName(0, 1, "x", [])),
            [
                new DotNameNode(1, 10, new PyDot(1, 2, []), new PyName(2, 10, "__init__", [])),
                new IndexNode(
                    10, 13,
                    new PyLeftBracket(10, 11, []),
                    new SlicesNode(11, 12, [
                        new SliceNode(11, 12, null, new PyColon(11, 12, []), null, null, null)
                    ], []),
                    new PyRightBracket(12, 13, [])
                )
            ]
        );

        Assert.Equivalent(required, res, strict: true);
    }

    [Fact]
    public void TestExpressionRulePrimaryEmptyTrippleIndex()
    {
        var parser = new PythonCoreParser("x.__init__[::]\r\n");
        parser.Advance();
        var res = parser.ParsePrimaryExpression();

        var required = new PrimaryExpressionNode(
            0, 14,
            new NameLiteralNode(0, 1, new PyName(0, 1, "x", [])),
            [
                new DotNameNode(1, 10, new PyDot(1, 2, []), new PyName(2, 10, "__init__", [])),
                new IndexNode(
                    10, 14,
                    new PyLeftBracket(10, 11, []),
                    new SlicesNode(11, 13, [
                        new SliceNode(11, 13, null, new PyColon(11, 12, []), null, new PyColon(12, 13, []), null)
                    ], []),
                    new PyRightBracket(13, 14, [])
                )
            ]
        );

        Assert.Equivalent(required, res, strict: true);
    }

    [Fact]
    public void TestExpressionRuleAwaitExpression()
    {
        var parser = new PythonCoreParser("await x\r\n");
        parser.Advance();
        var res = parser.ParseAwaitExpression();

        var required = new AwaitExpressionNode(
            0, 7,
            new PyAwait(0, 5, []),
            new NameLiteralNode(6, 7, new PyName(6,7, "x", [new WhiteSpaceTrivia(5, 6)]))
        );

        Assert.Equivalent(required, res, strict: true);
    }

    [Fact]
    public void TestExpressionRulePowerExpression()
    {
        var parser = new PythonCoreParser("a ** b\r\n");
        parser.Advance();
        var res = parser.ParsePowerExpression();

        var required = new PowerExpressionNode(
            0, 6,
            new NameLiteralNode(0, 1, new PyName(0, 1, "a", [])),
            new PyPower(2, 4, [ new WhiteSpaceTrivia(1, 2) ]),
            new NameLiteralNode(5, 6, new PyName(5, 6, "b", [new WhiteSpaceTrivia(4, 5) ]))
        );

        Assert.Equivalent(required, res, strict: true);
    }

    [Fact]
    public void TestExpressionRuleUnaryPlusExpression()
    {
        var parser = new PythonCoreParser("+a\r\n");
        parser.Advance();
        var res = parser.ParseFactorExpression();

        var required = new UnaryPlusExpressionNode(
            0, 2,
            new PyPlus(0, 1, []),
            new NameLiteralNode(1, 2, new PyName(1, 2, "a", []))
        );

        Assert.Equivalent(required, res, strict: true);
    }

    [Fact]
    public void TestExpressionRuleUnaryMinusExpression()
    {
        var parser = new PythonCoreParser("-a\r\n");
        parser.Advance();
        var res = parser.ParseFactorExpression();

        var required = new UnaryPlusExpressionNode(
            0, 2,
            new PyMinus(0, 1, []),
            new NameLiteralNode(1, 2, new PyName(1, 2, "a", []))
        );

        Assert.Equivalent(required, res, strict: true);
    }

    [Fact]
    public void TestExpressionRuleUnaryBitInvertExpression()
    {
        var parser = new PythonCoreParser("~a\r\n");
        parser.Advance();
        var res = parser.ParseFactorExpression();

        var required = new UnaryPlusExpressionNode(
            0, 2,
            new PyBitInvert(0, 1, []),
            new NameLiteralNode(1, 2, new PyName(1, 2, "a", []))
        );

        Assert.Equivalent(required, res, strict: true);
    }

    [Fact]
    public void TestExpressionRuleMulExpressionSingle()
    {
        var parser = new PythonCoreParser("a * b\r\n");
        parser.Advance();
        var res = parser.ParseTermExpression();

        var required = new MulExpressionNode(
            0, 5,
            new NameLiteralNode(0, 1, new PyName(0, 1, "a", [])),
            new PyMul(2, 3, [ new WhiteSpaceTrivia(1, 2) ]),
            new NameLiteralNode(4, 5, new PyName(4, 5, "b", [new WhiteSpaceTrivia(3, 4)]))
        );

        Assert.Equivalent(required, res, strict: true);
    }

    [Fact]
    public void TestExpressionRuleDivExpressionSingle()
    {
        var parser = new PythonCoreParser("a / b\r\n");
        parser.Advance();
        var res = parser.ParseTermExpression();

        var required = new DivExpressionNode(
            0, 5,
            new NameLiteralNode(0, 1, new PyName(0, 1, "a", [])),
            new PyDiv(2, 3, [new WhiteSpaceTrivia(1, 2)]),
            new NameLiteralNode(4, 5, new PyName(4, 5, "b", [new WhiteSpaceTrivia(3, 4)]))
        );

        Assert.Equivalent(required, res, strict: true);
    }

    [Fact]
    public void TestExpressionRuleModuloExpressionSingle()
    {
        var parser = new PythonCoreParser("a % b\r\n");
        parser.Advance();
        var res = parser.ParseTermExpression();

        var required = new ModuloExpressionNode(
            0, 5,
            new NameLiteralNode(0, 1, new PyName(0, 1, "a", [])),
            new PyModulo(2, 3, [new WhiteSpaceTrivia(1, 2)]),
            new NameLiteralNode(4, 5, new PyName(4, 5, "b", [new WhiteSpaceTrivia(3, 4)]))
        );

        Assert.Equivalent(required, res, strict: true);
    }

    [Fact]
    public void TestExpressionRuleMatriceExpressionSingle()
    {
        var parser = new PythonCoreParser("a @ b\r\n");
        parser.Advance();
        var res = parser.ParseTermExpression();

        var required = new MatriceExpressionNode(
            0, 5,
            new NameLiteralNode(0, 1, new PyName(0, 1, "a", [])),
            new PyMatrice(2, 3, [new WhiteSpaceTrivia(1, 2)]),
            new NameLiteralNode(4, 5, new PyName(4, 5, "b", [new WhiteSpaceTrivia(3, 4)]))
        );

        Assert.Equivalent(required, res, strict: true);
    }

    [Fact]
    public void TestExpressionRuleFloorDivExpressionSingle()
    {
        var parser = new PythonCoreParser("a // b\r\n");
        parser.Advance();
        var res = parser.ParseTermExpression();

        var required = new FloorDivExpressionNode(
            0, 6,
            new NameLiteralNode(0, 1, new PyName(0, 1, "a", [])),
            new PyFloorDiv(2, 4, [new WhiteSpaceTrivia(1, 2)]),
            new NameLiteralNode(5, 6, new PyName(5, 6, "b", [new WhiteSpaceTrivia(4, 5)]))
        );

        Assert.Equivalent(required, res, strict: true);
    }

    [Fact]
    public void TestExpressionRuleModuloMulExpression()
    {
        var parser = new PythonCoreParser("a % b * c\r\n");
        parser.Advance();
        var res = parser.ParseTermExpression();

        var required = new MulExpressionNode(
            0, 9,
            new ModuloExpressionNode(0, 6,
                    new NameLiteralNode(0, 1, new PyName(0, 1, "a", [] )),
                    new PyModulo(2, 3, [ new WhiteSpaceTrivia(1, 2) ]),
                    new NameLiteralNode(4, 5, new PyName(4, 5, "b", [new WhiteSpaceTrivia(3, 4)]))
                ),
            new PyMul(6, 7, [new WhiteSpaceTrivia(5, 6)]),
            new NameLiteralNode(8, 9, new PyName(8, 9, "c", [new WhiteSpaceTrivia(7, 8)]))
        );

        Assert.Equivalent(required, res, strict: true);
    }

    [Fact]
    public void TestExpressionRulePlusExpressionSingle()
    {
        var parser = new PythonCoreParser("a + b\r\n");
        parser.Advance();
        var res = parser.ParseSumExpression();

        var required = new PlusExpressionNode(
            0, 5,
            new NameLiteralNode(0, 1, new PyName(0, 1, "a", [])),
            new PyPlus(2, 3, [new WhiteSpaceTrivia(1, 2)]),
            new NameLiteralNode(4, 5, new PyName(4, 5, "b", [new WhiteSpaceTrivia(3, 4)]))
        );

        Assert.Equivalent(required, res, strict: true);
    }

    [Fact]
    public void TestExpressionRuleMinusExpressionSingle()
    {
        var parser = new PythonCoreParser("a - b\r\n");
        parser.Advance();
        var res = parser.ParseSumExpression();

        var required = new MinusExpressionNode(
            0, 5,
            new NameLiteralNode(0, 1, new PyName(0, 1, "a", [])),
            new PyMinus(2, 3, [new WhiteSpaceTrivia(1, 2)]),
            new NameLiteralNode(4, 5, new PyName(4, 5, "b", [new WhiteSpaceTrivia(3, 4)]))
        );

        Assert.Equivalent(required, res, strict: true);
    }

    [Fact]
    public void TestExpressionRuleMinusPlusExpression()
    {
        var parser = new PythonCoreParser("a - b + c\r\n");
        parser.Advance();
        var res = parser.ParseSumExpression();

        var required = new PlusExpressionNode(
            0, 9,
            new MinusExpressionNode(0, 6,
                new NameLiteralNode(0, 1, new PyName(0, 1, "a", [])),
                new PyMinus(2, 3, [new WhiteSpaceTrivia(1, 2)]),
                new NameLiteralNode(4, 5, new PyName(4, 5, "b", [new WhiteSpaceTrivia(3, 4)]))
            ),
            new PyPlus(6, 7, [new WhiteSpaceTrivia(5, 6)]),
            new NameLiteralNode(8, 9, new PyName(8, 9, "c", [new WhiteSpaceTrivia(7, 8)]))
        );

        Assert.Equivalent(required, res, strict: true);
    }

    [Fact]
    public void TestExpressionRuleShiftLeftExpressionSingle()
    {
        var parser = new PythonCoreParser("a << b\r\n");
        parser.Advance();
        var res = parser.ParseShiftExpression();

        var required = new ShiftLeftExpressionNode(
            0, 6,
            new NameLiteralNode(0, 1, new PyName(0, 1, "a", [])),
            new PyShiftLeft(2, 4, [new WhiteSpaceTrivia(1, 2)]),
            new NameLiteralNode(5, 6, new PyName(5, 6, "b", [new WhiteSpaceTrivia(4, 5)]))
        );

        Assert.Equivalent(required, res, strict: true);
    }

    [Fact]
    public void TestExpressionRuleShiftRightExpressionSingle()
    {
        var parser = new PythonCoreParser("a >> b\r\n");
        parser.Advance();
        var res = parser.ParseShiftExpression();

        var required = new ShiftLeftExpressionNode(
            0, 6,
            new NameLiteralNode(0, 1, new PyName(0, 1, "a", [])),
            new PyShiftRight(2, 4, [new WhiteSpaceTrivia(1, 2)]),
            new NameLiteralNode(5, 6, new PyName(5, 6, "b", [new WhiteSpaceTrivia(4, 5)]))
        );

        Assert.Equivalent(required, res, strict: true);
    }

    [Fact]
    public void TestExpressionRuleShiftRightLeftsExpression()
    {
        var parser = new PythonCoreParser("a >> b << c\r\n");
        parser.Advance();
        var res = parser.ParseShiftExpression();

        var required = new ShiftLeftExpressionNode(
            0, 11,
            new MinusExpressionNode(0, 7,
                new NameLiteralNode(0, 1, new PyName(0, 1, "a", [])),
                new PyShiftRight(2, 4, [new WhiteSpaceTrivia(1, 2)]),
                new NameLiteralNode(5, 6, new PyName(5, 6, "b", [new WhiteSpaceTrivia(4, 5)]))
            ),
            new PyShiftLeft(7, 9, [new WhiteSpaceTrivia(6, 7)]),
            new NameLiteralNode(10, 11, new PyName(10, 11, "c", [new WhiteSpaceTrivia(9, 10)]))
        );

        Assert.Equivalent(required, res, strict: true);
    }

    [Fact]
    public void TestExpressionRuleBitwiseAndExpressionSingle()
    {
        var parser = new PythonCoreParser("a & b\r\n");
        parser.Advance();
        var res = parser.ParseBitwiseAndExpression();

        var required = new BitwiseAndExpressionNode(
            0, 5,
            new NameLiteralNode(0, 1, new PyName(0, 1, "a", [])),
            new PyBitAnd(2, 3, [new WhiteSpaceTrivia(1, 2)]),
            new NameLiteralNode(4, 5, new PyName(4, 5, "b", [new WhiteSpaceTrivia(3, 4)]))
        );

        Assert.Equivalent(required, res, strict: true);
    }

    [Fact]
    public void TestExpressionRuleBitwiseAndAndExpression()
    {
        var parser = new PythonCoreParser("a & b & c\r\n");
        parser.Advance();
        var res = parser.ParseBitwiseAndExpression();

        var required = new BitwiseAndExpressionNode(
            0, 9,
            new BitwiseAndExpressionNode(0, 6,
                new NameLiteralNode(0, 1, new PyName(0, 1, "a", [])),
                new PyBitAnd(2, 3, [new WhiteSpaceTrivia(1, 2)]),
                new NameLiteralNode(4, 5, new PyName(4, 5, "b", [new WhiteSpaceTrivia(3, 4)]))
            ),
            new PyAnd(6, 7, [new WhiteSpaceTrivia(5, 6)]),
            new NameLiteralNode(8, 9, new PyName(8, 9, "c", [new WhiteSpaceTrivia(7, 8)]))
        );

        Assert.Equivalent(required, res, strict: true);
    }

    [Fact]
    public void TestExpressionRuleBitwiseXorExpressionSingle()
    {
        var parser = new PythonCoreParser("a ^ b\r\n");
        parser.Advance();
        var res = parser.ParseBitwiseXorExpression();

        var required = new BitwiseXorExpressionNode(
            0, 5,
            new NameLiteralNode(0, 1, new PyName(0, 1, "a", [])),
            new PyBitXor(2, 3, [new WhiteSpaceTrivia(1, 2)]),
            new NameLiteralNode(4, 5, new PyName(4, 5, "b", [new WhiteSpaceTrivia(3, 4)]))
        );

        Assert.Equivalent(required, res, strict: true);
    }

    [Fact]
    public void TestExpressionRuleBitwiseXorXorExpression()
    {
        var parser = new PythonCoreParser("a ^ b ^ c\r\n");
        parser.Advance();
        var res = parser.ParseBitwiseXorExpression();

        var required = new BitwiseXorExpressionNode(
            0, 9,
            new BitwiseXorExpressionNode(0, 6,
                new NameLiteralNode(0, 1, new PyName(0, 1, "a", [])),
                new PyBitXor(2, 3, [new WhiteSpaceTrivia(1, 2)]),
                new NameLiteralNode(4, 5, new PyName(4, 5, "b", [new WhiteSpaceTrivia(3, 4)]))
            ),
            new PyBitXor(6, 7, [new WhiteSpaceTrivia(5, 6)]),
            new NameLiteralNode(8, 9, new PyName(8, 9, "c", [new WhiteSpaceTrivia(7, 8)]))
        );

        Assert.Equivalent(required, res, strict: true);
    }

    [Fact]
    public void TestExpressionRuleBitwiseOrExpressionSingle()
    {
        var parser = new PythonCoreParser("a | b\r\n");
        parser.Advance();
        var res = parser.ParseBitwiseOrExpression();

        var required = new BitwiseAndExpressionNode(
            0, 5,
            new NameLiteralNode(0, 1, new PyName(0, 1, "a", [])),
            new PyBitOr(2, 3, [new WhiteSpaceTrivia(1, 2)]),
            new NameLiteralNode(4, 5, new PyName(4, 5, "b", [new WhiteSpaceTrivia(3, 4)]))
        );

        Assert.Equivalent(required, res, strict: true);
    }

    [Fact]
    public void TestExpressionRuleBitwiseOrOrExpression()
    {
        var parser = new PythonCoreParser("a | b | c\r\n");
        parser.Advance();
        var res = parser.ParseBitwiseOrExpression();

        var required = new BitwiseOrExpressionNode(
            0, 9,
            new BitwiseOrExpressionNode(0, 6,
                new NameLiteralNode(0, 1, new PyName(0, 1, "a", [])),
                new PyBitOr(2, 3, [new WhiteSpaceTrivia(1, 2)]),
                new NameLiteralNode(4, 5, new PyName(4, 5, "b", [new WhiteSpaceTrivia(3, 4)]))
            ),
            new PyBitOr(6, 7, [new WhiteSpaceTrivia(5, 6)]),
            new NameLiteralNode(8, 9, new PyName(8, 9, "c", [new WhiteSpaceTrivia(7, 8)]))
        );

        Assert.Equivalent(required, res, strict: true);
    }

    [Fact]
    public void TestExpressionRuleLessExpressionSingle()
    {
        var parser = new PythonCoreParser("a < b\r\n");
        parser.Advance();
        var res = parser.ParseComparisonExpression();

        var required = new LessExpressionNode(
            0, 5,
            new NameLiteralNode(0, 1, new PyName(0, 1, "a", [])),
            new PyLess(2, 3, [new WhiteSpaceTrivia(1, 2)]),
            new NameLiteralNode(4, 5, new PyName(4, 5, "b", [new WhiteSpaceTrivia(3, 4)]))
        );

        Assert.Equivalent(required, res, strict: true);
    }

    [Fact]
    public void TestExpressionRuleLessEqualExpressionSingle()
    {
        var parser = new PythonCoreParser("a <= b\r\n");
        parser.Advance();
        var res = parser.ParseComparisonExpression();

        var required = new LessEqualExpressionNode(
            0, 6,
            new NameLiteralNode(0, 1, new PyName(0, 1, "a", [])),
            new PyLessEqual(2, 4, [new WhiteSpaceTrivia(1, 2)]),
            new NameLiteralNode(5, 6, new PyName(5, 6, "b", [new WhiteSpaceTrivia(4, 5)]))
        );

        Assert.Equivalent(required, res, strict: true);
    }

    [Fact]
    public void TestExpressionRuleGreaterExpressionSingle()
    {
        var parser = new PythonCoreParser("a > b\r\n");
        parser.Advance();
        var res = parser.ParseComparisonExpression();

        var required = new GreaterExpressionNode(
            0, 5,
            new NameLiteralNode(0, 1, new PyName(0, 1, "a", [])),
            new PyGreater(2, 3, [new WhiteSpaceTrivia(1, 2)]),
            new NameLiteralNode(4, 5, new PyName(4, 5, "b", [new WhiteSpaceTrivia(3, 4)]))
        );

        Assert.Equivalent(required, res, strict: true);
    }

    [Fact]
    public void TestExpressionRuleGreaterEqualExpressionSingle()
    {
        var parser = new PythonCoreParser("a >= b\r\n");
        parser.Advance();
        var res = parser.ParseComparisonExpression();

        var required = new GreaterEqualExpressionNode(
            0, 6,
            new NameLiteralNode(0, 1, new PyName(0, 1, "a", [])),
            new PyGreaterEqual(2, 4, [new WhiteSpaceTrivia(1, 2)]),
            new NameLiteralNode(5, 6, new PyName(5, 6, "b", [new WhiteSpaceTrivia(4, 5)]))
        );

        Assert.Equivalent(required, res, strict: true);
    }

    [Fact]
    public void TestExpressionRuleNotEqualExpressionSingle()
    {
        var parser = new PythonCoreParser("a != b\r\n");
        parser.Advance();
        var res = parser.ParseComparisonExpression();

        var required = new NotEqualExpressionNode(
            0, 6,
            new NameLiteralNode(0, 1, new PyName(0, 1, "a", [])),
            new PyNotEqual(2, 4, [new WhiteSpaceTrivia(1, 2)]),
            new NameLiteralNode(5, 6, new PyName(5, 6, "b", [new WhiteSpaceTrivia(4, 5)]))
        );

        Assert.Equivalent(required, res, strict: true);
    }

    [Fact]
    public void TestExpressionRuleEqualExpressionSingle()
    {
        var parser = new PythonCoreParser("a == b\r\n");
        parser.Advance();
        var res = parser.ParseComparisonExpression();

        var required = new EqualExpressionNode(
            0, 6,
            new NameLiteralNode(0, 1, new PyName(0, 1, "a", [])),
            new PyEqual(2, 4, [new WhiteSpaceTrivia(1, 2)]),
            new NameLiteralNode(5, 6, new PyName(5, 6, "b", [new WhiteSpaceTrivia(4, 5)]))
        );

        Assert.Equivalent(required, res, strict: true);
    }

    [Fact]
    public void TestExpressionRuleIsExpressionSingle()
    {
        var parser = new PythonCoreParser("a is b\r\n");
        parser.Advance();
        var res = parser.ParseComparisonExpression();

        var required = new IsExpressionNode(
            0, 6,
            new NameLiteralNode(0, 1, new PyName(0, 1, "a", [])),
            new PyIs(2, 4, [new WhiteSpaceTrivia(1, 2)]),
            new NameLiteralNode(5, 6, new PyName(5, 6, "b", [new WhiteSpaceTrivia(4, 5)]))
        );

        Assert.Equivalent(required, res, strict: true);
    }

    [Fact]
    public void TestExpressionRuleIsNotExpressionSingle()
    {
        var parser = new PythonCoreParser("a is not b\r\n");
        parser.Advance();
        var res = parser.ParseComparisonExpression();

        var required = new IsNotExpressionNode(
            0, 10,
            new NameLiteralNode(0, 1, new PyName(0, 1, "a", [])),
            new PyIs(2, 4, [new WhiteSpaceTrivia(1, 2)]),
            new PyNot(5, 8, [new WhiteSpaceTrivia(4, 5)]),
            new NameLiteralNode(9, 10, new PyName(9, 10, "b", [new WhiteSpaceTrivia(8, 9)]))
        );

        Assert.Equivalent(required, res, strict: true);
    }

    [Fact]
    public void TestExpressionRuleNotInExpressionSingle()
    {
        var parser = new PythonCoreParser("a not in b\r\n");
        parser.Advance();
        var res = parser.ParseComparisonExpression();

        var required = new NotInExpressionNode(
            0, 10,
            new NameLiteralNode(0, 1, new PyName(0, 1, "a", [])),
            new PyNot(2, 5, [new WhiteSpaceTrivia(1, 2)]),
            new PyIn(6, 8, [new WhiteSpaceTrivia(5, 6)]),
            new NameLiteralNode(9, 10, new PyName(9, 10, "b", [new WhiteSpaceTrivia(8, 9)]))
        );

        Assert.Equivalent(required, res, strict: true);
    }

    [Fact]
    public void TestExpressionRuleLessGreaterExpression()
    {
        var parser = new PythonCoreParser("a < b > c\r\n");
        parser.Advance();
        var res = parser.ParseComparisonExpression();

        var required = new GreaterExpressionNode(
            0, 9,
            new LessExpressionNode(0, 6,
                new NameLiteralNode(0, 1, new PyName(0, 1, "a", [])),
                new PyLess(2, 3, [new WhiteSpaceTrivia(1, 2)]),
                new NameLiteralNode(4, 5, new PyName(4, 5, "b", [new WhiteSpaceTrivia(3, 4)]))
            ),
            new PyGreater(6, 7, [new WhiteSpaceTrivia(5, 6)]),
            new NameLiteralNode(8, 9, new PyName(8, 9, "c", [new WhiteSpaceTrivia(7, 8)]))
        );

        Assert.Equivalent(required, res, strict: true);
    }

    [Fact]
    public void TestExpressionRuleInversionExpressionSingle()
    {
        var parser = new PythonCoreParser("not a\r\n");
        parser.Advance();
        var res = parser.ParseInversionExpression();

        var required = new NotExpressionNode(
            0, 5,
            new PyNot(0, 3, []),
            new NameLiteralNode(4, 5, new PyName(4, 5, "a", [new WhiteSpaceTrivia(3, 4)]))
        );

        Assert.Equivalent(required, res, strict: true);
    }

    [Fact]
    public void TestExpressionRuleAndExpressionSingle()
    {
        var parser = new PythonCoreParser("a and b\r\n");
        parser.Advance();
        var res = parser.ParseConjunctionExpression();

        var required = new AndExpressionNode(
            0, 7,
            new NameLiteralNode(0, 1, new PyName(0, 1, "a", [])),
            new PyAnd(2, 5, [new WhiteSpaceTrivia(1, 2)]),
            new NameLiteralNode(6, 7, new PyName(6, 7, "b", [new WhiteSpaceTrivia(5, 6)]))
        );

        Assert.Equivalent(required, res, strict: true);
    }

    [Fact]
    public void TestExpressionRuleOrExpressionSingle()
    {
        var parser = new PythonCoreParser("a or b\r\n");
        parser.Advance();
        var res = parser.ParseDisjunctionExpression();

        var required = new OrExpressionNode(
            0, 6,
            new NameLiteralNode(0, 1, new PyName(0, 1, "a", [])),
            new PyOr(2, 4, [new WhiteSpaceTrivia(1, 2)]),
            new NameLiteralNode(5, 6, new PyName(5, 6, "b", [new WhiteSpaceTrivia(4, 5)]))
        );

        Assert.Equivalent(required, res, strict: true);
    }

    [Fact]
    public void TestExpressionRuleConjunctionDisjunctionExpression()
    {
        var parser = new PythonCoreParser("a and b or c\r\n");
        parser.Advance();
        var res = parser.ParseDisjunctionExpression();

        var required = new OrExpressionNode(
            0, 12,
            new AndExpressionNode(0, 8,
                new NameLiteralNode(0, 1, new PyName(0, 1, "a", [])),
                new PyAnd(2, 5, [new WhiteSpaceTrivia(1, 2)]),
                new NameLiteralNode(6, 7, new PyName(6, 7, "b", [new WhiteSpaceTrivia(5, 6)]))
            ),
            new PyOr(8, 10, [new WhiteSpaceTrivia(7, 8)]),
            new NameLiteralNode(11, 12, new PyName(11, 12, "c", [new WhiteSpaceTrivia(10, 11)]))
        );

        Assert.Equivalent(required, res, strict: true);
    }

    [Fact]
    public void TestExpressionRuleExpressionEmpty()
    {
        var parser = new PythonCoreParser("a\r\n");
        parser.Advance();
        var res = parser.ParseExpression();

        var required = new NameLiteralNode(0, 1, new PyName(0, 1, "a", []));

        Assert.Equivalent(required, res, strict: true);
    }

    [Fact]
    public void TestExpressionRuleExpressionTest()
    {
        var parser = new PythonCoreParser("a if b else c\r\n");
        parser.Advance();
        var res = parser.ParseExpression();

        var required = new TestExpressionNode(
            0, 13,
            new NameLiteralNode(0, 1, new PyName(0, 1, "a", [])),
            new PyIf(2, 4, [new WhiteSpaceTrivia(1, 2)]),
            new NameLiteralNode(5, 6, new PyName(5, 6, "b", [new WhiteSpaceTrivia(4, 5)])),
            new PyElse(7, 11, [new WhiteSpaceTrivia(6, 7)]),
            new NameLiteralNode(12, 13, new PyName(12, 13, "c", [new WhiteSpaceTrivia(11, 12)]))
        );

        Assert.Equivalent(required, res, strict: true);
    }
}