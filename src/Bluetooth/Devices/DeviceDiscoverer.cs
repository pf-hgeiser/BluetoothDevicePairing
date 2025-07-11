using BluetoothDevicePairing.Bluetooth.Devices.Utils;
using BluetoothDevicePairing.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Windows.Devices.Enumeration;
using static Vanara.PInvoke.Ole32.PROPERTYKEY.System;
using static Vanara.PInvoke.User32;
using DeviceWatcher = BluetoothDevicePairing.Bluetooth.Devices.Utils.DeviceWatcher;

namespace BluetoothDevicePairing.Bluetooth.Devices;

internal sealed class DiscoveryTime
{
    public int Seconds { get; }

    public DiscoveryTime(int timeInSeconds)
    {
        if (timeInSeconds is < 1 or > 30)
        {
            throw new AppException($"discovery time should be in range [1; 30] but was {timeInSeconds}");
        }

        Seconds = timeInSeconds;
    }
}

internal static class DeviceDiscoverer
{
    public static List<Device> DiscoverBluetoothDevices(DiscoveryTime time)
    {
        return Discover(AsqFilter.BluetoothDevicesFilter(), time);
    }

    public static List<Device> DiscoverPairedBluetoothDevices(DiscoveryTime time)
    {
        return Discover(AsqFilter.PairedBluetoothDevicesFilter(), time);
    }

    private static List<Device> Discover(AsqFilter filter, DiscoveryTime time)
    {
        
        Console.WriteLine($"Start discovering devices for {time.Seconds} seconds\n");

        var watcher = new DeviceWatcher(filter);
        Thread.Sleep(1000);
        watcher.Start();
        Thread.Sleep(time.Seconds * 1000);
        var devices = watcher.Stop();
        
        return devices.Select(CreateDevice).OfType<Device>().ToList();
    }

    private static Device CreateDevice(Windows.Devices.Enumeration.DeviceInformation info)
    {
        try
        {
            // This can throw the following exception:
            //     System.ArgumentException: The parameter is incorrect. The provided device ID is not a valid BluetoothDevice object.
            return new DeviceInfoId(info).DeviceType == DeviceType.Bluetooth
                ? BluetoothDevice.FromDeviceInfo(info)
                : BluetoothLeDevice.FromDeviceInfo(info);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: failed to get information from the discovered device [{info.Name}; {info.Id}]. Error message: {ex.Message}");
            return null;
        }
    }
}
