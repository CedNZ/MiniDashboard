using System.Reflection;

namespace MiniDashboard.Tests.UnitTests.TestHelpers;

internal static class AsyncTestHelper
{
    public static async Task InvokePrivateAsync(object instance, string methodName, params object?[]? parameters)
    {
        var method = instance.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
        if (method == null)
        {
            throw new InvalidOperationException($"Method '{methodName}' was not found on type '{instance.GetType().Name}'.");
        }

        var result = method.Invoke(instance, parameters);
        if (result is Task task)
        {
            await task.ConfigureAwait(false);
        }
        else
        {
            throw new InvalidOperationException($"Method '{methodName}' did not return a Task.");
        }
    }

    public static async Task WaitForConditionAsync(Func<bool> condition, TimeSpan timeout, TimeSpan? pollInterval = null)
    {
        var interval = pollInterval ?? TimeSpan.FromMilliseconds(10);
        var start = DateTime.UtcNow;
        while (!condition())
        {
            if (DateTime.UtcNow - start > timeout)
            {
                throw new TimeoutException("The condition was not met within the allotted time.");
            }

            await Task.Delay(interval).ConfigureAwait(false);
        }
    }
}
