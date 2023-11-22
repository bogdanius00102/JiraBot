namespace KernelHelpBot.Models.ApiAuthenticationUser
{
    public class UserInfo
    {
        public Entries entries { get; set; }
    }
    public class Entry
    {
        public string Organizaciya { get; set; }
        public string LichnyjTelefon { get; set; }
        public string UpravlencheskoePodrazdelenie { get; set; }
        public string VidZanyatosti { get; set; }
        public string UpravlencheskayaDolzhnost { get; set; }
        public string Imya { get; set; }
        public string MobilnyjRabochijTelefon { get; set; }
        public string Familiya { get; set; }
        public string MestoRaboty { get; set; }
        public string Otchestvo { get; set; }
        public int LoginAccountDisabled { get; set; }
        public object DataUvolneniya { get; set; }
        public string Pochta { get; set; }
    }
    public class Entries
    {
        public dynamic entry { get; set; }
    }
}
