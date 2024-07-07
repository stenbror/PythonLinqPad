using PythonCore;

namespace TestPythonCore;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var parser = new PythonCoreParser("def __init__(): pass");

        var text = "def __init__(): pass";
        var y = text.Substring(4, 12 - 4);


        var tst = new ExpressionNode(1, 2);
        Assert.Equal(1, tst.StartPos);

    }
}