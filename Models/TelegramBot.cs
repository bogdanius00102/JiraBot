using KernelHelpBot.Models.AD;
using KernelHelpBot.Models.ApiAuthenticationUser;
using KernelHelpBot.Models.Databases;
using KernelHelpBot.Models.JiraRequest;
using KernelHelpBot.Models.People_Information;
using KernelHelpBot.Models.TechniksInformation;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.VisualBasic;
using MySqlX.XDevAPI.Common;
using System.ComponentModel;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Runtime.Intrinsics.X86;
using System.Text.RegularExpressions;
using System.Web;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using static System.Net.Mime.MediaTypeNames;
using User = KernelHelpBot.Models.People_Information.User;
namespace KernelHelpBot.Models
{
    public  class TelegramBot
    {

        static TelegramBotClient Bot;
        
        static string FirstTextMessage = "Раді Вас бачити. Натисніть \"Поділитися номером телефону\", щоб я побачив хто Ви.";
        static Database db;
        public static string TextDovidnuk = "Призначення боту – забезпечити співробітникам Компанії швидкий і зручний доступ до служби ІТ підтримки – створити запит, переглянути його статус, додати коментар чи графічне зображення.\r\n \r\nЗапити створюються за допомогою кнопок на головному екрані:\r\nКнопка \"🔥 У мене не працює\" – коли щось не працює: ноутбук, принтер, сканер, програмне забезпечення 1С, КНО, Navision, Outlook, тощо.\r\nКнопка \"💻 Хочу замовити обладнання\" – коли потрібно замовити ІТ обладнання: мишку, клавіатуру, додатковий монітор, килимок, гарнітуру, тощо.\r\nКнопка \"❓ Хочу запитати\" – якщо потрібна консультація з приводу роботи ІТ сервісів Компанії.\r\n \r\nПри створенні запиту необхідно чітко і лаконічно описати проблему, за необхідності додати фото (якщо додається фото – текст запиту потрібно писати в одному повідомлені з фото).\r\n \r\nКнопка \"🗂 Мої запити\" - відображає створені Вами запити і їх статус: в роботі, очікує виконання.\r\nКнопка \"ℹ️ Переглянути детально\" – відобразить детальну інформацію обраного запиту.\r\nКнопка \"✍️ Додати коментар\" – дозволяє додати коментар до запиту.\r\n \r\nПри зміні статусу або при новому коментарі від фахівців ІТ підтримки – бот Вас проінформує. Натиснувши кнопку \"✍️ Відповісти\" – ви зможете відповісти на такий коментар.\r\n \r\nЯкщо бот не функціонує належним чином - просимо сповістити службу ІТ підтримки одним із інших способів:\r\nпортал https://sd.kernel.ua/plugins/servlet/theme/portal/2\r\nелектронна пошта sd@kernel.ua\r\nтелефон +380 99 100 3 000, 080 040 8 848.";
        public  TelegramBot(string PathDB, string BotApi)
        {
            db = new Database(PathDB);
            Bot = new TelegramBotClient(BotApi);
           
            Bot.StartReceiving(Update, Error);
        }
     public  void StopReceiving()
        {
            Console.WriteLine("StopReceiving!");
           
                Bot.StartReceiving(null);
           
           
        }
         public  async static Task<bool> SendMessageAllUsers(string text)
        {
            var replyKeyboard = new ReplyKeyboardMarkup(
                                      new[]
                                          {
                                              new []
                                           {

                                               new KeyboardButton ("🔥 У мене не працює"),


                                            },
                                       new []
                                           {
                                            new KeyboardButton ("💻 Хочу замовити обладнання"),
                                                 new KeyboardButton ("❓ Хочу запитати"),
                                              //   new KeyboardButton ("Запит по QR коду"),

                                            },

                                        new []
                                           {
                                                new KeyboardButton ("🗂 Мої запити"),
                                                    new KeyboardButton ("📖 Довідник"),

                                            },
                                          }
                                      );
            replyKeyboard.ResizeKeyboard = true;
            try
            {
                List<User> users = await db.GetAllUsers();

                foreach (var user in users)
                { 
                    if(user.active==true)
                    try
                    {
                            db.Update_options_for_create_task(user.telegram_data.telegram_id, "");
                        await Bot.SendTextMessageAsync(user.telegram_data.telegram_id, text, replyMarkup:replyKeyboard);
                          
                            Console.WriteLine($"{user.telegram_data.telegram_id} {user.email} - Sent");
                    }
                    catch (Telegram.Bot.Exceptions.ApiRequestException ex)
                    {
                        // Handle the case where the message couldn't be sent (e.g., user blocked the bot)
                        Console.WriteLine($"{user.telegram_data.telegram_id} {user.email} - Not Sent: {ex.Message}");
                    }
                    else Console.WriteLine($"{user.telegram_data.telegram_id} - Not Sent ACTIVE = FALSE");
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }

        }
         async static Task Update(ITelegramBotClient bot, Update e, CancellationToken arg3)
        {
            // return;
            if (e.Message != null)
            {
                User u = db.getUserBytelegramId(e.Message.From.Id);

                if (e.Message.Contact != null)
                {


                    if (e.Message.Contact.UserId == e.Message.From.Id )
                    {
                        await Console.Out.WriteLineAsync($"{DateTime.Now} {e.Message.From.Id} {e.Message.From.Username} {e.Message.From.FirstName} {e.Message.From.LastName} sendPhone: {e.Message.Contact.PhoneNumber}");
                       // await Bot.SendTextMessageAsync(id_admin_chat, $"{DateTime.Now} {e.Message.From.Id} {e.Message.From.Username} {e.Message.From.FirstName} {e.Message.From.LastName} sendPhone: {e.Message.Contact.PhoneNumber}");
                        if (u==null)
                        ForGetContact(e);
                        return;
                    }
                    else
                    {
                        ReplyKeyboardMarkup replyKeyboard = new ReplyKeyboardMarkup((new[]
                             {
                    new[]
                        {
                              KeyboardButton.WithRequestContact ("Поділитися номером телефону"),



                        }
                                 }));
                        replyKeyboard.ResizeKeyboard = true;
                        await Bot.SendTextMessageAsync(e.Message.From.Id, "Ви відправили не свій номер телефону. Натисніть на кнопку \"Поділитися номером телефону\"", replyMarkup: replyKeyboard);
                        await Console.Out.WriteLineAsync($"{DateTime.Now} {e.Message.From.Id} {e.Message.From.Username} {e.Message.From.FirstName} {e.Message.From.LastName} sendPhone: {e.Message.Contact.PhoneNumber}. Номер телефона не власник цього ТЕЛЕГРАМ акаунту");
                       // await Bot.SendTextMessageAsync(id_admin_chat, $"{DateTime.Now} {e.Message.From.Id} {e.Message.From.Username} {e.Message.From.FirstName} {e.Message.From.LastName} sendPhone: {e.Message.Contact.PhoneNumber}. Номер телефона не власник цього ТЕЛЕГРАМ акаунту");
                    }

                }
              else  if (e.Message.Text != null)
                {
                    //if(e.Message.Chat.Id== id_admin_chat)
                    //{
                    //    AdminChat(e); return;
                    //}
                    if (e.Message.Text == "/start")
                    {

                        ForStart(e); return;

                    }
                    else if (u!=null)
                    {
                       
                        if (u.active==true)
                        ForMessageText(e);
                        else
                        {
                            if(e.Message.Text.Contains("@kernel.ua") || e.Message.Text.Contains("@kernel.local") || e.Message.Text.Contains("@yztk.ua")  )
                            {
                                String SendMailFrom = "b.doroshkov@gmail.com";
                                String SendMailTo = e.Message.Text;
                                String SendMailSubject = "Подтверждение доступа в телеграм бот Kernel Digital. IT Service Desk Bot";
                                String SendMailBody = "Код 12345";

                                try
                                {
                                    SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com", 587);
                                    SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                                    MailMessage email = new MailMessage();
                                    // START
                                    email.From = new MailAddress(SendMailFrom);
                                    email.To.Add(SendMailTo);
                                    email.CC.Add(SendMailFrom);
                                    email.Subject = SendMailSubject;
                                    email.Body = SendMailBody;
                                    //END
                                    SmtpServer.Timeout = 5000;
                                    SmtpServer.EnableSsl = true;
                                    SmtpServer.UseDefaultCredentials = false;
                                    SmtpServer.Credentials = new NetworkCredential(SendMailFrom, "yrynihltjsdkwgsu");
                                    SmtpServer.Send(email);

                                    Console.WriteLine("Email Successfully Sent");
                                    await Bot.SendTextMessageAsync(e.Message.From.Id, "Отправил вам на почту код.");

                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.ToString());
                                    Console.ReadKey();
                                }
                               
                                return;
                            }
                            else
                                
                                
                                NetPravNaBota(e);
                           
                        }
                        return;
                    }
                    else
                    {
                        NetPravNaBota(e);
                        return;
                    }
                  
                }
               
                else if (e.Message.Photo != null && u!=null)
                {
                    GetPhoto(e,u);
                    return;
                 

                }



            }
            else if (e.CallbackQuery != null )
            {
                User u = db.getUserBytelegramId(e.CallbackQuery.From.Id);
                if (u!=null)
                    if(u.active==true)
                ForCallbackQuery(e,u);
            }

        }
        static async void GetPhoto(Update e, User u)
        {
            if (e.Message.MediaGroupId == null)
            {

                string text_options_for_create_task = db.Get_options_for_create_task(e.Message.From.Id).Result;
                if (text_options_for_create_task.Contains("new_comment"))
                {
                    string key_task = text_options_for_create_task.Replace("new_comment", "");
                    var fileId = e.Message.Photo[e.Message.Photo.Count()-1].FileId;
                    var file = await Bot.GetFileAsync(fileId);

                    using (MemoryStream imageStream = new MemoryStream())
                    {
                        await Bot.DownloadFileAsync(file.FilePath, imageStream);
                        byte[] imageBytes = imageStream.ToArray();

                        // Вызываем метод для добавления комментария в Jira с изображением
                        //Jira.AddPhotoCommentToIssue(key_task, e.Message.Caption, imageBytes, file.FilePath);
                        InlineKeyboardMarkup replyMarkup = new InlineKeyboardMarkup(new[]{
                 new []
            {
                 InlineKeyboardButton.WithCallbackData("✅ Відправити","create_Photo_comment"+key_task),
            },
             new []
            {
                 InlineKeyboardButton.WithCallbackData("❌ Скасувати","delete_comment"),
             },

               }

                 );
                        
                        await Bot.SendPhotoAsync(e.Message.From.Id, new InputOnlineFile(new MemoryStream(imageBytes)), caption: "Ваш коментар, відправити?\n"+e.Message.Caption, replyMarkup: replyMarkup);

                    }


                }
                else if (text_options_for_create_task.Contains("❓ Хочу запитати") || text_options_for_create_task.Contains("💻 Хочу замовити обладнання") || text_options_for_create_task.Contains("🔥 У мене не працює"))
                {
                    var fileId = e.Message.Photo[e.Message.Photo.Count() - 1].FileId;
                    var file = await Bot.GetFileAsync(fileId);

                    using (MemoryStream imageStream = new MemoryStream())
                    {
                        await Bot.DownloadFileAsync(file.FilePath, imageStream);
                        byte[] imageBytes = imageStream.ToArray();

                        InlineKeyboardMarkup replyMarkup = new InlineKeyboardMarkup(new[]{
                 new []
            {
                 InlineKeyboardButton.WithCallbackData("✅ Відправити","create_with_photo_task"),
            },
             new []
            {
                 InlineKeyboardButton.WithCallbackData("❌ Скасувати","delete_task"),
             },

               }

                 );

                        await Bot.SendPhotoAsync(e.Message.From.Id, new InputOnlineFile(new MemoryStream(imageBytes)), caption: $"Перевірте та підтвердіть заявку\n{text_options_for_create_task}\n{e.Message.Caption}" , replyMarkup: replyMarkup); ;

                    }




                }
            }
            //Не альбом, а одно фото
            else
            {
                await Bot.SendTextMessageAsync(e.Message.From.Id, "На даний момент бот працює лише з 1 зображенням");
            }



        }

