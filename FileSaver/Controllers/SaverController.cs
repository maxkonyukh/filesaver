using System;
using System.Threading.Tasks;
using FileSaver.Helpers;
using FileSaver.Models;
using FileSaver.Services;
using FileSaver.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FileSaver.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaverController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IBackgroundTaskQueue _backgroundTaskQueue;

        public SaverController(IBackgroundTaskQueue backgroundTaskQueue, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<QueuedHostedService>();
            _backgroundTaskQueue = backgroundTaskQueue;
        }

        [HttpPost, Route("save")]
        public IActionResult Save([FromBody] SaveViewModel request)
        {
            _backgroundTaskQueue.QueueBackgroundWorkItem(async token =>
            {
                var guid = Guid.NewGuid().ToString();

                try
                {
                    await FileHelper.FileWriteAsync($@"C:\Users\m.konyukh\Desktop\Files\File_{guid}.txt",
                        request.FileContent);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "An error occurred writing to the " +
                        $"database. Error: {ex.Message}");
                }

                await Task.Delay(TimeSpan.FromSeconds(10), token);
            });

            return Ok();
        }
    }
}
