using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace BluetoothDevicePairing.Bluetooth.Devices.Utils;

internal sealed class DeviceWatcher
{
    private readonly Windows.Devices.Enumeration.DeviceWatcher watcher;
    private readonly AutoResetEvent watcherStoppedEvent = new(false);
    private List<Windows.Devices.Enumeration.DeviceInformation> devices;

    public DeviceWatcher(AsqFilter filter)
    {
        watcher = Windows.Devices.Enumeration.DeviceInformation.CreateWatcher(filter.Query,
                                                                              null,
                                                                              Windows.Devices.Enumeration.DeviceInformationKind.AssociationEndpoint);

        watcher.Added += (_, info) => devices.Add(info);

        watcher.Removed += (_, removedDevice) =>
        {
            for (int i=0; i<devices.Count; i++)
            {
                if (devices[i] != null)
                {
                    var device = devices[i];
                    if (device != null && device.Id == removedDevice.Id)
                    {
                        devices.RemoveAt(i);    
                    }
                }
            }

#pragma warning disable S125 // Rethrow to preserve stack details

            //##### if foreach is used and list of devices is changed, then enumerator throws exception ####
            //foreach (var device in devices.Where(device => device.Id == removedDevice.Id))
            //{
            //    devices.Remove(device);
            //}
#pragma warning restore S125 // Rethrow to preserve stack details
        };

        watcher.Updated += (_, updatedDevice) =>
        {
            foreach (var device in devices.Where(device => device.Id == updatedDevice.Id))
            {
                device.Update(updatedDevice);
            }
        };

        watcher.Stopped += (_, _) => watcherStoppedEvent.Set();
    }

    public void Start()
    {
        devices = new();
        watcher.Start();
    }

    public IEnumerable<Windows.Devices.Enumeration.DeviceInformation> Stop()
    {
        watcher.Stop();
        var receivedSignal = watcherStoppedEvent.WaitOne(5 * 1000);
        if (!receivedSignal)
        {
            Console.WriteLine("Warning: the watcher didn't stop after 5 seconds");
        }

        return devices;
    }
}
