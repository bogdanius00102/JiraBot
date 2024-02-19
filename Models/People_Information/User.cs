namespace KernelHelpBot.Models.People_Information
{
    public class User
    {

        public int id { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string email { get; set; }
        public string department { get; set; }
        public string work_position { get; set; }
        //   public string personal_phone_number { get; set; }
        public string description { get; set; }
        public TelegramData telegram_data = new TelegramData();
        public bool active { get; set; }
        public string project { get; set; }
        public string RandomCode { get; set; }
        public string TimeHealthCode { get; set; }
        public string temporary_mail { get; set; }

    }
}
