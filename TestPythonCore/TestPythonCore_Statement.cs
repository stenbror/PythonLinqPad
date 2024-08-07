﻿using PythonCore;

namespace TestPythonCore
{
    public class TestPythonCoreStatement
    {
        [Fact]
        public void TestStatementBreak()
        {
            var parser = new PythonCoreParser("break\r\n\r\n");
            parser.Advance();
            var res = parser.ParseStmts();

            var required = new SimpleStmtsNode(0, 7,
                [new BreakStmtNode(0, 5, new PyBreak(0, 5, []))],
                [],
                new PyNewline(5, 7, '\r', '\n', [])
            );

            Assert.Equivalent(required, res, strict: true);
        }

        [Fact]
        public void TestStatementContinue()
        {
            var parser = new PythonCoreParser("continue\r\n\r\n");
            parser.Advance();
            var res = parser.ParseStmts();

            var required = new SimpleStmtsNode(0, 10,
                [new ContinueStmtNode(0, 8, new PyContinue(0, 8, []))],
                [],
                new PyNewline(8, 10, '\r', '\n', [])
            );

            Assert.Equivalent(required, res, strict: true);
        }

        [Fact]
        public void TestStatementPass()
        {
            var parser = new PythonCoreParser("pass\r\n\r\n");
            parser.Advance();
            var res = parser.ParseStmts();

            var required = new SimpleStmtsNode(0, 6,
                [new BreakStmtNode(0, 4, new PyBreak(0, 4, []))],
                [],
                new PyNewline(4, 6, '\r', '\n', [])
            );

            Assert.Equivalent(required, res, strict: true);
        }

        [Fact]
        public void TestStatementTypeAliasEmpty()
        {
            var parser = new PythonCoreParser("type test = run\r\n\r\n");
            parser.Advance();
            var res = parser.ParseStmts();

            var required = new SimpleStmtsNode(0, 17,
                [
                    new TypeAliasNode(0, 15,
                        new PyType(0, 4, []),
                        new PyName(5, 9, "test", [new WhiteSpaceTrivia(4, 5)]),
                        null,
                        new PyAssign(10, 11, [new WhiteSpaceTrivia(9, 10)]),
                        new NameLiteralNode(12, 15, new PyName(12, 15, "run", [new WhiteSpaceTrivia(11, 12)]))
                    )
                ],
                [],
                new PyNewline(15, 17, '\r', '\n', [])
            );

            Assert.Equivalent(required, res, strict: true);
        }

        [Fact]
        public void TestStatementTypeAliasSingle()
        {
            var parser = new PythonCoreParser("type test[a] = run\r\n\r\n");
            parser.Advance();
            var res = parser.ParseStmts();

            var required = new SimpleStmtsNode(0, 20,
                [
                    new TypeAliasNode(0, 18,
                        new PyType(0, 4, []),
                        new PyName(5, 9, "test", [new WhiteSpaceTrivia(4, 5)]),
                        new TypeParamsNode(9, 13,
                            new PyLeftBracket(9, 10, []),
                            new TypeParamSequenceNode(10, 11,
                                [
                                    new TypeParameterNode(10, 11, new PyName(10, 11, "a", []))
                                ],
                                []
                            ),
                            new PyRightBracket(11, 12, [])
                        ),
                        new PyAssign(13, 14, [new WhiteSpaceTrivia(12, 13)]),
                        new NameLiteralNode(15, 18, new PyName(15, 18, "run", [new WhiteSpaceTrivia(14, 15)]))
                    )
                ],
                [],
                new PyNewline(18, 20, '\r', '\n', [])
            );

            Assert.Equivalent(required, res, strict: true);
        }

        [Fact]
        public void TestStatementTypeAliasMultiple()
        {
            var parser = new PythonCoreParser("type test[a, *b, **c, d: e] = run\r\n\r\n");
            parser.Advance();
            var res = parser.ParseStmts();

            var node =
                ((((res as SimpleStmtsNode)?.Elements[0] as TypeAliasNode)?.Parameters as TypeParamsNode)?.Right as
                    TypeParamSequenceNode);
            Assert.Equal(10, node?.StartPos);
            Assert.Equal(26, node?.EndPos);

            var elements = node?.Elements;
            Assert.Equal(4, elements?.Length);
            Assert.IsType<TypeParameterNode>(elements?[0]);
            Assert.IsType<TypeStarParameterNode>(elements?[1]);
            Assert.IsType<TypePowerParameterNode>(elements?[2]);
            Assert.IsType<TypeParameterTypedNode>(elements?[3]);

            var element1 = elements?[1] as TypeStarParameterNode;
            Assert.Equivalent(new PyName(14, 15, "b", []), element1?.Name);

            var element2 = elements?[2] as TypePowerParameterNode;
            Assert.Equivalent(new PyName(19, 20, "c", []), element2?.Name);

            var element3 = elements?[3] as TypeParameterTypedNode;
            Assert.Equivalent(new PyName(22, 23, "d", [new WhiteSpaceTrivia(21, 22)]), element3?.Name);
            Assert.IsType<NameLiteralNode>(element3?.Right);

            var name = element3?.Right as NameLiteralNode;
            Assert.Equivalent(new PyName(25, 26, "e", [new WhiteSpaceTrivia(24, 25)]), name?.Element);

            var separators = node?.Separators;
            Assert.Equal(3, separators?.Length);
            Assert.Equivalent(new PyComma(11, 12, []), separators?[0]);
            Assert.Equivalent(new PyComma(15, 16, []), separators?[1]);
            Assert.Equivalent(new PyComma(20, 21, []), separators?[2]);
        }

        [Fact]
        public void TestStatementTypeAliasSingleWithComma()
        {
            var parser = new PythonCoreParser("type test[a,] = run\r\n\r\n");
            parser.Advance();
            var res = parser.ParseStmts();

            var node =
                ((((res as SimpleStmtsNode)?.Elements[0] as TypeAliasNode)?.Parameters as TypeParamsNode)?.Right as
                    TypeParamSequenceNode);
            Assert.Equal(10, node?.StartPos);
            Assert.Equal(12, node?.EndPos);

            var separators = node?.Separators;
            Assert.Equal(1, separators?.Length);
            Assert.Equivalent(new PyComma(11, 12, []), separators?[0]);
        }

