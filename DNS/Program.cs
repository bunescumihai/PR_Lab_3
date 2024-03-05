using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Management;

List<string> dnsServers = new List<string>() { "1.1.1.1; General", "89.34.198.253; Moldtelecom", "81.180.65.41; Nu functioneaza" };

void WriteMenu()
{
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine("\n1. Gasire DNS name dupa ip");
    Console.WriteLine("2. Gasire Ip address dupa DNS name");
    Console.WriteLine("3. Utilizeaza alt dns");
    Console.WriteLine("0. Iesire");
}

int WaitAction()
{
    int action = 0;
    do
    {
        Console.Write("Alegeti o actiune: ");
        try
        {
            action = int.Parse(Console.ReadLine());

            if(action < 0 && action > 3)
                Console.WriteLine("Introduceti o actiune valida");
        }
        catch
        {
            Console.WriteLine("Introduceti o actiune valida");
            action = -1;
        }
        
    }
    while (action < 0 || action > 3);

    return action;
}

void GetDNSNameByIp()
{
    Console.ForegroundColor = ConsoleColor.DarkYellow;
    Console.WriteLine("\nGaseste DNS name dupa ip");
    Console.Write("Introduce Ip-ul: ");
    Console.ForegroundColor = ConsoleColor.Cyan;
    
    string typedAddress = Console.ReadLine().Trim();

    try
    {
        IPAddress addr = IPAddress.Parse(typedAddress);
        IPHostEntry entry = Dns.GetHostEntry(addr);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("DNS name ip adresa: " + typedAddress + " => " + entry.HostName);
    }
    catch(FormatException fex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Ip adresa gresita");
    }
    catch(SocketException ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("DNS name nu a putut fi gasit");
    }
}

void GetIpByDNSName()
{
    Console.ForegroundColor = ConsoleColor.DarkYellow;
    Console.WriteLine("\nGasire Ip address dupa DNS name");
    Console.Write("Introduce DNS: ");
    Console.ForegroundColor = ConsoleColor.Cyan;

    string DNSName = Console.ReadLine().Trim();

    try
    {
        IPAddress[] addresses = Dns.GetHostAddresses(DNSName);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write($"Ip adrese asignate DNS name \"{DNSName}\": ");
        foreach(var address in addresses)
        {
            Console.Write(address + " ");
        }
    }
    catch (SocketException ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"DNS name \"{DNSName}\" nu este asignat nici unui Ip");
    }
}

void SetOtherDns()
{
    Console.ForegroundColor = ConsoleColor.DarkYellow;
    int server = -1;
    Console.WriteLine("\nLista de DNS severe");
       
    Console.ForegroundColor = ConsoleColor.Gray;
    int i = 1;

    foreach(var dns in dnsServers)
    {
        Console.WriteLine(i + ". " + dns);
        i++;
    }

    Console.ForegroundColor = ConsoleColor.DarkYellow;
    Console.Write("Alege un DNS server din lista: ");

    while(server == -1)
    {
        try
        {
            server = int.Parse(Console.ReadLine());
            string ip = dnsServers.ElementAt(server - 1).Split(';')[0];
            SetDNS(ip);
            Console.ForegroundColor= ConsoleColor.DarkYellow;
            Console.Write("DNS server setat: ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(ip);
        }
        catch(FormatException fex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Alegeti un DNS server din lista");
            server = -1;
        }
        catch (ArgumentOutOfRangeException fex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Alegeti un DNS server din lista");
            server = -1;
        }
        catch( Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Ceva nu a mers corect");
            server = -1;
        }
    }
}

int HandleAction(int action)
{
    switch (action)
    {
        case 0: return -1;
        case 1: GetDNSNameByIp(); return 0;
        case 2: GetIpByDNSName(); return 0;
        case 3: SetOtherDns(); return 0;
        default: return 0;
    }
}

while (true)
{

    WriteMenu();
    int action = WaitAction();
    
    int response = HandleAction(action);

    if (response == -1)
    {
        UnsetDNS();
        break;
    }
}


//   Love stackoverflow


NetworkInterface GetActiveEthernetOrWifiNetworkInterface()
{
    var Nic = NetworkInterface.GetAllNetworkInterfaces().FirstOrDefault(
        a => a.OperationalStatus == OperationalStatus.Up &&
        (a.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || a.NetworkInterfaceType == NetworkInterfaceType.Ethernet) &&
        a.GetIPProperties().GatewayAddresses.Any(g => g.Address.AddressFamily.ToString() == "InterNetwork"));

    return Nic;
}

void SetDNS(string DnsString)
{
    string[] Dns = { DnsString };
    var CurrentInterface = GetActiveEthernetOrWifiNetworkInterface();
    if (CurrentInterface == null) return;

    ManagementClass objMC = new ManagementClass("Win32_NetworkAdapterConfiguration");
    ManagementObjectCollection objMOC = objMC.GetInstances();
    foreach (ManagementObject objMO in objMOC)
    {
        if ((bool)objMO["IPEnabled"])
        {
            if (objMO["Description"].ToString().Equals(CurrentInterface.Description))
            {
                ManagementBaseObject objdns = objMO.GetMethodParameters("SetDNSServerSearchOrder");
                if (objdns != null)
                {
                    objdns["DNSServerSearchOrder"] = Dns;
                    objMO.InvokeMethod("SetDNSServerSearchOrder", objdns, null);
                }
            }
        }
    }
}

void UnsetDNS()
{
    var CurrentInterface = GetActiveEthernetOrWifiNetworkInterface();
    if (CurrentInterface == null) return;

    ManagementClass objMC = new ManagementClass("Win32_NetworkAdapterConfiguration");
    ManagementObjectCollection objMOC = objMC.GetInstances();
    foreach (ManagementObject objMO in objMOC)
    {
        if ((bool)objMO["IPEnabled"])
        {
            if (objMO["Description"].ToString().Equals(CurrentInterface.Description))
            {
                ManagementBaseObject objdns = objMO.GetMethodParameters("SetDNSServerSearchOrder");
                if (objdns != null)
                {
                    objdns["DNSServerSearchOrder"] = null;
                    objMO.InvokeMethod("SetDNSServerSearchOrder", objdns, null);
                }
            }
        }
    }
}