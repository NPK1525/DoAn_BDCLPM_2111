using NUnit.Framework;

namespace TestProject1.Utilities
{
    public static class TestContextHelper
    {
        public static string GetTestMessage()
        {
            var msg = TestContext.CurrentContext.Result.Message;
            if (string.IsNullOrEmpty(msg)) return "";
            return msg.Length > 300 ? msg[..300] + "..." : msg;
        }
    }
}
