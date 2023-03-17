
using Cila;

internal class Program
{
    private static RelayService _relayService;

    private static void Main(string[] args)
    {
        var cnfg = new ConfigurationBuilder();
        var configuration = cnfg.AddJsonFile("relaysettings.json")
            .Build();
        var appSettings = configuration.GetSection("RelaySettings").Get<OmniChainRelaySettings>();
        _relayService =  new RelayService(appSettings);
        var timer = new Timer(ExecuteTask, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
        // Wait indefinitely
        Console.WriteLine("Press Ctrl+C to exit.");
        var exitEvent = new ManualResetEvent(false);
        Console.CancelKeyPress += (sender, e) => exitEvent.Set();
        exitEvent.WaitOne();
    }

    private static void ExecuteTask(object state)
    {
        _relayService.SyncAllChains();
        Console.WriteLine($"Task executed at {DateTime.UtcNow}");
    }
}
