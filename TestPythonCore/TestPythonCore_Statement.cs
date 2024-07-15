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
    }
}
