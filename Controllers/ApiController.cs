using KernelHelpBot.Models;
using KernelHelpBot.Models.JiraRequest;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace KernelHelpBot.Controllers
{

    public class ApiController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> NewStatus()
        {
            try
            {
                using (StreamReader reader = new StreamReader(Request.Body))
                {
                    string body = await reader.ReadToEndAsync();
                    //  Console.WriteLine(body);

                    JiraGetWebhookJson json = JsonConvert.DeserializeObject<JiraGetWebhookJson>(body);

                    TelegramBot.NewStatus(json);


                    return Ok();
                }
            }
            catch (Exception ex)
            {
                // Логирование ошибок или обработка исключений здесь
                return StatusCode(911, $"Incorrect json: {ex.Message}");
            }

        }
        [HttpPost]
        public async Task<IActionResult> NewComment()
        {
            try
            {
                using (StreamReader reader = new StreamReader(Request.Body))
                {
                    string body = await reader.ReadToEndAsync();
                    //  Console.WriteLine(body);

                    JiraGetWebhookJson json = JsonConvert.DeserializeObject<JiraGetWebhookJson>(body);

                    TelegramBot.NewComment(json);


                    return Ok();
                }
            }
            catch (Exception ex)
            {
                // Логирование ошибок или обработка исключений здесь
                return StatusCode(912, $"Incorrect json: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> NewComment2()
        {
            string body = System.IO. File.ReadAllText("C:\\Users\\PC\\Desktop\\JiraBot\\JiraBot\\bin\\Debug\\net6.0\\text.txt");
            JiraGetWebhookJson json = JsonConvert.DeserializeObject<JiraGetWebhookJson>(body);

            TelegramBot.NewComment(json);

            return Ok();
        }


        [HttpPost] 
        public async Task<IActionResult> CloseTask()
        {
            try
            {
                using (StreamReader reader = new StreamReader(Request.Body))
                {
                    string body = await reader.ReadToEndAsync();
                    //  Console.WriteLine(body);

                    JiraGetWebhookJson json = JsonConvert.DeserializeObject<JiraGetWebhookJson>(body);

                    TelegramBot.CloseTask(json);


                    return Ok();
                }
            }
            catch (Exception ex)
            {
                // Логирование ошибок или обработка исключений здесь
                return StatusCode(912, $"Incorrect json: {ex.Message}");
            }
        }

    }

}