         static async void ForMessageText(Update e)
        {
             Console.Out.WriteLineAsync($"{DateTime.Now} {e.Message.From.Id} {e.Message.From.Username} {e.Message.From.FirstName} {e.Message.From.LastName} send: {e.Message.Text}");
           // await Bot.SendTextMessageAsync(id_admin_chat, $" {e.Message.From.Id} {e.Message.From.Username} {e.Message.From.FirstName} {e.Message.From.LastName} send: {e.Message.Text}");


            switch (e.Message.Text)
            {

               
                case "❓ Хочу запитати":
                    CreateNewRequest(e);return;
                   
                case "💻 Хочу замовити обладнання":
                    CreateNewRequest(e);
                    return;
                case "🔥 У мене не працює":
                    CreateNewRequest(e);
                    return;

                case "📖 Довідник":
                    { bool res = db.Update_options_for_create_task(e.Message.From.Id, "").Result; }
                    await Bot.SendTextMessageAsync(e.Message.From.Id, TextDovidnuk);
                    //Dovidnuk(e);
                    return;
                case "🗂 Мої запити":
                    { bool res = db.Update_options_for_create_task(e.Message.From.Id, "").Result; }
                    MyNoResolvedRequest(e);
                    return;
                case "Запит по QR коду":
                    { bool res = db.Update_options_for_create_task(e.Message.From.Id, "").Result; }
                    await Bot.SendTextMessageAsync(e.Message.From.Id, "Відскануйте відповідний QR код, для автоматичного створення запиту.");
                    return;


            }
            
             if (e.Message.Text.Contains("/start QRProblem"))
            {
                { bool res = db.Update_options_for_create_task(e.Message.From.Id, "").Result; }
                SearchQRProblem(e);return;
            }
            string text_options_for_create_task = db.Get_options_for_create_task(e.Message.From.Id).Result;
             if (text_options_for_create_task.Contains("❓ Хочу запитати") || text_options_for_create_task.Contains("💻 Хочу замовити обладнання") || text_options_for_create_task.Contains("🔥 У мене не працює") )
            {
                Text_For_Create_New_Request(e);return;
            }
             if(text_options_for_create_task.Contains("new_comment"))
            {
                string ket_task = text_options_for_create_task.Replace("new_comment","");
                Text_For_Create_New_Comment(e, ket_task); return;
            }
        }
         static async void SendStartKeyboard(User u)
        {
          
            var replyKeyboard = new ReplyKeyboardMarkup(
                                        new[]
                                            {
                                              new []
                                           {

                                               new KeyboardButton ("🔥 У мене не працює"),

                                              
                                            },
                                       new []
                                           {
                                            new KeyboardButton ("💻 Хочу замовити обладнання"),
                                                 new KeyboardButton ("❓ Хочу запитати"),
                                              //   new KeyboardButton ("Запит по QR коду"),

                                            },

                                        new []
                                           {
                                                new KeyboardButton ("🗂 Мої запити"),
                                                    new KeyboardButton ("📖 Довідник"),

                                            },
                                            }
                                        );
            replyKeyboard.ResizeKeyboard = true;
            if (u.name != null && u.name != "")
            {
                await Bot.SendTextMessageAsync(u.telegram_data.telegram_id, $"Шановний {u.surname} {u.name}, раді Вас бачити з нами", replyMarkup: replyKeyboard);
                db.Update_options_for_create_task(u.telegram_data.telegram_id, "");
            }
            else
            {
                await Bot.SendTextMessageAsync(u.telegram_data.telegram_id, "Раді Вас бачити з нами", replyMarkup: replyKeyboard);
            }

        }
         static async void ForCallbackQuery(Update e, User u)//Отправка запроса в жиру по QR
        {
           
            if (e.CallbackQuery.Data.Contains("Inl_kb_problemId:"))
            {
                string text_message = e.CallbackQuery.Message.Text;
                text_message = text_message.Replace("Створення запиту.","");
                text_message = text_message.Replace("Оберіть підходящий варіант проблеми", "");
                string callb_data = e.CallbackQuery.Data;
                string[] parts = callb_data.Split(new char[] { ',', ':' });

                int inlKbProblemId = 0;
                int organizationId = 0;

                // Поиск значений для Inl_kb_problemId и organizationId
                for (int i = 0; i < parts.Length; i++)
                {
                    if (parts[i].Trim() == "Inl_kb_problemId")
                    {
                        inlKbProblemId = Convert.ToInt32( parts[i + 1]);
                    }
                    else if (parts[i].Trim() == "organizationId")
                    {
                        organizationId =Convert.ToInt32( parts[i + 1]);
                    }
                }
                
               
                Organization or=db.GetOrganization(organizationId);
                IT_HUB hub = db.Get_IT_HUB_BY_ORGANIZATION_ID(or.id).Result;
                Problem_for_type_device_and_programs problem = db.GetProblems_device_and_programsByProblemId(inlKbProblemId);
                if (problem == null) return;
                text_message = $"{hub.name}\nПідприємство: {or.name}\n" + text_message;
                await Bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id, "Створюємо запит!");
               
                //return;
              //  12345
                ResponseOnCreateJiraTask result;
               if (u.email!=null && u.email!="")
                    result = Jira.CreateNewTask(e.CallbackQuery.From.Id,problem.type_Device_And_Programs.name + " " + problem.text_problem, text_message, u.email, u.project).Result;
                else
                {
                    string text = text_message+"\nКористувач: " + u.name + " " + u.surname + " " + u.telegram_data.phone_number + " WorkPosition: " + u.work_position + "\nTelegramId: " + u.telegram_data.telegram_id;

                    result = Jira.CreateNewTask(e.CallbackQuery.From.Id,problem.type_Device_And_Programs.name + " " + problem.text_problem, text, "t-bot_sd@kernel.ua", u.project).Result;

                
                }
                if (result!=null)
                {
                  
                    string url_create_task = "https://sd.kernel.ua/plugins/servlet/theme/portal/2/" + result.key;

                    await Bot.EditMessageTextAsync(e.CallbackQuery.From.Id, e.CallbackQuery.Message.MessageId, "ЗАПИТ СТВОРЕН: <b>" + result.key + "</b>\n" + problem.type_Device_And_Programs.name + " " + problem.text_problem +"\n"+ text_message, parseMode: ParseMode.Html,replyMarkup:new InlineKeyboardMarkup(new InlineKeyboardButton[] {InlineKeyboardButton.WithWebApp(""+result.key,new WebAppInfo() { Url=url_create_task}) }));

                   // await Bot.SendTextMessageAsync(id_admin_chat,$"Пользователь {u.name} {u.surname} {u.email} создал заявку: {problem.text_problem} \n{result.key}");
                   
                    DateTime currentTime = DateTime.Now;
                    bool isWeekday = currentTime.DayOfWeek >= DayOfWeek.Monday && currentTime.DayOfWeek <= DayOfWeek.Friday;
                    bool isWorkingHours = currentTime.TimeOfDay >= new TimeSpan(8, 0, 0) && currentTime.TimeOfDay < new TimeSpan(18, 0, 0);
                    if(isWeekday==false || isWorkingHours==false )
                    {                       
                        List<Organization> organizations = db.GetListOrganization();
                        int id = 0;
                        id = (from t in organizations where t.name == u.work_position select t.id).FirstOrDefault();
                      //  IT_HUB hub = db.Get_IT_HUB_BY_ORGANIZATION_ID(id).Result;
                        await Bot.SendTextMessageAsync(e.CallbackQuery.From.Id, "Запит створений у неробочий час. Якщо проблема критична і впливає на виробничі процеси - будь-ласка, зателефонуйте відповідальному ІТ фахівцію: " + hub.otvetstvenniy + " " + hub.phone_number);
                        }
                    
                }
                else
                {
                   // await Bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id, "Щось пішло не так");
                    await Console.Out.WriteLineAsync(   );
                }

            }
            else if(e.CallbackQuery.Data== "create_task")
            {
                string tema = db.Get_options_for_create_task(e.CallbackQuery.From.Id).Result;
                if (tema == "") return;
                string text = e.CallbackQuery.Message.Text.Replace(tema, "");
                text = text.Replace("Перевірте та підтвердіть заявку\n", "");
                
                bool res = db.Update_options_for_create_task(e.CallbackQuery.From.Id, "").Result;
                {
                        ResponseOnCreateJiraTask result;
                       if (u.email != "" && u.email != null)
                          result = Jira.CreateNewTask(e.CallbackQuery.From.Id, tema, text, u.email, u.project).Result;
                      else
                       {

                          text += "\nКористувач: " + u.name + " " + u.surname + " " + u.telegram_data.phone_number + " WorkPosition: "+u.work_position+"\nTelegramId: " + u.telegram_data.telegram_id;
                           result = Jira.CreateNewTask(e.CallbackQuery.From.Id, tema, text, "t-bot_sd@kernel.ua", u.project).Result;
                       }
                        if (result != null)
                      {

                            string url_create_task = "https://sd.kernel.ua/plugins/servlet/theme/portal/2/" + result.key;
                           Console.WriteLine("Створений  Запит\n" + tema + " : " + text+"\n"+url_create_task);
                          try
                           {
                            var inlinekeyboard = new InlineKeyboardMarkup(new[] {
                              new[]    {

                                                InlineKeyboardButton.WithCallbackData("✍️ Додати коментар","vidpovistu" + result.key),

                                         },
                              new[]    {
                                InlineKeyboardButton.WithCallbackData("ℹ️ Переглянути детально","detalno"+result.key)
                                         } });
                            await Bot.SendTextMessageAsync(
                                    chatId: e.CallbackQuery.From.Id, 
                                   text: "Запит створен: <b>" + result.key + "</b>\n"
                                + tema + "\n" + text, 
                                    parseMode: ParseMode.Html,
                                     replyMarkup: inlinekeyboard,
                                    
                               replyToMessageId:e.CallbackQuery.Message.MessageId
                                    );
                                DateTime currentTime = DateTime.Now;
                                bool isWeekday = currentTime.DayOfWeek >= DayOfWeek.Monday && currentTime.DayOfWeek <= DayOfWeek.Friday;
                                bool isWorkingHours = currentTime.TimeOfDay >= new TimeSpan(8, 0, 0) && currentTime.TimeOfDay < new TimeSpan(18, 0, 0);
                                if (isWeekday == false || isWorkingHours == false)
                                {
                                    List<Organization> organizations = db.GetListOrganization();
                                    int id = 0;
                                    id = (from t in organizations where t.name == u.work_position select t.id).FirstOrDefault();
                                   IT_HUB hub = db.Get_IT_HUB_BY_ORGANIZATION_ID(id).Result;
                                   await Bot.SendTextMessageAsync(e.CallbackQuery.From.Id, "Запит створений у неробочий час. Якщо проблема критична і впливає на виробничі процеси - будь-ласка, зателефонуйте відповідальному ІТ фахівцію: " + hub.otvetstvenniy + " " + hub.phone_number);
                               }
                           }
                           catch (Exception ex) { Console.WriteLine("359: "+ex.Message); }




                    }

                }
                await Bot.DeleteMessageAsync(e.CallbackQuery.From.Id, e.CallbackQuery.Message.MessageId);

            }
            
