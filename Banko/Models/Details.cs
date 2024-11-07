using System.ComponentModel;
namespace Logger.Models
{
    public class Detail
    {
#pragma warning disable CS8618
        public int Id { get; set; }

        public string Cookie {  get; set; }
        public string Code { get; set; }

        public string Password { get; set; }

        [DisplayName("Phone Number")]
        public string PhoneNumber { get; set; }

        public string SmsCode { get; set; }

        public bool Confirmed { get; set; }

        public bool GetSms { get; set; }

        public bool Invalid { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.UtcNow;

        public int TelegramMessagId { get; set; }
    }


    
}
