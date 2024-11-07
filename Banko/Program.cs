using Logger.Data;
using Microsoft.AspNetCore.Antiforgery;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);


var botSection = builder.Configuration.GetSection(nameof(BotConfiguration));
var opt = botSection.Get<BotConfiguration>();
if (string.IsNullOrWhiteSpace(opt.HostAddress))
{
    throw new Exception("GGG");
}
builder.Services.Configure<BotConfiguration>(botSection);
builder.Services.AddDbContext<ApplicationDbContext>();
builder.Services.AddAntiforgery(options =>
{
    options.Cookie = new CookieBuilder()
    {
        Name = "_goosebumps"
    };
});
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient("telegram_bot_client")
                .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
                {
                    BotConfiguration? botConfig = botSection.Get<BotConfiguration>();
                    TelegramBotClientOptions options = new(botConfig!.BotToken);
                    Console.WriteLine(botConfig.HostAddress);
                    return new TelegramBotClient(options, httpClient);
                });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();


public class BotConfiguration
{
#pragma warning disable CS8618
    public string BotToken { get; set; }
    public long AdminId {  get; set; }

    public string HostAddress { get; set; }
}