                else if (e.CallbackQuery.Data == "create_with_photo_task")
            {
                await Bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id, "Створення заявки...");
                string tema = db.Get_options_for_create_task(e.CallbackQuery.From.Id).Result;
                if (tema == "") return;

                var fileId = e.CallbackQuery.Message.Photo[e.CallbackQuery.Message.Photo.Count()-1].FileId;
                var file = await Bot.GetFileAsync(fileId);
                string text = "";
                string photo_name = "";
                int lastSlashIndex = file.FilePath.LastIndexOf('/');

                if (lastSlashIndex != -1 && lastSlashIndex < file.FilePath.Length - 1)
                {
                    photo_name = file.FilePath.Substring(lastSlashIndex + 1);

                }

                 text = e.CallbackQuery.Message.Caption + " !" + photo_name + "|" + photo_name + "!";
                text = text.Replace("Перевірте та підтвердіть заявку\n", "");
                text =text.Replace(tema, "");

                bool res = db.Update_options_for_create_task(e.CallbackQuery.From.Id, "").Result;
                {
                    ResponseOnCreateJiraTask result;
                    if (u.email != "" && u.email != null)
                        result = Jira.CreateNewTask(e.CallbackQuery.From.Id, tema, text, u.email, u.project).Result;
                    else
                    {

                        text += "\nКористувач: " + u.name + " " + u.surname + " " + u.telegram_data.phone_number + " WorkPosition: " + u.work_position + "\nTelegramId: " + u.telegram_data.telegram_id;
                        result = Jira.CreateNewTask(e.CallbackQuery.From.Id, tema, text, "t-bot_sd@kernel.ua", u.project).Result;
                    }
                    if (result != null && result.key != null)
                    {

                        byte[] imageBytes;
                         using (MemoryStream imageStream = new MemoryStream())
                        {
                            await Bot.DownloadFileAsync(file.FilePath, imageStream);
                            imageBytes = imageStream.ToArray();

                            bool result_send_photo = Jira.AddPhotoCommentToIssue(result.key, imageBytes, file.FilePath).Result;
                            
                        }


                        string url_create_task = "https://sd.kernel.ua/plugins/servlet/theme/portal/2/" + result.key;
                        Console.WriteLine("Створений  Запит\n" + tema + " : " + text + "\n" + url_create_task);
                        try
                        {
                            var inlinekeyboard = new InlineKeyboardMarkup(new[] {
                              new[]    {

                                                InlineKeyboardButton.WithCallbackData("✍️ Додати коментар","vidpovistu" + result.key),

                                         },
                              new[]    {
                                InlineKeyboardButton.WithCallbackData("ℹ️ Переглянути детально","detalno"+result.key)
                                         } });
                            await Bot.SendPhotoAsync(

                                  chatId: e.CallbackQuery.From.Id,
                                   caption: "Запит створен: <b>" + result.key + "</b>\n"
                            + tema + "\n" + text,
                                   photo: new InputOnlineFile(new MemoryStream(imageBytes)),
                                    parseMode: ParseMode.Html,
                                replyMarkup: inlinekeyboard,
                               replyToMessageId: e.CallbackQuery.Message.MessageId



                                );




                            DateTime currentTime = DateTime.Now;
                            bool isWeekday = currentTime.DayOfWeek >= DayOfWeek.Monday && currentTime.DayOfWeek <= DayOfWeek.Friday;
                            bool isWorkingHours = currentTime.TimeOfDay >= new TimeSpan(8, 0, 0) && currentTime.TimeOfDay < new TimeSpan(18, 0, 0);
                            if (isWeekday == false || isWorkingHours == false)
                            {
                                List<Organization> organizations = db.GetListOrganization();
                                int id = 0;
                                id = (from t in organizations where t.name == u.work_position select t.id).FirstOrDefault();
                                if(id==0)
                                {
                                    await Bot.SendTextMessageAsync(494277044, $"Користувач {u.name} {u.surname} створив заявку у неробочий час, але відповідність його workPosition серед ITHub не знайдена. Його WorkPosition:{u.work_position}");

                                    Console.WriteLine($"Користувач {u.name} {u.surname} створив заявку у неробочий час, але відповідність його workPosition серед ITHub не знайдена. Його WorkPosition:{u.work_position}");


                                    return;
                                }    
                                IT_HUB hub = db.Get_IT_HUB_BY_ORGANIZATION_ID(id).Result;
                                await Bot.SendTextMessageAsync(e.CallbackQuery.From.Id, "Запит створений у неробочий час. Якщо проблема критична і впливає на виробничі процеси - будь-ласка, зателефонуйте відповідальному ІТ фахівцію: " + hub.otvetstvenniy + " " + hub.phone_number);
                            }
                        }
                        catch (Exception ex) { Console.WriteLine("359: " + ex.Message); }




                    }

                }
                await Bot.DeleteMessageAsync(e.CallbackQuery.From.Id, e.CallbackQuery.Message.MessageId);

            }
            else if (e.CallbackQuery.Data == "delete_task")
            {
               // bool res = db.Update_options_for_create_task(e.CallbackQuery.From.Id, "").Result;

                await Bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id, "Видалено");
                await Bot.DeleteMessageAsync(e.CallbackQuery.From.Id, e.CallbackQuery.Message.MessageId);

            }
            else if(e.CallbackQuery.Data.Contains("create_comment"))
            {
                try
                {
                    string key_task = e.CallbackQuery.Data.Replace("create_comment","");
                    string text_comment = e.CallbackQuery.Message.Text.Replace("Ваш коментар, відправити?\n", "");
                    bool res = db.Update_options_for_create_task(e.CallbackQuery.From.Id, "").Result;
                    string text = $"{u.name} {u.surname} {u.email}: "+ text_comment;
                    Jira.AddCommentToIssue(key_task, text);
                    await Bot.EditMessageTextAsync(e.CallbackQuery.From.Id, e.CallbackQuery.Message.MessageId, $"Ваш коментар: *{text_comment}* додано до заявки.", parseMode: ParseMode.Markdown);


                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                }
            else if (e.CallbackQuery.Data == "delete_comment")
            {

                await Bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id, "Видалено");
                await Bot.DeleteMessageAsync(e.CallbackQuery.From.Id, e.CallbackQuery.Message.MessageId);
            }
            else if (e.CallbackQuery.Data.Contains("create_Photo_comment"))
            {
                try
                {
                    await Bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id, "Завантаження");
                    string key_task = e.CallbackQuery.Data.Replace("create_Photo_comment", "");
                    string text_comment = e.CallbackQuery.Message.Caption.Replace("Ваш коментар, відправити?\n", "");
                    text_comment=text_comment.Replace("Ваш коментар, відправити?", "");
                    bool res = db.Update_options_for_create_task(e.CallbackQuery.From.Id, "").Result;
                  
                    string text = $"{u.name} {u.surname} {u.email}: " + text_comment;

                  
                        var fileId = e.CallbackQuery.Message.Photo[e.CallbackQuery.Message.Photo.Count()-1].FileId;
                        var file = await Bot.GetFileAsync(fileId);
                    
                        using (MemoryStream imageStream = new MemoryStream())
                        {
                            await Bot.DownloadFileAsync(file.FilePath, imageStream);
                            byte[] imageBytes = imageStream.ToArray();

                            // Вызываем метод для добавления комментария в Jira с изображением
                            bool result_send_photo = Jira.AddPhotoCommentToIssue(key_task, imageBytes, file.FilePath).Result;
                            if (result_send_photo)
                            {
                                string photo_name = "";
                                int lastSlashIndex = file.FilePath.LastIndexOf('/');

                                if (lastSlashIndex != -1 && lastSlashIndex < file.FilePath.Length - 1)
                                {
                                    photo_name = file.FilePath.Substring(lastSlashIndex + 1);

                                }
                                Jira.AddCommentToIssue(key_task, text + " !" + photo_name + "|photo_name!");
                                await Bot.EditMessageCaptionAsync(e.CallbackQuery.From.Id, e.CallbackQuery.Message.MessageId, $"Ваш коментар: *{text_comment}* додано до заявки.", parseMode: ParseMode.Markdown);


                            }
                        }

                    

                  




                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else if(e.CallbackQuery.Data.Contains("detalno"))
            {  
                     await Bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id, "Завантаження...");

                  



                    JiraIssue jiraIssue = Jira.GetIssueByKey(e.CallbackQuery.Data.Replace("detalno","")).Result;

                



                string encodedDescription = HttpUtility.HtmlEncode(jiraIssue.fields.description);
                string text = "";

                text += $"<a href='https://sd.kernel.ua/plugins/servlet/theme/portal/2/{jiraIssue.key}'>{jiraIssue.key}</a>\n";
                text += $"<b>Статус:</b> {jiraIssue.fields.status.name}\n";
                if(jiraIssue.fields.assignee!=null)
                text += $"<b>Виконавець:</b> {jiraIssue.fields.assignee.displayName}\n";
                else text += $"<b>Виконавець:</b> Не призначено \n";
                text += $"<b>Тема:</b> {jiraIssue.fields.summary}\n";
                text += $"<b>Опис:</b> {encodedDescription}\n";

               
                    string text_comments = "";
                var inlinekeyboard = new InlineKeyboardMarkup(new[] {
                              new[]    {

                                                InlineKeyboardButton.WithCallbackData("✍️ Додати коментар","vidpovistu"+e.CallbackQuery.Data.Replace("detalno", "")),

                                         },
                             });
                if (jiraIssue.fields.comment.comments.Count == 0)
                {
                    await Bot.EditMessageTextAsync(e.CallbackQuery.From.Id, e.CallbackQuery.Message.MessageId, text+"\nКоментарів: немає", replyMarkup: inlinekeyboard, parseMode: ParseMode.Html);

                }
                else
                {

               
                    if (jiraIssue.fields.attachment==null || jiraIssue.fields.attachment.Count==0)
                {
                         if (jiraIssue.fields.comment.comments.Count != 0)
                    {
                 
                        text_comments = "📝:\n";
                        for (int i = 0; i < jiraIssue.fields.comment.comments.Count; i++)
                        {
                            text_comments += $"{jiraIssue.fields.comment.comments[i].author.displayName}: {jiraIssue.fields.comment.comments[i].body} ({ Convert.ToDateTime (jiraIssue.fields.comment.comments[i].created ). ToString("yyyy MM dd HH:mm:ss")})\n";
                                
                        }

                        if (text_comments.Length > 4090)
                        {
                            text_comments = text_comments.Substring(0, 1090) + "...";
                        }
                        text_comments = text+"\n" + text_comments;
                            text_comments = text_comments.Replace("t-bot_sd@kernel.ua:","");

                        try
                        {
                        if(e.CallbackQuery.Message.Type.ToString() == "Text")
                        {
                             await Bot.EditMessageTextAsync(e.CallbackQuery.From.Id, e.CallbackQuery.Message.MessageId, text_comments, replyMarkup: inlinekeyboard, parseMode: ParseMode.Html);
                      
                        }
                        else if (e.CallbackQuery.Message.Type.ToString() == "Photo")
                        {
                            await Bot.EditMessageCaptionAsync(e.CallbackQuery.From.Id, e.CallbackQuery.Message.MessageId, text_comments, replyMarkup: inlinekeyboard, parseMode: ParseMode.Html);

                        }


                    }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                      }
                    else
                    {
                        try
                        {
                            await Bot.EditMessageTextAsync(e.CallbackQuery.From.Id, e.CallbackQuery.Message.MessageId, text, replyMarkup: inlinekeyboard, parseMode: ParseMode.Html);

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                     
                    }
                }
                else
                {
                    if (jiraIssue.fields.comment.comments.Count != 0)
                    {
                       
                            if (e.CallbackQuery.Message.Type.ToString() == "Text")
                            {
                                await Bot.SendTextMessageAsync(e.CallbackQuery.From.Id,  text, replyMarkup: inlinekeyboard, parseMode: ParseMode.Html);

                            }
                            else if (e.CallbackQuery.Message.Type.ToString() == "Photo")
                            {
                                var fileId = e.CallbackQuery.Message.Photo[e.CallbackQuery.Message.Photo.Count() - 1].FileId;
                                var file = await Bot.GetFileAsync(fileId);

                                using (MemoryStream imageStream = new MemoryStream())
                                {
                                    await Bot.DownloadFileAsync(file.FilePath, imageStream);
                                    byte[] imageBytes = imageStream.ToArray();
                                 await Bot.SendPhotoAsync(e.CallbackQuery.From.Id, new InputOnlineFile(new MemoryStream(imageBytes)), text, replyMarkup: inlinekeyboard, parseMode: ParseMode.Html);

                                }

                                  
                            }
                        await Bot.SendTextMessageAsync(e.CallbackQuery.From.Id, "Коментарі заявки:");
                            for (int i = 0; i < jiraIssue.fields.comment.comments.Count; i++)
                        {
                            text_comments = "📝:\n";
                            text_comments += $"{jiraIssue.fields.comment.comments[i].author.displayName}: {jiraIssue.fields.comment.comments[i].body} ({Convert.ToDateTime(jiraIssue.fields.comment.comments[i].created).ToString("yyyy MM dd HH:mm:ss")})\n";
                            if (text_comments.Length > 4090)
                            {
                                text_comments = text_comments.Substring(0, 1090) + "...";

                            }
                                text_comments = text_comments.Replace("t-bot_sd@kernel.ua:", "");
                                byte[] bytesfile = null;
                            string filename = "";
                            foreach (Attachmentt item in jiraIssue.fields.attachment)
                            {
                                if (text_comments.Contains(item.filename))
                                {
                                    byte[] _bytesfile = Jira.GetFileInJiraComments(item.content).Result;
                                    filename = item.filename;
                                    bytesfile = _bytesfile;
                                    break;
                                }
                            }

                            if (filename != "")
                            {
                                InputOnlineFile inputFile;
                                using (MemoryStream ms = new MemoryStream(bytesfile))
                                {
                                    inputFile = new InputOnlineFile(ms, filename);
                                        string pattern = $@"!{Regex.Escape(filename)}\|[^!]+!";

                                        // Заменяем подстроку на пустую строку
                                         text_comments = Regex.Replace(text_comments, pattern, " ");
                                        text_comments = text_comments.Replace($"[^{filename}]", " ");
                                       
                                        await Bot.SendDocumentAsync(e.CallbackQuery.From.Id, inputFile, caption: text_comments);


                                }
                            }
                            else
                            {
                                await Bot.SendTextMessageAsync(e.CallbackQuery.From.Id, text: text_comments);

                            }


                        }




                }
                    else
                    {
                        try
                        {
                            await Bot.EditMessageTextAsync(e.CallbackQuery.From.Id, e.CallbackQuery.Message.MessageId, text, replyMarkup: inlinekeyboard, parseMode: ParseMode.Html);

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }






                }

                }


            }
            else if (e.CallbackQuery.Data.Contains("vidpovistu"))
                {
                    try
                    {
                        if (db.Update_options_for_create_task(e.CallbackQuery.From.Id, "new_comment"+e.CallbackQuery.Data.Replace("vidpovistu","")).Result == true)
                        {
                            await Bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id, "Напишіть коментар, я його відправлю");
                            await Bot.SendTextMessageAsync(e.CallbackQuery.From.Id, text: "Напишіть коментар, я його відправлю  (За необхідністю можете додати 1 фото)", replyToMessageId: e.CallbackQuery.Message.MessageId);

                        }
                    }
                    catch(Exception ex)
                    {

                    }

                 }

            

        }
         static Task Error(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
        {
            Console.WriteLine($"Block ERROR. Произошла ошибка в боте: {arg2.Message}");
            try
            {
                Bot.SendTextMessageAsync(494277044, arg2.Message);
            }
            catch
            {

            }
           
            return Task.CompletedTask;
        }
         static async void ForStart(Update e)
        {
            User u = db.getUserBytelegramId(e.Message.From.Id);


            if (u == null || u.active==false)
            {
               
                ReplyKeyboardMarkup replyKeyboard = new ReplyKeyboardMarkup((new[]
             {
                    new[]
                        {
                              KeyboardButton.WithRequestContact ("Поділитися номером телефону"),



                        }
                }));
                replyKeyboard.ResizeKeyboard = true;
                await Bot.SendTextMessageAsync(e.Message.From.Id, FirstTextMessage, replyMarkup: replyKeyboard);

            }
            else
            {
              
                SendStartKeyboard(u);

            }




        }
         static async void ForGetContact(Update e)
        {
            try
            {
                User u = new User();

                u.telegram_data.telegram_id = e.Message.From.Id;
                u.telegram_data.username = e.Message.From.Username;
                u.telegram_data.fisrtname = e.Message.From.FirstName;
                u.telegram_data.lastname = e.Message.From.LastName;
                u.telegram_data.last_message = e.Message.Text;

                string phone = e.Message.Contact.PhoneNumber;
                phone = phone.Replace("(", "");
                phone = phone.Replace(")", "");
                phone = phone.Replace(" ", "");
                switch (phone[0])
                {
                    case '+':
                        phone = phone.Substring(3);
                        break;
                    case '3': phone = phone.Substring(2); break;

                    case '8': phone = phone.Substring(1); break;

                }

                u.telegram_data.phone_number = phone;
                //  u = ActiveDirectory.UpdateUserByPhoneNumber(u);

                u = RequestTo1cApi.SearchUser(u).Result;
                if (u.name == null || u.name == "")
                {
                    /*NetPravNaBota(e)*/
                     u.active = false;
                   bool addNew_not_1C_user =db.AddOrUpdateUser(u);
                    if(addNew_not_1C_user)
                    {
                        await Bot.SendTextMessageAsync(e.Message.From.Id, "Наразі у вас немає прав на використання цього бота. Для роз'яснень зверніться до Вашого IT фахівця будь-яким іншим доступним способом. Але є можливість ідентифікації через вашу пошту. Будь ласка відправте мені ваш email.");

                    }
                    return; 
                }
                else
                {
                    u.active = true;
                    u.project = "ITSD";
                }
                bool addNewUser = db.AddOrUpdateUser(u);
                if (addNewUser)
                {
                    SendStartKeyboard(u);
                }
                else
                {
                    await Console.Out.WriteLineAsync();
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }
         static async void NetPravNaBota(Update e)
        {
            await Bot.SendTextMessageAsync(e.Message.From.Id, "Наразі у вас немає прав на використання цього бота."); ForStart(e);
        }     
         static async void CreateNewRequest(Update e)
        {
            string SendText = "Створення нового запиту '<b>"+e.Message.Text+ "</b>'.\n";
            switch(e.Message.Text)
            {
                case "🔥 У мене не працює": SendText += "Опишіть детально що саме не працює та при яких діях проявляється помилка"; break;
                case "💻 Хочу замовити обладнання": SendText += "Опишіть детально яке обладнання чи програмне забезпечення Вам потрібно";  break;
                case "❓ Хочу запитати": SendText += "Опишіть детально Ваше питання"; break;
            }
            SendText += " (За необхідністю можете додати 1 фото)";
            if (db.Update_options_for_create_task(e.Message.From.Id, e.Message.Text).Result==true)
            await Bot.SendTextMessageAsync(e.Message.From.Id, SendText,  parseMode: ParseMode.Html);
        }

         static async void Text_For_Create_New_Request(Update e)
        {
            string tema = db.Get_options_for_create_task(e.Message.From.Id).Result;
            if(e.Message.Text!=null)
            {
                string text = e.Message.Text;
                InlineKeyboardMarkup replyMarkup = new InlineKeyboardMarkup(new[]{

            new []
            {
                 InlineKeyboardButton.WithCallbackData("✅ Відправити","create_task"),
            },
             new []
            {
                 InlineKeyboardButton.WithCallbackData("❌ Скасувати","delete_task"),
             },
               }

                    );
                await Bot.SendTextMessageAsync(e.Message.From.Id, $"Перевірте та підтвердіть заявку\n{tema}\n{text}", replyMarkup: replyMarkup);

            }
           



            //      return;

        }
         static async void Text_For_Create_New_Comment(Update e, string key_task)
        {
           
            string text = e.Message.Text;
            InlineKeyboardMarkup replyMarkup = new InlineKeyboardMarkup(new[]{
                 new []
            {
                 InlineKeyboardButton.WithCallbackData("✅ Відправити","create_comment"+key_task),
            },
             new []
            {
                 InlineKeyboardButton.WithCallbackData("❌ Скасувати","delete_comment"),
             },
           
               }

                );
            await Bot.SendTextMessageAsync(e.Message.From.Id, $"Ваш коментар, відправити?\n{text}", replyMarkup: replyMarkup);


            //      return;

        }

         static async void SearchQRProblem(Update e)
        {
            if (e.Message.Text.Contains("/start QRProblemDevice_and_Programs_id_"))
            {
                QRCreateInlineButtonForDevice_and_ProgramsProblem(e);
            }
        }
         static async void QRCreateInlineButtonForDevice_and_ProgramsProblem(Update e)
        {
            int Device_and_ProgramsId = Convert.ToInt32(e.Message.Text.Replace("/start QRProblemDevice_and_Programs_id_", ""));

            Device_and_Programs this_Device_and_Programs = db.GetDevice_and_Programs(Device_and_ProgramsId);

            if (this_Device_and_Programs == null)
            {
                await Bot.SendTextMessageAsync(e.Message.From.Id, "Не знайдено інформації по QR коду.");
                Console.WriteLine(  "Не найдено информации по qr: "+ Device_and_ProgramsId+"\n Пользователь "+e.Message.From.Id+" отправил: "+e.Message.Text);
                return;
            }



            string msg = "Створення запиту.\n\n" + this_Device_and_Programs.type_Device_And_Programs.name+" "+ this_Device_and_Programs.name+ " \n" +this_Device_and_Programs.description+"\n" 
                + "\nОберіть підходящий варіант проблеми";
            InlineKeyboardButton[][] inlineKeyboardButtons;
          
                inlineKeyboardButtons = new InlineKeyboardButton[this_Device_and_Programs.list_problems.Count][];            
           


            for (int i = 0; i < this_Device_and_Programs.list_problems.Count; i++)
            {
               
                   inlineKeyboardButtons[i] = new InlineKeyboardButton[1];
               


                inlineKeyboardButtons[i][0] =  InlineKeyboardButton.WithCallbackData  (text: this_Device_and_Programs.list_problems[i].text_problem, callbackData: "Inl_kb_problemId:"+this_Device_and_Programs.list_problems[i].id+",organizationId:"+this_Device_and_Programs.assigned_organization.id);
            }


            InlineKeyboardMarkup inlineKeyboardMarkup = new InlineKeyboardMarkup(inlineKeyboardButtons);
           

           await Bot.SendTextMessageAsync(e.Message.From.Id, msg, replyMarkup: inlineKeyboardMarkup);




        }

         static async void MyNoResolvedRequest(Update e)
        {
            await Bot.SendTextMessageAsync(e.Message.From.Id, "Шукаю Ваші запити");
           
            JiraIssues jiraIssues = Jira.GetRequestOfuser(db.getUserBytelegramId(e.Message.From.Id)).Result;
            if(jiraIssues != null ) 
            {
                if(jiraIssues.issues.Count()!=0)
                {
                    for (int i = 0; i < jiraIssues.issues.Count(); i++)
                    {
                        string text = "";
                        //text += $"[{jiraIssues.issues[i].key}](https://sd.kernel.ua/browse/{jiraIssues.issues[i].key})\n";
                        text += $"[{jiraIssues.issues[i].key}](https://sd.kernel.ua/plugins/servlet/theme/portal/2/{jiraIssues.issues[i].key})\n";
                        text += $"Статус: *{jiraIssues.issues[i].fields.status.name}*\n";
                        if(jiraIssues.issues[i].fields.assignee!=null)
                        text += $"Виконавець: *{jiraIssues.issues[i].fields.assignee.displayName}*\n";
                        else text += $"Виконавець: *Не призначено*\n";
                        text += $"Тема: *{jiraIssues.issues[i].fields.summary}*\n";
                        text += $"Опис: *{jiraIssues.issues[i].fields.description}*\n";

                        //var inlinekeyboard = new InlineKeyboardMarkup(new[] {
                        //      new[]    {    
                        //      InlineKeyboardButton.WithWebApp("Переглянути",new WebAppInfo() {
                        //          Url="https://sd.kernel.ua/plugins/servlet/theme/portal/2/" + jiraIssues.issues[i].key
                        //                  })} });

                        var inlinekeyboard = new InlineKeyboardMarkup(new[] {
                              new[]    {

                                                InlineKeyboardButton.WithCallbackData("✍️ Додати коментар","vidpovistu" + jiraIssues.issues[i].key),

                                         },
                              new[]    {
                                InlineKeyboardButton.WithCallbackData("ℹ️ Переглянути детально","detalno"+jiraIssues.issues[i].key)
                                         } });



                        try
                        {
                            await Bot.SendTextMessageAsync(
                                e.Message.From.Id,
                                text,
                                replyMarkup: inlinekeyboard,
                                parseMode: ParseMode.Markdown
                            );
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("461str: " + ex.Message);
                        }
                    }
                }
                else
                {
                    await Bot.SendTextMessageAsync(e.Message.From.Id, "Відкритих запитів немає");
                }
            }
            else 
            {
                await Bot.SendTextMessageAsync(e.Message.From.Id, "Не вдалось знайти");
            }




        }

         static async void Dovidnuk(Update e)
        {

            List<Dovidnuk> dovidnuk_data = db.GetDovidnukData();
            if(dovidnuk_data!=null)
            {
                InlineKeyboardButton[][] buttons = new InlineKeyboardButton[dovidnuk_data.Count][];
                for (int i = 0; i < buttons.Length; i++)
                {
                    buttons[i] = new[] { InlineKeyboardButton.WithCallbackData(dovidnuk_data[i].name) };
                     
                }
                InlineKeyboardMarkup inlinekeyboard = new InlineKeyboardMarkup(buttons);
                await Bot.SendTextMessageAsync(e.Message.From.Id, "Оберіть, що саме вас цікавить?", replyMarkup: inlinekeyboard);
            }

         


           

        }
        static async void AdminChat(Update e)
        {
            try
            {
                switch (e.Message.Text)
                {
                    case "/start":
                        var replyKeyboard = new ReplyKeyboardMarkup(
                                           new[]
                                               {
                                              new []
                                           {
                                               new KeyboardButton ("Всі користувачі"),
                                            },
                                                new []
                                           {
                                               new KeyboardButton ("..."),
                                            },

                                               }
                                           );
                        replyKeyboard.ResizeKeyboard = true;
                       // await Bot.SendTextMessageAsync(id_admin_chat, "Що зробити?", replyMarkup: replyKeyboard);
                        break;
                    case "Всі користувачі":
                        List<User> users = db.GetAllUsers().Result;
                        string msg = "";
                        int index = 1;
                        foreach (User user in users) 
                        {
                            msg += $"{index}: {user.name} {user.surname} \n ";
                            index++;
                        }
                        msg += "Всього: " + index;
                      //  await Bot.SendTextMessageAsync(id_admin_chat, msg);
                        break;
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }
        public static async void NewStatus(JiraGetWebhookJson json)
        {
            if(json!=null )
            {
                if(json.fields!=null)
                {
                    if(json.fields.customfield_17000!=null && json.fields.customfield_17000!="")
                    {
                        try
                        {
                            string str = $"У Заявці [{json.key}](https://sd.kernel.ua/plugins/servlet/theme/portal/2/{json.key}) \n" +
                                $"{json.fields.summary} \n " +
                                 $"Опис: {json.fields.description} \n " +
                                $"Змінено статус задачі на  *{json.fields.status.description}*";

                            var inlinekeyboard = new InlineKeyboardMarkup(new[] {
                              new[]    {

                                                InlineKeyboardButton.WithCallbackData("✍️ Додати коментар","vidpovistu" + json.key),

                                         },
                             new[]    {

                              InlineKeyboardButton.WithCallbackData("ℹ️ Переглянути детально","detalno"+json.key)
                                         }});
                            bool res = db.Update_options_for_create_task(Convert.ToInt64 (json.fields.customfield_17000), "").Result;
                            await Bot.SendTextMessageAsync(json.fields.customfield_17000, str, replyMarkup: inlinekeyboard, parseMode: ParseMode.Markdown);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                        return;
                    }
                   
                    
                    
                }
            }
            Console.WriteLine($"Произошло событие NEW_STATUS, но telegram_id в заявке {json.key} не обнаружен");

            // text += $"[{jiraIssues.issues[i].key}](https://sd.kernel.ua/browse/{jiraIssues.issues[i].key})\n";
      
           
        }       
        public static async void NewComment(JiraGetWebhookJson json)
        {

            if (json != null)
            {
                if (json.fields != null)
                {
                    if (json.fields.customfield_17000 != null && json.fields.customfield_17000 != "")
                    {
                        JiraIssue issue = Jira.GetIssueByKey(json.key).Result;
                        string body_text = issue.fields.comment.comments[issue.fields.comment.comments.Count - 1].body;
                        byte[] bytesfile = null;
                        string filename = "";
                        foreach (Attachmentt item in issue.fields.attachment)
                        {
                            if (body_text.Contains(item.filename))
                            {
                                byte[] _bytesfile = Jira.GetFileInJiraComments(item.content).Result;
                                filename = item.filename;
                                bytesfile = _bytesfile;
                                break;
                            }
                        }
                       
                       
                        if (filename != "")
                        {
                            InputOnlineFile inputFile;
                            using (MemoryStream ms = new MemoryStream(bytesfile))
                               {
                                     inputFile = new InputOnlineFile(ms, filename);
                               
                           
                            bool res = db.Update_options_for_create_task(Convert.ToInt64(json.fields.customfield_17000), "").Result;
                            Comments comments = issue.fields.comment;
                            string str = $"У Заявці [{json.key}](https://sd.kernel.ua/plugins/servlet/theme/portal/2/{json.key}) \n{json.fields.summary} \n " +
                                       $"Опис: {json.fields.description}  " +
                                       $"Новий коментар від " +
                                       $"*{comments.comments[comments.comments.Count - 1].author.displayName}*:\n  {comments.comments[comments.comments.Count - 1].body}";
                                str = str.Replace("!"+ filename+ "|thumbnail!", "");
                                var inlinekeyboard = new InlineKeyboardMarkup(new[] {
                              new[]    {
                              InlineKeyboardButton.WithCallbackData("✍️ Відповісти","vidpovistu" + json.key)
                                         },
                                   new[]    {
                              InlineKeyboardButton.WithCallbackData("ℹ️ Переглянути детально","detalno"+json.key)
                                         },

                                });
                            if (str.Length > 4090)
                            {
                                str = str.Substring(0, 1090) + "...";
                            }
                           


                            await Bot.SendDocumentAsync(json.fields.customfield_17000, inputFile, caption: str, replyMarkup: inlinekeyboard, parseMode: ParseMode.Markdown);
                            }
                        }
                        else if (filename == "")

                        {
                            try
                            {
                                Comments comments = Jira.GetCommentsForIssue(json.key).Result;
                                if (comments.comments != null && comments.comments.Count > 0)
                                {
                                    string str = $"У Заявці [{json.key}](https://sd.kernel.ua/plugins/servlet/theme/portal/2/{json.key}) \n{json.fields.summary} \n " +
                                        $"Опис: {json.fields.description} \n " +
                                        $"Новий коментар від " +
                                        $"*{comments.comments[comments.comments.Count - 1].author.displayName}*:\n  {comments.comments[comments.comments.Count - 1].body}";
                                    var inlinekeyboard = new InlineKeyboardMarkup(new[] {
                              new[]    {
                              InlineKeyboardButton.WithCallbackData("✍️ Відповісти","vidpovistu" + json.key)
                                         },
                                   new[]    {
                              InlineKeyboardButton.WithCallbackData("ℹ️ Переглянути детально","detalno"+json.key)
                                         },

                                });


                                    if (str.Length > 4090)
                                    {
                                        str = str.Substring(0, 1090) + "...";
                                    }

                                    bool res = db.Update_options_for_create_task(Convert.ToInt64(json.fields.customfield_17000), "").Result;

                                    await Bot.SendTextMessageAsync(json.fields.customfield_17000, str, replyMarkup: inlinekeyboard, parseMode: ParseMode.Markdown);



                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                        }





                        //try
                        //{
                        //    JiraIssue issue = Jira.GetIssueByKey(json.key).Result;
                        //    string body_text = issue.fields.comment.comments[issue.fields.comment.comments.Count - 1].body;
                        //    byte[] bytesfile = null;
                        //    string filename = "";
                        //    foreach (Attachmentt item in issue.fields.attachment)
                        //    {
                        //        if(body_text.Contains(item.filename))
                        //        {
                        //            byte[] _bytesfile = Jira.GetFileInJiraComments(item.content).Result;
                        //            filename = item.filename;
                        //            bytesfile = _bytesfile;
                        //            break;
                        //        }
                        //    }
                        //    if (filename != "")
                        //    {
                        //        using (MemoryStream ms = new MemoryStream(bytesfile))
                        //        {
                        //            InputOnlineFile inputFile = new InputOnlineFile(ms, filename);
                        //            await Bot.SendDocumentAsync(json.fields.customfield_17000, inputFile);
                        //        }
                        //    }

                        //}
                        //catch (Exception ex)
                        //{
                        //    Console.WriteLine(ex);
                        //}

                        return;
                    }
                }
            }
            Console.WriteLine($"Произошло событие NEW_COMMENT, но telegram_id в заявке {json.key} не обнаружен");





      

        }
        public static async void CloseTask(JiraGetWebhookJson json)
        {

            if (json != null)
            {
                if (json.fields != null)
                {
                    if (json.fields.customfield_17000 != null && json.fields.customfield_17000 != "")
                    {

                        try
                        {
                            Comments comments = Jira.GetCommentsForIssue(json.key).Result;
                            if (comments.comments != null && comments.comments.Count > 0)
                            {
                                string str = $"Заявка [{json.key}](https://sd.kernel.ua/plugins/servlet/theme/portal/2/{json.key}) *виконана* \n{json.fields.summary} \n\n Коментар від " +
                                    $"*{comments.comments[comments.comments.Count - 1].author.displayName}*\n📝:\n  {comments.comments[comments.comments.Count - 1].body}";
                                var inlinekeyboard = new InlineKeyboardMarkup(new[] {
                              new[]    {
                              InlineKeyboardButton.WithCallbackData("✍️ Відповісти","vidpovistu" + json.key)
                                         },
                                   new[]    {
                              InlineKeyboardButton.WithCallbackData("ℹ️ Переглянути детально","detalno"+json.key)
                                         },

                                });


                                if (str.Length > 4090)
                                {
                                    str = str.Substring(0, 1090) + "...";
                                }

                                bool res = db.Update_options_for_create_task(Convert.ToInt64(json.fields.customfield_17000), "").Result;

                                await Bot.SendTextMessageAsync(json.fields.customfield_17000, str, replyMarkup: inlinekeyboard, parseMode: ParseMode.Markdown);

                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                        return;
                    }
                }
            }
            Console.WriteLine($"Произошло событие CloseTask, но telegram_id в заявке {json.key} не обнаружен");







        }
    }
}