        [Fact]
        public void TestStatementGlobalSingle()
        {
            var parser = new PythonCoreParser("global a\r\n\r\n");
            parser.Advance();
            var res = parser.ParseStmts();

            var required = new SimpleStmtsNode(0, 10,
                [
                    new GlobalNode(0, 8,
                        new PyGlobal(0, 6, []),
                        [new PyName(7, 8, "a", [new WhiteSpaceTrivia(6, 7)])],
                        []
                    )
                ],
                [],
                new PyNewline(8, 10, '\r', '\n', [])
            );

            Assert.Equivalent(required, res, strict: true);
        }

        [Fact]
        public void TestStatementGlobalMulti()
        {
            var parser = new PythonCoreParser("global a,b\r\n\r\n");
            parser.Advance();
            var res = parser.ParseStmts();

            var required = new SimpleStmtsNode(0, 12,
                [
                    new GlobalNode(0, 10,
                        new PyGlobal(0, 6, []),
                        [
                            new PyName(7, 8, "a", [new WhiteSpaceTrivia(6, 7)]),
                            new PyName(9, 10, "b", [])
                        ],
                        [
                            new PyComma(8, 9, [])
                        ]
                    )
                ],
                [],
                new PyNewline(10, 12, '\r', '\n', [])
            );

            Assert.Equivalent(required, res, strict: true);
        }

        [Fact]
        public void TestStatementNonlocalSingle()
        {
            var parser = new PythonCoreParser("nonlocal a\r\n\r\n");
            parser.Advance();
            var res = parser.ParseStmts();

            var required = new SimpleStmtsNode(0, 12,
                [
                    new NonlocalNode(0, 10,
                        new PyNonlocal(0, 8, []),
                        [new PyName(9, 10, "a", [new WhiteSpaceTrivia(8, 9)])],
                        []
                    )
                ],
                [],
                new PyNewline(10, 12, '\r', '\n', [])
            );

            Assert.Equivalent(required, res, strict: true);
        }

        [Fact]
        public void TestStatementNonlocalMulti()
        {
            var parser = new PythonCoreParser("nonlocal a,b\r\n\r\n");
            parser.Advance();
            var res = parser.ParseStmts();

            var required = new SimpleStmtsNode(0, 14,
                [
                    new NonlocalNode(0, 12,
                        new PyNonlocal(0, 8, []),
                        [
                            new PyName(9, 10, "a", [new WhiteSpaceTrivia(8, 9)]),
                            new PyName(11, 12, "b", [])
                        ],
                        [
                            new PyComma(10, 11, [])
                        ]
                    )
                ],
                [],
                new PyNewline(12, 14, '\r', '\n', [])
            );

            Assert.Equivalent(required, res, strict: true);
        }

        [Fact]
        public void TestStatementReturnEmpty()
        {
            var parser = new PythonCoreParser("return\r\n\r\n");
            parser.Advance();
            var res = parser.ParseStmts();

            var required = new SimpleStmtsNode(0, 8,
                [
                    new ReturnNode(0, 6,
                        new PyReturn(0, 6, []),
                        null
                    )
                ],
                [],
                new PyNewline(6, 8, '\r', '\n', [])
            );

            Assert.Equivalent(required, res, strict: true);
        }

        [Fact]
        public void TestStatementReturn()
        {
            var parser = new PythonCoreParser("return a\r\n\r\n");
            parser.Advance();
            var res = parser.ParseStmts();

            var required = new SimpleStmtsNode(0, 10,
                [
                    new ReturnNode(0, 8,
                        new PyReturn(0, 6, []),
                        new NameLiteralNode(7, 8, new PyName(7, 8, "a", [new WhiteSpaceTrivia(6, 7)]))
                    )
                ],
                [],
                new PyNewline(8, 10, '\r', '\n', [])
            );

            Assert.Equivalent(required, res, strict: true);
        }

        [Fact]
        public void TestStatementRaiseEmpty()
        {
            var parser = new PythonCoreParser("raise\r\n\r\n");
            parser.Advance();
            var res = parser.ParseStmts();

            var required = new SimpleStmtsNode(0, 7,
                [
                    new RaiseNode(0, 5,
                        new PyRaise(0, 5, [])
                    )
                ],
                [],
                new PyNewline(5, 7, '\r', '\n', [])
            );

            Assert.Equivalent(required, res, strict: true);
        }

        [Fact]
        public void TestStatementRaiseElement()
        {
            var parser = new PythonCoreParser("raise a\r\n\r\n");
            parser.Advance();
            var res = parser.ParseStmts();

            var required = new SimpleStmtsNode(0, 9,
                [
                    new RaiseElementNode(0, 7,
                        new PyRaise(0, 5, []),
                        new NameLiteralNode(6, 7, new PyName(6, 7, "a", [new WhiteSpaceTrivia(5, 6)]))
                    )
                ],
                [],
                new PyNewline(7, 9, '\r', '\n', [])
            );

            Assert.Equivalent(required, res, strict: true);
        }

        [Fact]
        public void TestStatementRaiseFrom()
        {
            var parser = new PythonCoreParser("raise a from b\r\n\r\n");
            parser.Advance();
            var res = parser.ParseStmts();

            var required = new SimpleStmtsNode(0, 16,
                [
                    new RaiseFromNode(0, 14,
                        new PyRaise(0, 5, []),
                        new NameLiteralNode(6, 7, new PyName(6, 7, "a", [new WhiteSpaceTrivia(5, 6)])),
                        new PyFrom(8, 12, [new WhiteSpaceTrivia(7, 8)]),
                        new NameLiteralNode(13, 14, new PyName(13, 14, "b", [new WhiteSpaceTrivia(12, 13)]))
                    )
                ],
                [],
                new PyNewline(14, 16, '\r', '\n', [])
            );

            Assert.Equivalent(required, res, strict: true);
        }

        [Fact]
        public void TestStatementYield()
        {
            var parser = new PythonCoreParser("yield a\r\n\r\n");
            parser.Advance();
            var res = parser.ParseStmts();

            var required = new SimpleStmtsNode(0, 9,
                [
                    new YieldStmtNode(0, 7,
                        new YieldExpressionNode(0, 7, new PyYield(0, 5, []),
                            new NameLiteralNode(6, 7, new PyName(6, 7, "a", [new WhiteSpaceTrivia(5, 6)])))
                    )
                ],
                [],
                new PyNewline(7, 9, '\r', '\n', [])
            );

            Assert.Equivalent(required, res, strict: true);
        }

