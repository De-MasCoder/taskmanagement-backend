using MassTransit;
using TaskContracts.Events;
using TaskContracts.Models;
using TaskEmailService.Helpers;

namespace TaskEmailService
{
    public class TaskCreatedEventConsumer : IConsumer<TaskCreated>
    {
        private readonly Supabase.Client _supabase;
        private readonly ILogger<TaskCreatedEventConsumer> _logger;
        private readonly IConfiguration _configuration;

        public TaskCreatedEventConsumer(Supabase.Client supabase, ILogger<TaskCreatedEventConsumer> logger, IConfiguration configuration)
        {
            _supabase = supabase;
            _logger = logger;
            _configuration = configuration;
        }
        public async Task Consume(ConsumeContext<TaskCreated> context)
        {
            try
            {
                var task = context.Message;
                // Notify managers about the new task
                var managers = await _supabase
                    .From<User>()
                    .Where(x => x.Role == "manager")
                    .Get();

                foreach (var manager in managers.Models)
                {
                    await EmailHelper.SendEmailAsync(manager.EmailAddress, task.Name, _configuration);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while consuming TaskCreatedEvent");
                throw;
            }
        }
    }
}
