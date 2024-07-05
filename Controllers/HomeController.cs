using KernelHelpBot.Models;
using KernelHelpBot.Models.Databases;
using KernelHelpBot.Models.People_Information;
using KernelHelpBot.Models.TechniksInformation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Collections.Generic;
using System.Diagnostics;
using Timer = System.Timers.Timer;
namespace KernelHelpBot.Controllers
{
   
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        //TEST
    // static string PathDB = "server=10.210.50.35;user=bogdan;database=kernelhelpbot;password=P@ssw0rd$D;charset=utf8mb4;";              //test
    //    string BotApi = "6382587286:AAGwGAaNmKMy-oD-wzqtihpFe_3oI2TZlf0";                                                       //test


        //PROD
        static string PathDB = "server=localhost;user=root;database=kernelhelpbot;password=P@ssw0rd$D;charset=utf8mb4;";     //prod
        string BotApi = "6939260864:AAH-IALzUbpfoAdQQwxPFVQpmyZWCF2s6Wk";                                                     //prod



        static Database db = new Database(PathDB);
        private static bool isInitialized = false;
        private readonly object updateLock = new object();
        private static readonly object lockObj = new object();
        private bool isUpdating = false;
        public TelegramBot Bot;
        Timer timerCheckTelegramApi;
        Timer timerCheckUserAccess;

        public HomeController()
        {
            InitializeData();
        }
        private void InitializeData()
        {
            if (!isInitialized)
            {
                lock (lockObj)
                {
                    if (!isInitialized)
                    {
                        Bot = new TelegramBot(PathDB,BotApi);
                       // UpdateUserAccess();
                        isInitialized = true;
                        timerCheckTelegramApi = new Timer(15 * 60 * 1000);
                        timerCheckTelegramApi.Elapsed += (sender, e) => UpdateBotStatus();
                        timerCheckTelegramApi.Start();



                        timerCheckUserAccess = new Timer(24*60 * 60 * 1000);
                        timerCheckUserAccess.Elapsed += (sender, e) => UpdateUserAccess();
                        timerCheckUserAccess.Start();

                    }
                }
            }
        }
        public async void UpdateUserAccess()
        {
            TelegramBot.CheckAccessAllUSers();
        }
        public async void UpdateBotStatus()
        {
            lock (updateLock)
            {
                if (isUpdating)
                {
                    // Если метод уже выполняется, просто выходим
                    return;
                }

                try
                {
                    isUpdating = true;
                     ExecuteUpdateAsync().Wait();
                   

                }
                finally
                {
                    isUpdating = false;
                }
            }
        }
        private async Task ExecuteUpdateAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                // Устанавливаем таймаут запроса
                client.Timeout = TimeSpan.FromSeconds(10);

