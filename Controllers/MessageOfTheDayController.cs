using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotdApiDotnet.Models;
using MotdApiDotnet.Services;

namespace MotdApiDotnet.Controllers
{
    [Route("/")]
    [ApiController]
    public class MessageOfTheDayController : ControllerBase
    {
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
            if (motd == null) return NotFound();

            return motd;
        }

        // POST: /
        [HttpPost]
        public async Task<ActionResult<MessageOfTheDayItem>> PostMotd(string message)
        {
            var motd = new MessageOfTheDayItem() { Message = message };
            await service.CreateAsync(motd);

            // TODO - Modify CreateAsync to return the created object
            // then return below
            return BadRequest();
        }

        // GET: /6663730d73d66868453f5990
        [HttpGet("{id}")]
        public async Task<ActionResult<MessageOfTheDayItem>> GetSpecificMotd(string id)
        {
            var motd = await service.GetAsync(id);
            if (motd == null) return NotFound();

            return motd;
        }

        // PATCH: /6663730d73d66868453f5990
        [HttpPost("{id}")]
        public async Task<ActionResult<MessageOfTheDayItem>> PatchMotd(string id, string message)
        {
            // TODO - we need a better patch function on the service
            var motd = await service.GetAsync(id);
            if (motd == null) return NotFound();

            // Update the message in the DB
            motd.Message = message;
            await service.UpdateAsync(id, motd);

            // TODO - modify UpdateAsync to return the modified object?
            return motd;
        }

        // DELETE: /6663730d73d66868453f5990
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMotd(string id)
        {
            // TODO - Modify RemoveAsync to return if the object existed or not
            await service.RemoveAsync(id);
            return Ok();
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
