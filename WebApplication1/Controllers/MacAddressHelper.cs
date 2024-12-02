using System.Linq;
using System.Net.NetworkInformation;

public class MacAddressHelper
{
    public static string GetMacAddress()
    {

        // Get all network interfaces
        var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

        foreach (var networkInterface in networkInterfaces)
        {
            // Check if the network interface is operational
            if (networkInterface.OperationalStatus == OperationalStatus.Up &&
                (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                 networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211))
            {
                // Get the MAC address of the interface
                var macAddress = networkInterface.GetPhysicalAddress();

                // Convert the MAC address to a human-readable format
                return string.Join(":", macAddress.GetAddressBytes().Select(b => b.ToString("X2")));
            }
        }

        return "No MAC Address found.";
    }
}
