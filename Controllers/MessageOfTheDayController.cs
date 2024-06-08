using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson.Serialization.Attributes;
using MotdApiDotnet.Models;
using MotdApiDotnet.Services;

namespace MotdApiDotnet.Controllers
{
    [Route("/")]
    [ApiController]
    public class MessageOfTheDayController : ControllerBase
    {
        //
        //  Classes
        //

        public class CreateMotdBody
        {
            [BsonElement("message")]
            public string Message { get; set; } = null!;
        }

        public class UpdateMotdBody
        {
            [BsonElement("message")]
            public string Message { get; set; } = null!;
        }

        //
        //  Variables
        //

        private readonly MessageOfTheDayService service;

        //
        //  Constructor
        //

        public MessageOfTheDayController(MessageOfTheDayService service)
        {
            this.service = service;
        }

        //
        //  Routes
        //

        // GET: /
        [HttpGet]
        public async Task<ActionResult<MessageOfTheDayItem>> GetLatestMotd()
        {
            var motd = await service.GetLatestAsync();
            if (motd == null) return StatusCode(404);

            return motd;
        }

        // POST: /
        [HttpPost]
        public async Task<ActionResult<MessageOfTheDayItem>> PostMotd([FromBody] CreateMotdBody body)
        {
            var motd = await service.CreateAsync(body.Message);
            if (motd == null) return StatusCode(400);

            return motd;
        }

        // GET: /6663730d73d66868453f5990
        [HttpGet("{id}")]
        public async Task<ActionResult<MessageOfTheDayItem>> GetSpecificMotd(string id)
        {
            var motd = await service.GetAsync(id);
            if (motd == null) return StatusCode(404);

            return motd;
        }

        // PATCH: /6663730d73d66868453f5990
        [HttpPost("{id}")]
        public async Task<ActionResult<MessageOfTheDayItem>> PatchMotd(string id, [FromBody] UpdateMotdBody body)
        {
            var motd = await service.UpdateAsync(id, body.Message);
            if (motd == null) return StatusCode(404);

            return motd;
        }

        // DELETE: /6663730d73d66868453f5990
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMotd(string id)
        {
            var didRemove = await service.RemoveAsync(id);
            if (!didRemove) return StatusCode(404);
            return StatusCode(200);
        }

        // GET: /history
        [HttpGet("history")]
        public async Task<ActionResult<HistoryPage<MessageOfTheDayItem>>> GetOldMotds([FromQuery] string? previousLastId = null, [FromQuery] int pageSize = 8)
        {
            var pageItems = await service.ListPageAsync(pageSize, previousLastId);
            return new HistoryPage<MessageOfTheDayItem>()
            {
                LastId = pageItems.Count > 0 ? pageItems.Last().Id : null,
                Items = pageItems
            };
        }
    }
}