        [Fact]
        public void TestStatementAssertSingle()
        {
            var parser = new PythonCoreParser("assert a\r\n\r\n");
            parser.Advance();
            var res = parser.ParseStmts();

            var required = new SimpleStmtsNode(0, 10,
                [
                    new AssertSingleNode(0, 8,
                        new PyAssert(0, 6, []),
                        new NameLiteralNode(7, 8, new PyName(7, 8, "a", [new WhiteSpaceTrivia(6, 7)]))
                    )
                ],
                [],
                new PyNewline(8, 10, '\r', '\n', [])
            );

            Assert.Equivalent(required, res, strict: true);
        }

        [Fact]
        public void TestStatementAssert()
        {
            var parser = new PythonCoreParser("assert a,b\r\n\r\n");
            parser.Advance();
            var res = parser.ParseStmts();

            var required = new SimpleStmtsNode(0, 12,
                [
                    new AssertNode(0, 10,
                        new PyAssert(0, 6, []),
                        new NameLiteralNode(7, 8, new PyName(7, 8, "a", [new WhiteSpaceTrivia(6, 7)])),
                        new PyComma(8, 9, []),
                        new NameLiteralNode(9, 10, new PyName(9, 10, "b", []))
                    )
                ],
                [],
                new PyNewline(10, 12, '\r', '\n', [])
            );

            Assert.Equivalent(required, res, strict: true);
        }

        [Fact]
        public void TestStatementImportName()
        {
            var parser = new PythonCoreParser("import a\r\n\r\n");
            parser.Advance();
            var res = parser.ParseStmts();

            var required = new SimpleStmtsNode(0, 10,
                [
                    new ImportNameNode(0, 8,
                        new PyImport(0, 6, []),
                        new DottedNameNode(7, 8, [
                            new PyName(7, 8, "a", [new WhiteSpaceTrivia(6, 7)])
                        ], [])
                    )
                ],
                [],
                new PyNewline(8, 10, '\r', '\n', [])
            );

            Assert.Equivalent(required, res, strict: true);
        }

        [Fact]
        public void TestStatementImportNameDouble()
        {
            var parser = new PythonCoreParser("import a,b\r\n\r\n");
            parser.Advance();
            var res = parser.ParseStmts();

            var required = new SimpleStmtsNode(0, 12,
                [
                    new ImportNameNode(0, 10,
                        new PyImport(0, 6, []),
                        new DottedAsNamesNode(7, 10, [
                            new DottedNameNode(7, 8, [new PyName(7, 8, "a", [new WhiteSpaceTrivia(6, 7)])], []),
                            new DottedNameNode(9, 10, [new PyName(9, 10, "b", [])], [])
                        ], [
                            new PyComma(8, 9, [])
                        ])
                    )
                ],
                [],
                new PyNewline(10, 12, '\r', '\n', [])
            );

            Assert.Equivalent(required, res, strict: true);
        }

        [Fact]
        public void TestStatementImportNameSingleDottedName()
        {
            var parser = new PythonCoreParser("import a.b.c\r\n\r\n");
            parser.Advance();
            var res = parser.ParseStmts();

            var required = new SimpleStmtsNode(0, 14,
                [
                    new ImportNameNode(0, 12,
                        new PyImport(0, 6, []),
                        new DottedNameNode(7, 12,
                        [
                            new PyName(7, 8, "a", [new WhiteSpaceTrivia(6, 7)]),
                            new PyName(9, 10, "b", []),
                            new PyName(11, 12, "c", [])
                        ], [
                            new PyDot(8, 9, []),
                            new PyDot(10, 11, [])
                        ])
                    )
                ],
                [],
                new PyNewline(12, 14, '\r', '\n', [])
            );

            Assert.Equivalent(required, res, strict: true);
        }

        [Fact]
        public void TestStatementImportNameSingleAs()
        {
            var parser = new PythonCoreParser("import a as b\r\n\r\n");
            parser.Advance();
            var res = parser.ParseStmts();

            var required = new SimpleStmtsNode(0, 15,
                [
                    new ImportNameNode(0, 13,
                        new PyImport(0, 6, []),
                        new DottedAsNameNode(7, 13,
                            new DottedNameNode(7, 9, [
                                new PyName(7, 8, "a", [new WhiteSpaceTrivia(6, 7)])
                            ], []),
                            new PyAs(9, 11, [new WhiteSpaceTrivia(8, 9)]),
                            new PyName(12, 13, "b", [new WhiteSpaceTrivia(11, 12)])
                        )
                    )
                ],
                [],
                new PyNewline(13, 15, '\r', '\n', [])
            );

            Assert.Equivalent(required, res, strict: true);
        }

        [Fact]
        public void TestStatementImportNameSingleAsAndElement()
        {
            var parser = new PythonCoreParser("import a as b, c\r\n\r\n");
            parser.Advance();
            var res = parser.ParseStmts();

            var required = new SimpleStmtsNode(0, 18,
                [
                    new ImportNameNode(0, 16,
                        new PyImport(0, 6, []),
                        new DottedAsNamesNode(7, 16, [
                            new DottedAsNameNode(7, 13,
                                new DottedNameNode(7, 9, [
                                    new PyName(7, 8, "a", [new WhiteSpaceTrivia(6, 7)])
                                ], []),
                                new PyAs(9, 11, [new WhiteSpaceTrivia(8, 9)]),
                                new PyName(12, 13, "b", [new WhiteSpaceTrivia(11, 12)])
                            ),

                            new DottedNameNode(15, 16,
                            [
                                new PyName(15, 16, "c", [new WhiteSpaceTrivia(14, 15)]),
                            ], [])
                        ], [
                            new PyComma(13, 14, [])
                        ])
                    )
                ],
                [],
                new PyNewline(16, 18, '\r', '\n', [])
            );

            Assert.Equivalent(required, res, strict: true);
        }

        [Fact]
        public void TestStatementImportFromName()
        {
            var parser = new PythonCoreParser("from . import a\r\n\r\n");
            parser.Advance();
            var res = parser.ParseStmts();

            var required = new SimpleStmtsNode(0, 17,
                [
                    new ImportFromStmtNode(0, 15,
                        new PyFrom(0, 4, []),
                        [
                            new PyDot(5, 6, [new WhiteSpaceTrivia(4, 5)])
                        ],
                        null,
                        new PyImport(7, 13, [new WhiteSpaceTrivia(6, 7)]),
                        null,
                        new ImportFromNode(14, 15,
                            new PyName(14, 15, "a", [new WhiteSpaceTrivia(13, 14)])
                        ),
                        null
                    )
                ],
                [],
                new PyNewline(15, 17, '\r', '\n', [])
            );

            Assert.Equivalent(required, res, strict: true);
        }

