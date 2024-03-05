using System.Net;
using System.Net.Sockets;

void WriteMenu()
{
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine("\n1. Gasire ip din numele domeniului");
    Console.WriteLine("2. Găsi domeniu sau lista de domenii din ip");
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
    Console.WriteLine("\nGaseste Ip address dupa DNS name");
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
        break;
}




IPAddress addr = IPAddress.Parse("185.60.218.35");
IPHostEntry entry = Dns.GetHostEntry(addr);
Console.WriteLine(entry.HostName);

IPHostEntry hostInfo = Dns.GetHostEntry("www.contoso.com");
Console.WriteLine(hostInfo.HostName);
var a = Dns.GetHostAddresses("www.facebook.com");
Console.WriteLine(a[0]);