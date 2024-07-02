using PythonCore;

namespace TestPythonCore;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var parser = new PythonCoreParser();

        var tst = new ExpressionNode(1, 2);
        Assert.Equal(1u, tst.StartPos);

    }
}