        [Fact]
        public void TestStatementImportFromNameAs()
        {
            var parser = new PythonCoreParser("from . import a as b\r\n\r\n");
            parser.Advance();
            var res = parser.ParseStmts();

            var required = new SimpleStmtsNode(0, 22,
                [
                    new ImportFromStmtNode(0, 20,
                        new PyFrom(0, 4, []),
                        [
                            new PyDot(5, 6, [new WhiteSpaceTrivia(4, 5)])
                        ],
                        null,
                        new PyImport(7, 13, [new WhiteSpaceTrivia(6, 7)]),
                        null,
                        new ImportFromAsNode(14, 20,
                            new PyName(14, 15, "a", [new WhiteSpaceTrivia(13, 14)]),
                            new PyAs(16, 18, [new WhiteSpaceTrivia(15, 16)]),
                            new PyName(19, 20, "b", [new WhiteSpaceTrivia(18, 19)])
                        ),
                        null
                    )
                ],
                [],
                new PyNewline(20, 22, '\r', '\n', [])
            );

            Assert.Equivalent(required, res, strict: true);
        }

        [Fact]
        public void TestStatementImportFromNameAsDouble()
        {
            var parser = new PythonCoreParser("from . import a as b, c\r\n\r\n");
            parser.Advance();
            var res = parser.ParseStmts();

            var required = new SimpleStmtsNode(0, 25,
                [
                    new ImportFromStmtNode(0, 23,
                        new PyFrom(0, 4, []),
                        [
                            new PyDot(5, 6, [new WhiteSpaceTrivia(4, 5)])
                        ],
                        null,
                        new PyImport(7, 13, [new WhiteSpaceTrivia(6, 7)]),
                        null,
                        new ImportFromAsNamesNode(14, 23, [
                            new ImportFromAsNode(14, 20,
                                new PyName(14, 15, "a", [new WhiteSpaceTrivia(13, 14)]),
                                new PyAs(16, 18, [new WhiteSpaceTrivia(15, 16)]),
                                new PyName(19, 20, "b", [new WhiteSpaceTrivia(18, 19)])),

                            new ImportFromNode(22, 23, new PyName(22, 23, "c", [new WhiteSpaceTrivia(21, 22)]))
                        ], [
                            new PyComma(20, 21, [])
                        ]),
                        null
                    )
                ],
                [],
                new PyNewline(23, 25, '\r', '\n', [])
            );

            Assert.Equivalent(required, res, strict: true);
        }

        [Fact]
        public void TestStatementImportFromMul()
        {
            var parser = new PythonCoreParser("from . import *\r\n\r\n");
            parser.Advance();
            var res = parser.ParseStmts();

            var required = new SimpleStmtsNode(0, 17,
                [
                    new ImportFromStmtNode(0, 15,
                        new PyFrom(0, 4, []),
                        [
                            new PyDot(5, 6, [new WhiteSpaceTrivia(4, 5)])
                        ],
                        null,
                        new PyImport(7, 13, [new WhiteSpaceTrivia(6, 7)]),
                        new PyMul(14, 15, [new WhiteSpaceTrivia(13, 14)]),
                        null,
                        null
                    )
                ],
                [],
                new PyNewline(15, 17, '\r', '\n', [])
            );

            Assert.Equivalent(required, res, strict: true);
        }

        [Fact]
        public void TestStatementImportFromParenthesis()
        {
            var parser = new PythonCoreParser("from . import (a)\r\n\r\n");
            parser.Advance();
            var res = parser.ParseStmts();

            var start = ((res as SimpleStmtsNode)?.Elements[0] as ImportFromStmtNode)?.Start;
            var right = ((res as SimpleStmtsNode)?.Elements[0] as ImportFromStmtNode)?.Right;
            var end = ((res as SimpleStmtsNode)?.Elements[0] as ImportFromStmtNode)?.End;

            Assert.Equivalent(new PyLeftParen(14, 15, [new WhiteSpaceTrivia(13, 14)]), start!);
            Assert.Equivalent(new ImportFromNode(15, 16, new PyName(15, 16, "a", [])), right);
            Assert.Equivalent(new PyRightParen(16, 17, []), end!);
        }

        [Fact]
        public void TestStatementImportFromMulWithMoreDots()
        {
            var parser = new PythonCoreParser("from .... import *\r\n\r\n");
            parser.Advance();
            var res = parser.ParseStmts();

            var required = new SimpleStmtsNode(0, 20,
                [
                    new ImportFromStmtNode(0, 18,
                        new PyFrom(0, 4, []),
                        [
                            new PyElipsis(5, 8, [new WhiteSpaceTrivia(4, 5)]),
                            new PyDot(8, 9, [])
                        ],
                        null,
                        new PyImport(10, 16, [new WhiteSpaceTrivia(9, 10)]),
                        new PyMul(17, 18, [new WhiteSpaceTrivia(16, 17)]),
                        null,
                        null
                    )
                ],
                [],
                new PyNewline(18, 20, '\r', '\n', [])
            );

            Assert.Equivalent(required, res, strict: true);
        }

        [Fact]
        public void TestStatementImportFromMulWithName()
        {
            var parser = new PythonCoreParser("from a import *\r\n\r\n");
            parser.Advance();
            var res = parser.ParseStmts();

            var required = new SimpleStmtsNode(0, 17,
                [
                    new ImportFromStmtNode(0, 15,
                        new PyFrom(0, 4, []),
                        [],
                        new DottedNameNode(5, 7, [
                            new PyName(5, 6, "a", [new WhiteSpaceTrivia(4, 5)])
                        ], []),
                        new PyImport(7, 13, [new WhiteSpaceTrivia(6, 7)]),
                        new PyMul(14, 15, [new WhiteSpaceTrivia(13, 14)]),
                        null,
                        null
                    )
                ],
                [],
                new PyNewline(15, 17, '\r', '\n', [])
            );

            Assert.Equivalent(required, res, strict: true);
        }

