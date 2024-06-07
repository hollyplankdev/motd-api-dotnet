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

        public MessageOfTheDayController(MessageOfTheDayService service)
        {
            this.service = service;
        }

        // GET: /
        [HttpGet]
        public async Task<ActionResult<MessageOfTheDayItem>> GetLatestMotd()
        {
            return BadRequest();
        }

        // POST: /
        [HttpPost]
        public async Task<ActionResult<MessageOfTheDayItem>> PostMotd()
        {
            return BadRequest();
        }

        // GET: /6663730d73d66868453f5990
        [HttpGet("{id}")]
        public async Task<ActionResult<MessageOfTheDayItem>> GetSpecificMotd(string id)
        {
            return BadRequest();
        }

        // PATCH: /6663730d73d66868453f5990
        [HttpPost("{id}")]
        public async Task<ActionResult<MessageOfTheDayItem>> PatchMotd(string id)
        {
            return BadRequest();
        }


        // DELETE: /6663730d73d66868453f5990
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMotd(string id)
        {
            return BadRequest();
        }

        // GET: /history
        [HttpGet("history")]
        public async Task<ActionResult<HistoryPage<MessageOfTheDayItem>>> GetOldMotds()
        {
            return BadRequest();
        }
    }
}
