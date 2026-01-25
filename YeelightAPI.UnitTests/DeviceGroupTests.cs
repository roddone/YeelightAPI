namespace YeelightAPI.UnitTests;

public class DeviceGroupTests
{
    #region Constructor Tests

    [Test]
    public async Task DeviceGroup_DefaultConstructor_CreatesEmptyGroup()
    {
        using var group = new DeviceGroup();

        await Assert.That(group).IsEmpty();
        await Assert.That(group.Name).IsNull();
    }

    [Test]
    public async Task DeviceGroup_ConstructorWithName_SetsName()
    {
        using var group = new DeviceGroup("Living Room");

        await Assert.That(group.Name).IsEqualTo("Living Room");
        await Assert.That(group).IsEmpty();
    }

    [Test]
    public async Task DeviceGroup_ConstructorWithDevice_AddsDevice()
    {
        using var device = new Device("192.168.1.100");
        using var group = new DeviceGroup(device);

        await Assert.That(group).HasSingleItem();
        await Assert.That(group[0]).IsSameReferenceAs(device);
    }

    [Test]
    public async Task DeviceGroup_ConstructorWithDeviceAndName_SetsNameAndAddsDevice()
    {
        using var device = new Device("192.168.1.100");
        using var group = new DeviceGroup(device, "Test Group");

        await Assert.That(group.Name).IsEqualTo("Test Group");
        await Assert.That(group).HasSingleItem();
    }

    [Test]
    public async Task DeviceGroup_ConstructorWithParams_AddsAllDevices()
    {
        using var device1 = new Device("192.168.1.100");
        using var device2 = new Device("192.168.1.101");
        using var device3 = new Device("192.168.1.102");
        using var group = new DeviceGroup(device1, device2, device3);

        await Assert.That(group.Count).IsEqualTo(3);
        await Assert.That(group).Contains(device1);
        await Assert.That(group).Contains(device2);
        await Assert.That(group).Contains(device3);
    }

    [Test]
    public async Task DeviceGroup_ConstructorWithEnumerable_AddsAllDevices()
    {
        using var device1 = new Device("192.168.1.100");
        using var device2 = new Device("192.168.1.101");
        var devices = new List<Device> { device1, device2 };
        using var group = new DeviceGroup(devices, "Bedroom");

        await Assert.That(group.Count).IsEqualTo(2);
        await Assert.That(group.Name).IsEqualTo("Bedroom");
    }

    #endregion

    #region ToString Tests

    [Test]
    public async Task DeviceGroup_ToString_WithName()
    {
        using var device = new Device("192.168.1.100");
        using var group = new DeviceGroup(device, "Kitchen");

        var result = group.ToString();

        await Assert.That(result).IsEqualTo("Kitchen (1 devices)");
    }

    [Test]
    public async Task DeviceGroup_ToString_WithoutName()
    {
        using var device1 = new Device("192.168.1.100");
        using var device2 = new Device("192.168.1.101");
        using var group = new DeviceGroup(device1, device2);

        var result = group.ToString();

        await Assert.That(result).IsEqualTo(" (2 devices)");
    }

    [Test]
    public async Task DeviceGroup_ToString_Empty()
    {
        using var group = new DeviceGroup("Empty Group");

        var result = group.ToString();

        await Assert.That(result).IsEqualTo("Empty Group (0 devices)");
    }

    #endregion

    #region List Behavior Tests

    [Test]
    public async Task DeviceGroup_CanAddDevices()
    {
        using var device = new Device("192.168.1.100");
        using var group = new DeviceGroup();

        group.Add(device);

        await Assert.That(group).HasSingleItem();
    }

    [Test]
    public async Task DeviceGroup_CanRemoveDevices()
    {
        using var device = new Device("192.168.1.100");
        using var group = new DeviceGroup(device);

        group.Remove(device);

        await Assert.That(group).IsEmpty();
    }

    [Test]
    public async Task DeviceGroup_CanClearDevices()
    {
        using var device1 = new Device("192.168.1.100");
        using var device2 = new Device("192.168.1.101");
        using var group = new DeviceGroup(device1, device2);

        group.Clear();

        await Assert.That(group).IsEmpty();
    }

    [Test]
    public async Task DeviceGroup_CanEnumerate()
    {
        using var device1 = new Device("192.168.1.100");
        using var device2 = new Device("192.168.1.101");
        using var group = new DeviceGroup(device1, device2);

        var count = 0;
        foreach (var device in group)
        {
            count++;
        }

        await Assert.That(count).IsEqualTo(2);
    }

    [Test]
    public async Task DeviceGroup_CanAccessByIndex()
    {
        using var device1 = new Device("192.168.1.100");
        using var device2 = new Device("192.168.1.101");
        using var group = new DeviceGroup(device1, device2);

        await Assert.That(group[0]).IsSameReferenceAs(device1);
        await Assert.That(group[1]).IsSameReferenceAs(device2);
    }

    #endregion

    #region Name Property Tests

    [Test]
    public async Task DeviceGroup_Name_CanBeSet()
    {
        using var group = new DeviceGroup();

        group.Name = "Updated Name";

        await Assert.That(group.Name).IsEqualTo("Updated Name");
    }

    [Test]
    public async Task DeviceGroup_Name_CanBeNull()
    {
        using var group = new DeviceGroup("Initial Name");

        group.Name = null;

        await Assert.That(group.Name).IsNull();
    }

    #endregion

    #region IDisposable Tests

    [Test]
    public async Task DeviceGroup_ImplementsIDisposable()
    {
        var group = new DeviceGroup();

        await Assert.That(group).IsAssignableTo<IDisposable>();

        group.Dispose(); // Should not throw
    }

    [Test]
    public async Task DeviceGroup_IsListOfDevice()
    {
        using var group = new DeviceGroup();

        await Assert.That(group).IsAssignableTo<List<Device>>();
    }

    #endregion
}
