using System.Collections.Concurrent;
using YeelightAPI.Models;

namespace YeelightAPI.UnitTests;

/// <summary>
/// Base class for integration tests that provides rate limiting to avoid exceeding
/// the Yeelight device quota of 60 requests per minute.
/// </summary>
public abstract class IntegrationTestBase
{
    // Shared across all integration tests - tracks request timestamps per device
    private static readonly ConcurrentDictionary<string, Queue<DateTime>> _requestTimestamps = new();
    private static readonly SemaphoreSlim _rateLimitSemaphore = new(1, 1);

    // Yeelight limit: 60 requests per minute per device
    private const int MaxRequestsPerMinute = 60;
    private const int SafeRequestsPerMinute = 50; // Leave some margin
    private static readonly TimeSpan RateLimitWindow = TimeSpan.FromMinutes(1);
    private static readonly TimeSpan MinDelayBetweenRequests = TimeSpan.FromMilliseconds(1200); // ~50 req/min

    /// <summary>
    /// Waits if necessary to respect the rate limit before making a request.
    /// Call this before executing commands on a device.
    /// </summary>
    protected static async Task RespectRateLimit(string? deviceId = null)
    {
        var key = deviceId ?? "global";

        await _rateLimitSemaphore.WaitAsync();
        try
        {
            if (!_requestTimestamps.TryGetValue(key, out var timestamps))
            {
                timestamps = new Queue<DateTime>();
                _requestTimestamps[key] = timestamps;
            }

            var now = DateTime.UtcNow;

            // Remove old timestamps outside the window
            while (timestamps.Count > 0 && now - timestamps.Peek() > RateLimitWindow)
            {
                timestamps.Dequeue();
            }

            // If we're at the limit, wait until the oldest request expires
            if (timestamps.Count >= SafeRequestsPerMinute)
            {
                var oldestTimestamp = timestamps.Peek();
                var waitTime = RateLimitWindow - (now - oldestTimestamp) + TimeSpan.FromMilliseconds(100);
                if (waitTime > TimeSpan.Zero)
                {
                    Console.WriteLine($"Rate limit: waiting {waitTime.TotalSeconds:F1}s for {key}");
                    await Task.Delay(waitTime);
                }
                // Remove the expired timestamp
                timestamps.Dequeue();
            }

            // Add current request timestamp
            timestamps.Enqueue(DateTime.UtcNow);
        }
        finally
        {
            _rateLimitSemaphore.Release();
        }
    }

    /// <summary>
    /// Adds a small delay between operations to spread out requests.
    /// </summary>
    protected static Task SmallDelay() => Task.Delay(MinDelayBetweenRequests);

    /// <summary>
    /// Gets a random connected device with rate limiting.
    /// </summary>
    protected static async Task<Device> GetRandomConnectedDevice(METHODS? supportedMethod = null)
    {
        await RespectRateLimit();

        var devices = (await DeviceLocator.DiscoverAsync())
            .Where(d => !supportedMethod.HasValue || d.SupportedOperations.Contains(supportedMethod.Value))
            .ToList();

        if (devices.Count == 0)
            throw new InvalidOperationException($"No devices found" +
                (supportedMethod.HasValue ? $" supporting {supportedMethod.Value}" : ""));

        var randomIndex = new Random().Next(0, devices.Count);
        var device = devices[randomIndex];
        Console.WriteLine($"Using device: {device} (Model: {device.Model})");

        await RespectRateLimit(device.Id);
        await device.Connect();

        return device;
    }

    /// <summary>
    /// Gets all devices and connects them with rate limiting.
    /// </summary>
    protected static async Task<List<Device>> GetAllConnectedDevices(METHODS? supportedMethod = null)
    {
        await RespectRateLimit();

        var devices = (await DeviceLocator.DiscoverAsync())
            .Where(d => !supportedMethod.HasValue || d.SupportedOperations.Contains(supportedMethod.Value))
            .ToList();

        foreach (var device in devices)
        {
            await RespectRateLimit(device.Id);
            await device.Connect();
        }

        return devices;
    }

    /// <summary>
    /// Executes a device command with rate limiting.
    /// </summary>
    protected static async Task<T> ExecuteWithRateLimit<T>(Device device, Func<Task<T>> command)
    {
        await RespectRateLimit(device.Id);
        return await command();
    }

    /// <summary>
    /// Executes a device command with rate limiting.
    /// </summary>
    protected static async Task ExecuteWithRateLimit(Device device, Func<Task> command)
    {
        await RespectRateLimit(device.Id);
        await command();
    }
}
