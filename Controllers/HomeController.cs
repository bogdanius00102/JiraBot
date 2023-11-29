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

namespace KernelHelpBot.Controllers
{
   
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        //static Database db = new Database("server=localhost;user=root;database=kernelhelpbot;password=toor;charset=utf8;");
        static Database db = new Database("server=localhost;user=root;database=kernelhelpbot;password=P@ssw0rd$D;charset=utf8;");

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
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