        [Fact]
        public void TestStatementImportFromMulWithDottedName()
        {
            var parser = new PythonCoreParser("from a.b import *\r\n\r\n");
            parser.Advance();
            var res = parser.ParseStmts();

            var left = ((res as SimpleStmtsNode)?.Elements[0] as ImportFromStmtNode)?.Left;

            Assert.IsType<DottedNameNode>(left!);

            Assert.Equivalent(new PyName(5, 6, "a", [new WhiteSpaceTrivia(4, 5)]),
                (left as DottedNameNode)?.Elements[0]);

            Assert.Equivalent(new PyName(7, 8, "b", []), (left as DottedNameNode)?.Elements[1]);
        }







        [Fact]
        public void TestStatementIfStatementSimple()
        {
            var parser = new PythonCoreParser("if a > 5: pass\r\n\r\n");
            parser.Advance();
            var res = parser.ParseStmts();

            var required = new IfStatementNode(0, 16,
                new PyIf(0, 2, []),
                new GreaterExpressionNode(3, 8,
                    new NameLiteralNode(3, 4, new PyName(3, 4, "a", [new WhiteSpaceTrivia(2, 3)])),
                    new PyGreater(5, 6, [new WhiteSpaceTrivia(4, 5)]),
                    new NumberLiteralNode(7, 8, new PyNumber(7, 8, "5", [new WhiteSpaceTrivia(6, 7)]))
                ),
                new PyColon(8, 9, []),
                new SimpleStmtsNode(10, 16, [
                    new PassStmtNode(10, 14, new PyPass(10, 14, [new WhiteSpaceTrivia(9, 10)]))
                ], [], new PyNewline(14, 16, '\r', '\n', [])),
                [],
                null
            );

            Assert.Equivalent(required, res, strict: true);
        }

        [Fact]
        public void TestStatementIfElseStatementSimple()
        {
            var parser = new PythonCoreParser("if a > 5: pass\r\nelse: pass\r\n\r\n");
            parser.Advance();
            var res = parser.ParseStmts();

            var required = new IfStatementNode(0, 28,
                new PyIf(0, 2, []),
                new GreaterExpressionNode(3, 8,
                    new NameLiteralNode(3, 4, new PyName(3, 4, "a", [new WhiteSpaceTrivia(2, 3)])),
                    new PyGreater(5, 6, [new WhiteSpaceTrivia(4, 5)]),
                    new NumberLiteralNode(7, 8, new PyNumber(7, 8, "5", [new WhiteSpaceTrivia(6, 7)]))
                ),
                new PyColon(8, 9, []),
                new SimpleStmtsNode(10, 16, [
                    new PassStmtNode(10, 14, new PyPass(10, 14, [new WhiteSpaceTrivia(9, 10)]))
                ], [], new PyNewline(14, 16, '\r', '\n', [])),
                [],
                new ElseStatementNode(16, 28,
                    new PyElse(16, 20, []),
                    new PyColon(20, 21, []),
                    new SimpleStmtsNode(22, 28, [
                        new PassStmtNode(22, 26, new PyPass(22, 26, [new WhiteSpaceTrivia(21, 22)]))
                    ], [], new PyNewline(26, 28, '\r', '\n', []))
                )

            );

            Assert.Equivalent(required, res, strict: true);
        }

        [Fact]
        public void TestStatementIfElifElseStatementSimple()
        {
            var parser = new PythonCoreParser("if a > 5: pass\r\nelif a < 6: pass\r\nelse: pass\r\n\r\n");
            parser.Advance();
            var res = parser.ParseStmts();

            var required = new IfStatementNode(0, 46,
                new PyIf(0, 2, []),
                new GreaterExpressionNode(3, 8,
                    new NameLiteralNode(3, 4, new PyName(3, 4, "a", [new WhiteSpaceTrivia(2, 3)])),
                    new PyGreater(5, 6, [new WhiteSpaceTrivia(4, 5)]),
                    new NumberLiteralNode(7, 8, new PyNumber(7, 8, "5", [new WhiteSpaceTrivia(6, 7)]))
                ),
                new PyColon(8, 9, []),
                new SimpleStmtsNode(10, 16, [
                    new PassStmtNode(10, 14, new PyPass(10, 14, [new WhiteSpaceTrivia(9, 10)]))
                ], [], new PyNewline(14, 16, '\r', '\n', [])),
                [
                    new ElifStatementNode(16, 34,
                        new PyElif(16, 20, []),
                        new LessExpressionNode(21, 26,
                            new NameLiteralNode(21, 22, new PyName(21, 22, "a", [new WhiteSpaceTrivia(20, 21)])),
                            new PyLess(23, 24, [new WhiteSpaceTrivia(22, 23)]),
                            new NumberLiteralNode(25, 26, new PyNumber(25, 26, "6", [new WhiteSpaceTrivia(24, 25)]))
                        ),
                        new PyColon(26, 27, []),
                        new SimpleStmtsNode(28, 34, [
                            new PassStmtNode(28, 32, new PyPass(28, 32, [new WhiteSpaceTrivia(27, 28)]))
                        ], [], new PyNewline(32, 34, '\r', '\n', []))
                    )
                ],
                new ElseStatementNode(34, 46,
                    new PyElse(34, 38, []),
                    new PyColon(38, 39, []),
                    new SimpleStmtsNode(40, 46, [
                        new PassStmtNode(40, 44, new PyPass(40, 44, [new WhiteSpaceTrivia(39, 40)]))
                    ], [], new PyNewline(44, 46, '\r', '\n', []))
                )
            );

            Assert.Equivalent(required, res, strict: true);
        }

        [Fact]
        public void TestStatementWhileStatementSimple()
        {
            var parser = new PythonCoreParser("while a > 5: pass\r\n\r\n");
            parser.Advance();
            var res = parser.ParseStmts();

            var required = new WhileStatementNode(0, 19,
                new PyWhile(0, 5, []),
                new GreaterExpressionNode(6, 11,
                    new NameLiteralNode(6, 7, new PyName(6, 7, "a", [new WhiteSpaceTrivia(5, 6)])),
                    new PyGreater(8, 9, [new WhiteSpaceTrivia(7, 8)]),
                    new NumberLiteralNode(10, 11, new PyNumber(10, 11, "5", [new WhiteSpaceTrivia(9, 10)]))
                ),
                new PyColon(11, 12, []),
                new SimpleStmtsNode(13, 19, [
                    new PassStmtNode(13, 17, new PyPass(13, 17, [new WhiteSpaceTrivia(12, 13)]))
                ], [], new PyNewline(17, 19, '\r', '\n', [])),
                null
            );

            Assert.Equivalent(required, res, strict: true);
        }

