namespace JsonServer
{
    public class DatabaseAutoSaver(IServiceProvider serviceProvider) : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var database = scope.ServiceProvider.GetRequiredService<Database>();
                    await database.SaveChangesAsync();
                }
                catch (Exception)
                {
                    // ignore
                }
                finally
                {
                    await Task.Delay(new TimeSpan(0, 0, 5), stoppingToken);
                }
            }
        }
    }
}
