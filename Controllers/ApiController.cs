﻿using KernelHelpBot.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace KernelHelpBot.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApiController: ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> NewComment ()
        {
            try
            {
                using (StreamReader reader = new StreamReader(Request.Body))
                {
                    string body = await reader.ReadToEndAsync();
                    TelegramBot.NewComment(body);
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                // Логирование ошибок или обработка исключений здесь
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
      
        }

    }