        [Fact]
        public void TestStatementWhileStatementWithElse()
        {
            var parser = new PythonCoreParser("while a > 5: pass\r\nelse: pass\r\n\r\n");
            parser.Advance();
            var res = parser.ParseStmts();

            var required = new WhileStatementNode(0, 31,
                new PyWhile(0, 5, []),
                new GreaterExpressionNode(6, 11,
                    new NameLiteralNode(6, 7, new PyName(6, 7, "a", [new WhiteSpaceTrivia(5, 6)])),
                    new PyGreater(8, 9, [new WhiteSpaceTrivia(7, 8)]),
                    new NumberLiteralNode(10, 11, new PyNumber(10, 11, "5", [new WhiteSpaceTrivia(9, 10)]))
                ),
                new PyColon(11, 12, []),
                new SimpleStmtsNode(13, 19, [
                    new PassStmtNode(13, 17, new PyPass(13, 17, [new WhiteSpaceTrivia(12, 13)]))
                ], [], new PyNewline(17, 19, '\r', '\n', [])),
                new ElseStatementNode(19, 31,
                    new PyElse(19, 23, []),
                    new PyColon(23, 24, []),
                    new SimpleStmtsNode(25, 31, [
                        new PassStmtNode(25, 29, new PyPass(25, 29, [new WhiteSpaceTrivia(24, 25)]))
                    ], [], new PyNewline(29, 31, '\r', '\n', []))
                )
            );

            Assert.Equivalent(required, res, strict: true);
        }

        [Fact]
        public void TestStatementTryFinallyStatement()
        {
            var parser = new PythonCoreParser("try: pass\r\nfinally: pass\r\n\r\n");
            parser.Advance();
            var res = parser.ParseStmts();

            var required = new TryFinallyStatementBlockNode(0, 26,
                new PyTry(0, 3, []),
                new PyColon(3, 4, []),
                new SimpleStmtsNode(5, 11, [
                    new PassStmtNode(5, 9, new PyPass(5, 9, [new WhiteSpaceTrivia(4, 5)]))
                ], [], new PyNewline(9, 11, '\r', '\n', [])),
                new FinallyStatementNode(11, 26,
                    new PyFinally(11, 18, []),
                    new PyColon(18, 19, []),
                    new SimpleStmtsNode(20, 26, [
                        new PassStmtNode(20, 24, new PyPass(20, 24, [new WhiteSpaceTrivia(19, 20)]))
                    ], [], new PyNewline(24, 26, '\r', '\n', []))
                )

            );

            Assert.Equivalent(required, res, strict: true);
        }

        [Fact]
        public void TestStatementTryExceptStatement()
        {
            var parser = new PythonCoreParser("try: pass\r\nexcept: pass\r\n\r\n");
            parser.Advance();
            var res = parser.ParseStmts();

            var required = new TryExceptFinallyStatementBlockNode(0, 25,
                new PyTry(0, 3, []),
                new PyColon(3, 4, []),
                new SimpleStmtsNode(5, 11, [
                    new PassStmtNode(5, 9, new PyPass(5, 9, [new WhiteSpaceTrivia(4, 5)]))
                ], [], new PyNewline(9, 11, '\r', '\n', [])),
                [
                    new DefaultExceptStatementNode(11, 25,
                        new PyExcept(11, 17, []),
                        new PyColon(17, 18, []),
                        new SimpleStmtsNode(19, 25, [
                            new PassStmtNode(19, 23, new PyPass(19, 23, [new WhiteSpaceTrivia(18, 19)]))
                        ], [], new PyNewline(23, 25, '\r', '\n', [])))
                ],
                null,
                null
            );

            Assert.Equivalent(required, res, strict: true);
        }

        [Fact]
        public void TestStatementTryExceptElseStatement()
        {
            var parser = new PythonCoreParser("try: pass\r\nexcept: pass\r\nelse: pass\r\n\r\n");
            parser.Advance();
            var res = parser.ParseStmts();

            var required = new TryExceptFinallyStatementBlockNode(0, 37,
                new PyTry(0, 3, []),
                new PyColon(3, 4, []),
                new SimpleStmtsNode(5, 11, [
                    new PassStmtNode(5, 9, new PyPass(5, 9, [new WhiteSpaceTrivia(4, 5)]))
                ], [], new PyNewline(9, 11, '\r', '\n', [])),
                [
                    new DefaultExceptStatementNode(11, 25,
                        new PyExcept(11, 17, []),
                        new PyColon(17, 18, []),
                        new SimpleStmtsNode(19, 25, [
                            new PassStmtNode(19, 23, new PyPass(19, 23, [new WhiteSpaceTrivia(18, 19)]))
                        ], [], new PyNewline(23, 25, '\r', '\n', [])))
                ],
                new ElseStatementNode(25, 37,
                    new PyElse(25, 29, []),
                    new PyColon(29, 30, []),
                    new SimpleStmtsNode(31, 37, [
                        new PassStmtNode(31, 35, new PyPass(31, 35, [new WhiteSpaceTrivia(30, 31)]))
                    ], [], new PyNewline(35, 37, '\r', '\n', []))
                ),
                null
            );

            Assert.Equivalent(required, res, strict: true);
        }

        [Fact]
        public void TestStatementTryExceptElseFinallyStatement()
        {
            var parser = new PythonCoreParser("try: pass\r\nexcept: pass\r\nelse: pass\r\nfinally: pass\r\n\r\n");
            parser.Advance();
            var res = parser.ParseStmts();

            var required = new TryExceptFinallyStatementBlockNode(0, 52,
                new PyTry(0, 3, []),
                new PyColon(3, 4, []),
                new SimpleStmtsNode(5, 11, [
                    new PassStmtNode(5, 9, new PyPass(5, 9, [new WhiteSpaceTrivia(4, 5)]))
                ], [], new PyNewline(9, 11, '\r', '\n', [])),
                [
                    new DefaultExceptStatementNode(11, 25,
                        new PyExcept(11, 17, []),
                        new PyColon(17, 18, []),
                        new SimpleStmtsNode(19, 25, [
                            new PassStmtNode(19, 23, new PyPass(19, 23, [new WhiteSpaceTrivia(18, 19)]))
                        ], [], new PyNewline(23, 25, '\r', '\n', [])))
                ],
                new ElseStatementNode(25, 37,
                    new PyElse(25, 29, []),
                    new PyColon(29, 30, []),
                    new SimpleStmtsNode(31, 37, [
                        new PassStmtNode(31, 35, new PyPass(31, 35, [new WhiteSpaceTrivia(30, 31)]))
                    ], [], new PyNewline(35, 37, '\r', '\n', []))
                ),
                new FinallyStatementNode(37, 52,
                    new PyFinally(37, 44, []),
                    new PyColon(44, 45, []),
                    new SimpleStmtsNode(46, 52, [
                        new PassStmtNode(46, 50, new PyPass(46, 50, [new WhiteSpaceTrivia(45, 46)]))
                    ], [], new PyNewline(50, 52, '\r', '\n', []))
                )
            );

            Assert.Equivalent(required, res, strict: true);
        }

