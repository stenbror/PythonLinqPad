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
    }
}
