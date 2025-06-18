using CommandLine;
using System;
using BluetoothDevicePairing.Bluetooth.Adapters;
using System.IO;

namespace BluetoothDevicePairing.Commands;

[Verb("list-adapters",
      HelpText = "Lists bluetooth adapters. Prints a table with the following fields:\n" +
                 "|Is default|Radio mac address|Name|State|")]
internal sealed class ListAdaptersOptions
{
}

internal static class ListAdapters
{
    public static void Execute(ListAdaptersOptions _)
    {
        var adapters = AdapterFinder.FindBluetoothAdapters();
        Console.WriteLine(new string('-', 71));
        foreach (var a in adapters)
        {
            PrintAdapter(a);
#pragma warning disable S1075
            PrintAdapter2File(a, "c:\\temp\\BTadaptors.txt");
#pragma warning restore S1075
        }
        Console.WriteLine(new string('-', 71));
    }

    private static void PrintAdapter(Adapter a)
    {
        Console.WriteLine($"|{IsDefault(a),1}|{a.MacAddress}|{a.Name,-40}|{a.State,-8}|");
    }

    private static void PrintAdapter2File(Adapter a, string fileNameAndPath)
    {
        using (StreamWriter outputFile = new StreamWriter(fileNameAndPath))
        {
            outputFile.WriteLine("IsDefault="+IsDefault(a)+";MacAdr="+a.MacAddress+";Name="+a.Name+";State="+a.State+";");
        }
    }

    private static string IsDefault(Adapter a)
    {
        return a.IsDefault ? "*" : "";
    }
}
