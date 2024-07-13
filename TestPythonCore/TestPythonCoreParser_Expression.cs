﻿using PythonCore;

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
}