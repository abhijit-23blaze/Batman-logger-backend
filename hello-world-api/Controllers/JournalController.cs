using Microsoft.AspNetCore.Mvc;
using hello_world_api.Models;
using hello_world_api.Services;

namespace hello_world_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JournalController : ControllerBase
    {
        private readonly JournalService _journalService;

        public JournalController(JournalService journalService)
        {
            _journalService = journalService;
        }

        // GET api/journal?limit=10
        [HttpGet]
        public ActionResult<IEnumerable<JournalEntry>> GetEntries([FromQuery] int limit = 0)
        {
            return _journalService.GetEntries(limit);
        }

        // GET api/journal/date/2023-05-20
        [HttpGet("date/{date}")]
        public ActionResult<IEnumerable<JournalEntry>> GetEntriesByDate(DateTime date)
        {
            return _journalService.GetEntriesByDate(date);
        }

        // POST api/journal
        [HttpPost]
        public ActionResult<JournalEntry> AddEntry([FromBody] EntryRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Content))
            {
                return BadRequest("Content cannot be empty");
            }

            var entry = _journalService.AddEntry(request.Content);
            return CreatedAtAction(nameof(GetEntries), new { id = entry.Id }, entry);
        }
    }

    public class EntryRequest
    {
        public string Content { get; set; } = string.Empty;
    }
} 