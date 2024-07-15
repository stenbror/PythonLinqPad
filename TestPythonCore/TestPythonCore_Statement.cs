using PythonCore;

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
                    [ new BreakStmtNode(0, 5, new PyBreak(0, 5, [])) ],
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
                            new PyName(5, 9, "test", [ new WhiteSpaceTrivia(4, 5) ]),
                            null,
                            new PyAssign(10, 11, [ new WhiteSpaceTrivia(9, 10) ]),
                            new NameLiteralNode(12, 15, new PyName(12, 15, "run", [ new WhiteSpaceTrivia(11, 12) ]))
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
                        new PyName(5, 9, "test", [ new WhiteSpaceTrivia(4, 5) ]),
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
                        new PyAssign(13, 14, [ new WhiteSpaceTrivia(12, 13) ]),
                        new NameLiteralNode(15, 18, new PyName(15, 18, "run", [ new WhiteSpaceTrivia(14, 15) ]))
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

            var node = ((((res as SimpleStmtsNode)?.Elements[0] as TypeAliasNode)?.Parameters as TypeParamsNode)?.Right as TypeParamSequenceNode);
            Assert.Equal(10, node?.StartPos);
            Assert.Equal(26, node?.EndPos);

            var elements = node?.Elements;
            Assert.Equal(4, elements?.Length);
            Assert.IsType<TypeParameterNode>(elements?[0]);
            Assert.IsType<TypeStarParameterNode>(elements?[1]);
            Assert.IsType<TypePowerParameterNode>(elements?[2]);
            Assert.IsType<TypeParameterTypedNode>(elements?[3]);

            var element1 = elements?[1] as TypeStarParameterNode;
            Assert.Equivalent( new PyName(14, 15, "b", []), element1?.Name);

            var element2 = elements?[2] as TypePowerParameterNode;
            Assert.Equivalent(new PyName(19, 20, "c", []), element2?.Name);

            var element3 = elements?[3] as TypeParameterTypedNode;
            Assert.Equivalent(new PyName(22, 23, "d", [ new WhiteSpaceTrivia(21, 22) ]), element3?.Name);
            Assert.IsType<NameLiteralNode>(element3?.Right);

            var name = element3?.Right as NameLiteralNode;
            Assert.Equivalent( new PyName(25, 26, "e", [new WhiteSpaceTrivia(24, 25)]) , name?.Element);

            var separators = node?.Separators;
            Assert.Equal(3, separators?.Length);
            Assert.Equivalent( new PyComma(11, 12, []), separators?[0] );
            Assert.Equivalent(new PyComma(15, 16, []), separators?[1]);
            Assert.Equivalent(new PyComma(20, 21, []), separators?[2]);
        }

        [Fact]
        public void TestStatementTypeAliasSingleWithComma()
        {
            var parser = new PythonCoreParser("type test[a,] = run\r\n\r\n");
            parser.Advance();
            var res = parser.ParseStmts();

            var node = ((((res as SimpleStmtsNode)?.Elements[0] as TypeAliasNode)?.Parameters as TypeParamsNode)?.Right as TypeParamSequenceNode);
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
                            [ new PyName(7, 8, "a", [ new WhiteSpaceTrivia(6, 7) ]) ], 
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
                            new PyName(7, 8, "a", [ new WhiteSpaceTrivia(6, 7) ]),
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
                        [ new PyName(9, 10, "a", [ new WhiteSpaceTrivia(8, 9) ]) ],
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
                            new PyName(9, 10, "a", [ new WhiteSpaceTrivia(8, 9) ]),
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
                        new NameLiteralNode(7, 8, new PyName(7,8, "a", [ new WhiteSpaceTrivia(6, 7) ]))
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
                        new NameLiteralNode(6, 7, new PyName(6, 7, "a", [ new WhiteSpaceTrivia(5, 6) ]))
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
                        new NameLiteralNode(6, 7, new PyName(6, 7, "a", [ new WhiteSpaceTrivia(5, 6) ])),
                        new PyFrom(8, 12, [ new WhiteSpaceTrivia(7, 8) ]),
                        new NameLiteralNode(13, 14, new PyName(13, 14, "b", [ new WhiteSpaceTrivia(12, 13) ]))
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
                        new YieldExpressionNode(0, 7, new PyYield(0, 5, []), new NameLiteralNode(6, 7, new PyName(6, 7, "a", [ new WhiteSpaceTrivia(5, 6) ])))
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
                        new NameLiteralNode(7, 8, new PyName(7, 8, "a", [ new WhiteSpaceTrivia(6, 7) ]))
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
                        new NameLiteralNode(7, 8, new PyName(7, 8, "a", [ new WhiteSpaceTrivia(6, 7) ])),
                        new PyComma(8, 9, []),
                        new NameLiteralNode(9, 10, new PyName(9, 10, "b", [] ))
                    )
                ],
                [],
                new PyNewline(10, 12, '\r', '\n', [])
            );

            Assert.Equivalent(required, res, strict: true);
        }
    }
}
