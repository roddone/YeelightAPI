namespace YeelightAPI.UnitTests;

/// <summary>
/// Global hooks for integration tests to manage rate limiting.
/// </summary>
public class GlobalTestHooks
{
    // Delay after each integration test to respect rate limits
    private const int DelayAfterTestMs = 1000;

    [AfterEvery(Test)]
    public static async Task AfterEachTest(TestContext context)
    {
        // Only add delay for integration tests (tests in classes with NotInParallel attribute)
        var testClass = context.TestDetails.ClassType;
        if (testClass != null &&
            testClass.GetCustomAttributes(typeof(NotInParallelAttribute), true).Length > 0)
        {
            await Task.Delay(DelayAfterTestMs);
        }
    }
}