        [Fact]
        public void TestStatementTryExceptFinallyStatement()
        {
            var parser = new PythonCoreParser("try: pass\r\nexcept: pass\r\nfinally: pass\r\n\r\n");
            parser.Advance();
            var res = parser.ParseStmts();

            var required = new TryExceptFinallyStatementBlockNode(0, 40,
                new PyTry(0, 3, []),
                new PyColon(3, 4, []),
                new SimpleStmtsNode(5, 11, [
                    new PassStmtNode(5, 9, new PyPass(5, 9, [new WhiteSpaceTrivia(4, 5)]))
                ], [], new PyNewline(9, 11, '\r', '\n', [])),
                [
                    new DefaultExceptStatementNode(11, 25,
                        new PyExcept(11, 17, []),
                        new PyColon(17, 18, []),
                        new SimpleStmtsNode(19, 25, [
                            new PassStmtNode(19, 23, new PyPass(19, 23, [new WhiteSpaceTrivia(18, 19)]))
                        ], [], new PyNewline(23, 25, '\r', '\n', [])))
                ],
                null,
                new FinallyStatementNode(25, 40,
                    new PyFinally(25, 32, []),
                    new PyColon(32, 33, []),
                    new SimpleStmtsNode(34, 40, [
                        new PassStmtNode(34, 38, new PyPass(34, 38, [new WhiteSpaceTrivia(33, 34)]))
                    ], [], new PyNewline(38, 40, '\r', '\n', []))
                )
            );

            Assert.Equivalent(required, res, strict: true);
        }

        [Fact]
        public void TestStatementTryExceptSingleStatement()
        {
            var parser = new PythonCoreParser("try: pass\r\nexcept a: pass\r\n\r\n");
            parser.Advance();
            var res = parser.ParseStmts();

            var required = new TryExceptFinallyStatementBlockNode(0, 27,
                new PyTry(0, 3, []),
                new PyColon(3, 4, []),
                new SimpleStmtsNode(5, 11, [
                    new PassStmtNode(5, 9, new PyPass(5, 9, [new WhiteSpaceTrivia(4, 5)]))
                ], [], new PyNewline(9, 11, '\r', '\n', [])),
                [
                    new ExceptStatementNode(11, 27,
                        new PyExcept(11, 17, []),
                        new NameLiteralNode(18, 19, new PyName(18, 19, "a", [new WhiteSpaceTrivia(17, 18)])),
                        null,
                        null,
                        new PyColon(19, 20, []),
                        new SimpleStmtsNode(21, 27, [
                            new PassStmtNode(21, 25, new PyPass(21, 25, [new WhiteSpaceTrivia(20, 21)]))
                        ], [], new PyNewline(25, 27, '\r', '\n', [])))


                ],
                null,
                null
            );

            Assert.Equivalent(required, res, strict: true);
        }

        [Fact]
        public void TestStatementTryExceptSingleAsStatement()
        {
            var parser = new PythonCoreParser("try: pass\r\nexcept a as b: pass\r\n\r\n");
            parser.Advance();
            var res = parser.ParseStmts();

            var required = new TryExceptFinallyStatementBlockNode(0, 32,
                new PyTry(0, 3, []),
                new PyColon(3, 4, []),
                new SimpleStmtsNode(5, 11, [
                    new PassStmtNode(5, 9, new PyPass(5, 9, [new WhiteSpaceTrivia(4, 5)]))
                ], [], new PyNewline(9, 11, '\r', '\n', [])),
                [
                    new ExceptStatementNode(11, 32,
                        new PyExcept(11, 17, []),
                        new NameLiteralNode(18, 19, new PyName(18, 19, "a", [new WhiteSpaceTrivia(17, 18)])),
                        new PyAs(20, 22, [new WhiteSpaceTrivia(19, 20)]),
                        new PyName(23, 24, "b", [new WhiteSpaceTrivia(22, 23)]),
                        new PyColon(24, 25, []),
                        new SimpleStmtsNode(26, 32, [
                            new PassStmtNode(26, 30, new PyPass(26, 30, [new WhiteSpaceTrivia(25, 26)]))
                        ], [], new PyNewline(30, 32, '\r', '\n', [])))


                ],
                null,
                null
            );

            Assert.Equivalent(required, res, strict: true);
        }

        [Fact]
        public void TestStatementTryStarExceptSingleStatement()
        {
            var parser = new PythonCoreParser("try: pass\r\nexcept *a: pass\r\n\r\n");
            parser.Advance();
            var res = parser.ParseStmts();

            var required = new TryExceptFinallyStatementBlockNode(0, 28,
                new PyTry(0, 3, []),
                new PyColon(3, 4, []),
                new SimpleStmtsNode(5, 11, [
                    new PassStmtNode(5, 9, new PyPass(5, 9, [new WhiteSpaceTrivia(4, 5)]))
                ], [], new PyNewline(9, 11, '\r', '\n', [])),
                [
                    new StarExceptStatementNode(11, 28,
                        new PyExcept(11, 17, []),
                        new PyMul(18, 19, [new WhiteSpaceTrivia(17, 18)]),
                        new NameLiteralNode(19, 20, new PyName(19, 20, "a", [])),
                        null,
                        null,
                        new PyColon(20, 21, []),
                        new SimpleStmtsNode(22, 28, [
                            new PassStmtNode(22, 26, new PyPass(22, 26, [new WhiteSpaceTrivia(21, 22)]))
                        ], [], new PyNewline(26, 28, '\r', '\n', [])))


                ],
                null,
                null
            );

            Assert.Equivalent(required, res, strict: true);
        }

