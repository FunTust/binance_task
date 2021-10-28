using binance_task.Data;
using binance_task.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace binance_task.Controllers
{
    [Route("api_v1/[controller]")]
    [ApiController]
    public class KeyController : ControllerBase
    {
        private readonly ApiDbContext _context;
        public KeyController(ApiDbContext context)
        {
            _context = context;
        }

        [Route("GetAll")]
        [HttpGet]
        public object Get() => _context.Keys.Select((c) => new
            {
                Id = c.Id,
                API_KEY = c.API_KEY,
                SECRET_KEY = c.SECRET_KEY
            }).OrderBy(b => b.Id).ToList();

        [HttpGet("GetUserData/{id}")]
        public async Task<ActionResult<API_UserData_model>> GetUserData(int id)
        {
            var keys = await _context.Keys.FindAsync(id);
            BinanceApi User = new BinanceApi(keys.API_KEY);
            API_UserData_model UserInfo = await User.GetAccountInfo(keys.SECRET_KEY);
                return UserInfo;
        }

        [HttpGet("GetLimits")]
        public async Task<ActionResult<API_rateLimits>> GetLimits()
        {
            BinanceApi User = new BinanceApi("");
            API_rateLimits LimitsInfo = await User.GetExchangeInfo();
            return LimitsInfo;
        }

        [HttpGet("GetBTC")]
        public async Task<ActionResult<API_TickerPrice>> GetBTC()
        {
            BinanceApi User = new BinanceApi("");
            API_TickerPrice BTCInfo = await User.GetTickerPrice();
            return BTCInfo;
        }

        [HttpGet("GetById/{id}")]
        public async Task<ActionResult<Keys>> GetById(int id)
        {
            var keys = await _context.Keys.FindAsync(id);

            if (keys == null)
            {
                return NotFound();
            }

            return keys;
        }

       
        [HttpPut("Put/{id}")]
        public async Task<IActionResult> UpdateKey(int id, Keys key)
        {
            if (id != key.Id)
            {
                return BadRequest();
            }

            _context.Entry(key).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!keyExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool keyExists(int id)
        {
            throw new NotImplementedException();
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteKey(int id)
        {
            var keys = await _context.Keys.FindAsync(id);
            if (keys == null)
            {
                return NotFound();
            }

            _context.Keys.Remove(keys);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Route("Post")]
        [HttpPost]
        public async Task<ActionResult<Keys>> PostTodoItem(Keys todoItem)
        {
            _context.Keys.Add(todoItem);
            await _context.SaveChangesAsync();

            //return CreatedAtAction("GetTodoItem", new { id = todoItem.Id }, todoItem);
            return CreatedAtAction(nameof(Get), new { id = todoItem.Id }, todoItem);
        }
    }
}
