using Xunit;

namespace ThirdRun.Tests
{
    public class ResourceBarDemoTest
    {
        [Fact]
        public void ResourceBarDemo_RunsSuccessfully()
        {
            // Just verify the demo runs without exceptions
            var exception = Record.Exception(() => ResourceBarDemo.RunDemo());
            Assert.Null(exception);
        }
    }
}