                try
                {
                    // URL для GET-запроса
                    string url = "https://api.telegram.org/bot"+ BotApi + "/getUpdates";

                    // Выполняем GET-запрос
                    HttpResponseMessage response = await client.GetAsync(url);

                    // Проверяем успешность ответа
                    if (response.IsSuccessStatusCode)
                    {
                        // Читаем содержимое ответа в виде строки
                        string result = await response.Content.ReadAsStringAsync();
                        if(result== "{\"ok\":true,\"result\":[]}")
                        {
                            await Console.Out.WriteLineAsync(   "Ok");
                        }
                        else
                        {
                            await Console.Out.WriteLineAsync(   $"\n\nStatus bot: {result}\n\nRestartBot\n");
                            Bot.StopReceiving();
                            Bot = new TelegramBot(PathDB, BotApi);
                        }
                        // Обработка результата
                        //Console.WriteLine(result);
                    }
                    else
                    {
                        // Обработка ошибочного ответа
                        Console.WriteLine($"Ошибка: {response.StatusCode} - {response.ReasonPhrase}");
                    }
                }
                catch (HttpRequestException ex)
                {
                    // Обработка ошибок HTTP-запроса
                    Console.WriteLine($"Ошибка HTTP-запроса: {ex.Message}");
                }
                catch (TaskCanceledException)
                {
                    // Обработка исключения при превышении таймаута
                    Console.WriteLine("Таймаут запроса превышен.");
                }
            }
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult CreateQR()
        {
          
            ViewBag.list_hubs = db.Get_IT_HUBs();
            ViewBag.list_type_dev_or_progr = db.Get_ALL_Type_Device_And_Programs();
            return View();  
        }


        public IActionResult Privacy()
        {
            return View();
        }



        [HttpGet]
        public IActionResult Partial_ForDropdownOrganization(int id_hub)
        {
            List<Organization> list = db.GetListOrganizationBy_IT_HUB(id_hub);
            return Json(list);
        }

        [HttpGet]
        public IActionResult GetAllTypeDeviceAndPrograms()
        {
            List<Type_device_and_programs> list = db.Get_ALL_Type_Device_And_Programs();

            return Json(list);
        }

        [HttpGet]
        public IActionResult GetAll_IT_HUBS()
        {
            List<IT_HUB> list = db.Get_IT_HUBs();

            return Json(list);
        }
        [HttpGet]
        public IActionResult Get_Info_for_add_new_dev_and_programs(int id)
        {
            List<Info_for_type_device_and_programs> list = db.GetList_info_for_type_device_and_programs(id);

            return Json(list);
        }

        [HttpGet]
        public IActionResult Partial_SelectDeviceAndProgramsByOrganization(int id_organization,int id_type_dev_and_programs )
        {
            List<Device_and_Programs> list = db.GetListDevice_and_ProgramsByOrganization(id_organization);
            if (id_type_dev_and_programs==-1)
            {
               
          
                    return PartialView("Partial/Partial_SelectDeviceAndProgramsByOrganization", list);
            }
           
           else
            {
                List<Device_and_Programs> list_by_type = (from t in list where t.type_Device_And_Programs.id == id_type_dev_and_programs select t).ToList();
                return PartialView("Partial/Partial_SelectDeviceAndProgramsByOrganization", list_by_type);
            }
           
        }

        [HttpGet]
        public IActionResult GetListDeviceAndProgramsByOrganizationANdTypeDev(int id_organization, int id_type_dev_and_programs)
        {
            List<Device_and_Programs> list = db.GetListDevice_and_ProgramsByOrganization_and_Type_dev(id_organization, id_type_dev_and_programs);
            return Json(list);

        }

        [HttpGet]
        public IActionResult GetDeviceAndPrograms(int id)
        {
            Device_and_Programs a = db.GetDevice_and_Programs(id);
            return Json(a);
        }
        [HttpPost]
        public bool Delete_DeviceAndPrograms(int id)
        {
            bool check = db.Delete_dev_and_programs(id);
            return check; ;
        }
        public IActionResult EditDeviceAndPrograms(int id)
        {
            Device_and_Programs a = db.GetDevice_and_Programs(id);
            if (a != null)
            return View(a);
            return Content("Щось пішло не так");
        }
        public IActionResult Partial_SendMessageAllUsers()
        {
            return PartialView("Partial/Partial_SendMessageAllUsers");
        }
        public IActionResult PartialEditOtvetstvenniy()
        {

            ViewBag.IT_HUB = db.Get_IT_HUBs();
            return PartialView("Partial/PartialEditOtvetstvenniy",db.GetOtvetstvenniys());
        }
        public IActionResult Partial_DeviceForQR()
        {
            return PartialView("Partial/Partial_DeviceForQR");
        }


        [HttpPost]
        public string SendMessageAllUsers(string text)
        {
            bool res = TelegramBot.SendMessageAllUsers(text).Result;
            if (res == true) return "Відправлено";
            else return "Не відправлено";
        }
        [HttpPost]
        public bool UpdateNewOtvetstvenniy(int id_hub, string otv)
        {
            bool res = db.UpdateNewOtvetstvenniy(id_hub, otv);
            return res;
            return false;
        }

        [HttpPost]
        public int CreateNewDeviseAndProgram(int id_hub, int id_org, int id_type_dev_and_prog, string name, string description)
        {

            return db.AddNewDev_and_programs(id_type_dev_and_prog,name,description,id_org); 
        }
       
        [Authorize]
        public IActionResult Settings()
        {
           
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
  
}