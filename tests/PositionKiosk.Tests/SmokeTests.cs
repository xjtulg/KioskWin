using Xunit;

namespace PositionKiosk.Tests;

public class SmokeTests
{
    [Fact]
    public void TestRunnerWorks()
    {
        // 仅验证测试运行器与项目引用链路正常；Task 2 起被真正的 Core 测试取代
        Assert.Equal(1, 1);
    }
}
