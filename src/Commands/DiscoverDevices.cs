using BluetoothDevicePairing.Bluetooth.Devices;
using CommandLine;
using System;
using System.IO;
using System.Linq;
using Windows.UI.Xaml.Shapes;

namespace BluetoothDevicePairing.Commands;

[Verb("discover",
      HelpText = "Discover devices. Prints a table with the following fields:\n" +
                 "|Device type|Mac address|Pairing status|Device name|")]
internal sealed class DiscoverDevicesOptions
{
    [Option("discovery-time",
            Default = 5,
            HelpText = "how long to search for devices. Units: seconds")]
    public int DiscoveryTime { get; set; }
}

internal static class DiscoverDevices
{
    public static void Execute(DiscoverDevicesOptions opts)
    {
        var devices = DeviceDiscoverer.DiscoverBluetoothDevices(new DiscoveryTime(opts.DiscoveryTime)).OrderBy(d => d.Name);
        Console.WriteLine("List all K850 devices:");
        Console.WriteLine(new string('-', 73));
        Console.WriteLine($"|Tp|{"Dev",17}|ConStatus|{"Name", -40}|");
        Console.WriteLine(new string('-', 73));
#pragma warning disable S1075
        string fileNameAndPath = "c:\\temp\\BTdevices.txt";
#pragma warning restore S1075
        if (File.Exists(fileNameAndPath) )
        {
            File.Delete(fileNameAndPath);
        }

        //HG show only K850 devices
        var k850Devices = devices.Where(a => a.Name.Contains("K850"));
        if (k850Devices.Any())
        {
            foreach (var d in k850Devices)
            {
                PrintDevice(d);
                PrintDevice2File(d, fileNameAndPath);
            }
        }
        else
        {
            Console.WriteLine("## No K850 device found, try switching on power ##");
        }

        //HG show only newest K850 devices
#pragma warning disable S1075
        string latestFileNameAndPath = "c:\\temp\\mac.txt";
#pragma warning restore S1075

        Console.WriteLine($"Write newest K850 devices to file {latestFileNameAndPath}");
        var listOfMacs = k850Devices.Select(a => a.Id.DeviceMac.ToString());
        if (listOfMacs is not null)
        {
            var highestMac = listOfMacs.Max();
            if (File.Exists(latestFileNameAndPath))
            {
                File.Delete(latestFileNameAndPath);
            }
            PrintMac2File(highestMac, latestFileNameAndPath);
        }
        else
        {
            Console.WriteLine("## Newest K850 device found, try switching on power ##");
        }

        Console.WriteLine(new string('-', 73));
    }

    private static void PrintMac2File(string macAdress, string latestFileNameAndPath)
    {
        using (StreamWriter outputFile = new StreamWriter(latestFileNameAndPath, true))
        {
            Console.WriteLine($"mac adress {macAdress} written");
            outputFile.WriteLine(macAdress);
        }
    }

    private static void PrintDevice(Device d)
    {
        Console.WriteLine($"|{GetType(d),2}|{d.Id.DeviceMac}|{GetConnectionStatus(d),-9}|{GetName(d),-40}|");

        if (!d.AssociatedAudioDevices.Any())
        {
            return;
        }

        Console.WriteLine($"|  |                 |         |  Associated audio devices:             |");
        foreach (var audioDevice in d.AssociatedAudioDevices)
        {
            Console.WriteLine($"|  |                 |         |  * {audioDevice.Name,-36}|");
        }
    }
    private static void PrintDevice2File(Device d, string fileNameAndPath)
    {
        using (StreamWriter outputFile = new StreamWriter(fileNameAndPath, true))
        {
            outputFile.WriteLine("Type="+GetType(d)+";Id="+d.Id.DeviceMac+";Status="+GetConnectionStatus(d)+";Name="+GetName(d)+";");

            if (!d.AssociatedAudioDevices.Any())
            {
                return;
            }

            outputFile.WriteLine($"|  |                 |         |  Associated audio devices:             |");
            foreach (var audioDevice in d.AssociatedAudioDevices)
            {
                outputFile.WriteLine($"|  |                 |         |  * {audioDevice.Name,-36}|");
            }
        }
    }

    private static string GetType(Device d)
    {
        return d.Id.DeviceType == DeviceType.BluetoothLE ? "LE" : "";
    }

    private static string GetName(Device d)
    {
        return d.Name == "" ? "<Unknown>" : d.Name;
    }

    private static string GetConnectionStatus(Device d)
    {
        return d.ConnectionStatus == ConnectionStatus.NotPaired ? "" : d.ConnectionStatus.ToString();
    }
}
