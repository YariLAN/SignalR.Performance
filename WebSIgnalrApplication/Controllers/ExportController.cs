using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignalRChatApp.Hubs;

namespace WebSIgnalrApplication.Controllers
{
    [Route("api/latency")]
    [ApiController]
    public class ExportController : ControllerBase
    {
        public List<(DateTime Time, double Latency)> GetLatencies()
        {
            lock (ChatHub.GlobalLatencies)
            {
                return new List<(DateTime, double)>(ChatHub.GlobalLatencies); // Копия данных
            }
        }

        [HttpGet("export")]
        public IActionResult ExportLatencies()
        {
            lock (ChatHub.GlobalLatencies)
            {
                // Преобразуем данные в формат, удобный для экспорта
                var exportData = ChatHub.GlobalLatencies.Select(l => new
                {
                    Time = l.Time.ToString("o"), // ISO 8601 формат времени
                    Latency = l.Latency
                }).ToList();

                return Ok(exportData); // Возвращаем данные в формате JSON
            }
        }
    }
}
