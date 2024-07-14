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
    }
}