        [Fact]
        public void TestStatementTryStarExceptSingleAsStatement()
        {
            var parser = new PythonCoreParser("try: pass\r\nexcept *a as b: pass\r\n\r\n");
            parser.Advance();
            var res = parser.ParseStmts();

            var required = new TryExceptFinallyStatementBlockNode(0, 33,
                new PyTry(0, 3, []),
                new PyColon(3, 4, []),
                new SimpleStmtsNode(5, 11, [
                    new PassStmtNode(5, 9, new PyPass(5, 9, [new WhiteSpaceTrivia(4, 5)]))
                ], [], new PyNewline(9, 11, '\r', '\n', [])),
                [
                    new StarExceptStatementNode(11, 33,
                        new PyExcept(11, 17, []),
                        new PyMul(18, 19, [new WhiteSpaceTrivia(17, 18)]),
                        new NameLiteralNode(19, 20, new PyName(19, 20, "a", [])),
                        new PyAs(21, 23, [new WhiteSpaceTrivia(20, 21)]),
                        new PyName(24, 25, "b", [new WhiteSpaceTrivia(23, 24)]),
                        new PyColon(25, 26, []),
                        new SimpleStmtsNode(27, 33, [
                            new PassStmtNode(27, 31, new PyPass(27, 31, [new WhiteSpaceTrivia(26, 27)]))
                        ], [], new PyNewline(31, 33, '\r', '\n', [])))


                ],
                null,
                null
            );

            Assert.Equivalent(required, res, strict: true);
        }

        [Fact]
        public void TestStatementTryExceptMultipleStatement()
        {
            var parser = new PythonCoreParser("try: pass\r\nexcept a: pass\r\nexcept: pass\r\n\r\n");
            parser.Advance();
            var res = parser.ParseStmts();

            var required = new TryExceptFinallyStatementBlockNode(0, 42,
                new PyTry(0, 3, []),
                new PyColon(3, 4, []),
                new SimpleStmtsNode(5, 11, [
                    new PassStmtNode(5, 9, new PyPass(5, 9, [new WhiteSpaceTrivia(4, 5)]))
                ], [], new PyNewline(9, 11, '\r', '\n', [])),
                [
                    new ExceptStatementNode(11, 27,
                        new PyExcept(11, 17, []),
                        new NameLiteralNode(18, 19, new PyName(18, 19, "a", [new WhiteSpaceTrivia(17, 18)])),
                        null,
                        null,
                        new PyColon(19, 20, []),
                        new SimpleStmtsNode(21, 27, [
                            new PassStmtNode(21, 25, new PyPass(21, 25, [new WhiteSpaceTrivia(20, 21)]))
                        ], [], new PyNewline(25, 27, '\r', '\n', []))),

                    new DefaultExceptStatementNode(27, 42,
                        new PyExcept(27, 32, []),
                        new PyColon(33, 34, []),
                        new SimpleStmtsNode(36, 41, [
                            new PassStmtNode(36, 40, new PyPass(36, 40, [new WhiteSpaceTrivia(35, 36)]))
                        ], [], new PyNewline(40, 42, '\r', '\n', [])))
                ],
                null,
                null
            );
        }






        [Fact]
        public void TestStatementSimpleDefaultMatchStatement()
        {
            var parser = new PythonCoreParser("match a:\r\n  case _: pass\r\npass\r\n\r\n");
            parser.Advance();
            var res = parser.ParseStmt();

            var required = new MatchStatementNode(0, 26,
                    new PyMatch(0, 5, []),
                    new NameLiteralNode(6, 7, new PyName(6, 7, "a" , [ new WhiteSpaceTrivia(5, 6) ])),
                        new PyColon(7, 8, []),
                    new PyNewline(8, 10, '\r', '\n', []),
                    new PyIndent([ new WhiteSpaceTrivia(10, 11), new WhiteSpaceTrivia(11, 12) ]),
                    [
                        new MatchCaseStatementNode(12, 24, 
                                new PyCase(12, 16, []),
                                new MatchDefaultCasePatternNode(17, 18, new PyDefault(17, 18, [ new WhiteSpaceTrivia(16, 17) ])),
                                null,
                                new PyColon(18, 19, []),
                                new SimpleStmtsNode(20, 24, [
                                    new PassStmtNode(20, 24, new PyPass(20, 24, [new WhiteSpaceTrivia(19, 20)]))
                                ], [], new PyNewline(24, 26, '\r', '\n', []))
                            )
                    ],
                    new PyDedent()
                );

            Assert.Equivalent(required, res, strict: true);
        }

        [Fact]
        public void TestStatementSimpleMatchOrStatement()
        {
            var parser = new PythonCoreParser("match a:\r\n  case 1 | 2 | 3: pass\r\npass\r\n\r\n");
            parser.Advance();
            var res = parser.ParseStmt();
            
            var root = ((res as MatchStatementNode)!.Elements[0] as MatchCaseStatementNode)!.Pattern as MatchOrPatternsNode;
            
            Assert.IsType<MatchOrPatternsNode>(root);

            var required = new MatchOrPatternsNode(17, 26, [
                new MatchNumberCasePatternNode(17, 19, new PyNumber(17, 18, "1", [new WhiteSpaceTrivia(16, 17)])),
                new MatchNumberCasePatternNode(21, 23, new PyNumber(21, 22, "2", [new WhiteSpaceTrivia(20, 21)])),
                new MatchNumberCasePatternNode(25, 26, new PyNumber(25, 26, "3", [new WhiteSpaceTrivia(24, 25)]))
            ], [
                new PyBitOr(19, 20, [new WhiteSpaceTrivia(18, 19)]),
                new PyBitOr(23, 24, [new WhiteSpaceTrivia(22, 23)])
            ]);
            
            Assert.Equivalent(required, root, strict: true);
        }
        
        [Fact]
        public void TestStatementSimpleMatchOrAsStatement()
        {
            var parser = new PythonCoreParser("match a:\r\n  case 1 | 2 | 3 as b: pass\r\npass\r\n\r\n");
            parser.Advance();
            var res = parser.ParseStmt();
            
            var root = ((res as MatchStatementNode)!.Elements[0] as MatchCaseStatementNode)!.Pattern as MatchAsPatternNode;
            
            Assert.IsType<MatchAsPatternNode>(root);
            
            Assert.Equivalent((root.As as PyAs), new PyAs(27, 29, [ new WhiteSpaceTrivia(26, 27) ]));
            Assert.Equivalent((root.Name as PyName), new PyName(30, 31, "b", [ new WhiteSpaceTrivia(29, 30) ]));

            Assert.IsType<MatchOrPatternsNode>(root.Left);
        }

    }
}
