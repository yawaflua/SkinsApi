
namespace SkinsApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHost(builder =>
                {
                    builder.UseKestrel(kestrelBuilder => kestrelBuilder.ListenAnyIP(80));
                    builder.UseStartup<Startup>();
                    builder.ConfigureLogging(k =>
                    {
                        k.SetMinimumLevel(LogLevel.Warning);
                    });
                })
                .Build()
                .Run();
        }
    }
}
