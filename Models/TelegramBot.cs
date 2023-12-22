using KernelHelpBot.Models.AD;
using KernelHelpBot.Models.ApiAuthenticationUser;
using KernelHelpBot.Models.Databases;
using KernelHelpBot.Models.JiraRequest;
using KernelHelpBot.Models.People_Information;
using KernelHelpBot.Models.TechniksInformation;
using Microsoft.VisualBasic;
using MySqlX.XDevAPI.Common;
using System.Runtime.Intrinsics.X86;
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
    public class TelegramBot
    {

        static TelegramBotClient Bot;
      //  static long id_admin_chat = -1002006933069;
        static string FirstTextMessage = "Раді Вас бачити. Натисніть \"Поділитися номером телефону\", щоб я побачив хто Ви.";
        //static Database db = new Database("server=localhost;user=root;database=kernelhelpbot;password=toor;charset=utf8mb4;");

          static Database db = new Database("server=localhost;user=root;database=kernelhelpbot;password=P@ssw0rd$D;charset=utf8mb4;");
        public TelegramBot()
        {
            //kernelhelp
            //itsd

            //Bot = new TelegramBotClient("6382587286:AAGwGAaNmKMy-oD-wzqtihpFe_3oI2TZlf0");
            Bot = new TelegramBotClient("6939260864:AAH-IALzUbpfoAdQQwxPFVQpmyZWCF2s6Wk");
            Bot.StartReceiving(Update, Error);
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
                    if(user.actice==true)
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
                if (e.Message.Text != null)
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
                       
                        if (u.actice==true)
                        ForMessageText(e);
                        else
                        {
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

            }
            else if (e.CallbackQuery != null )
            {
                User u = db.getUserBytelegramId(e.CallbackQuery.From.Id);
                if (u!=null)
                    if(u.actice==true)
                ForCallbackQuery(e,u);
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
                    await Bot.SendTextMessageAsync(e.Message.From.Id, "Даний розділ знаходиться в розробці");
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
                
               
                
                Problem_for_type_device_and_programs problem = db.GetProblems_device_and_programsByProblemId(inlKbProblemId);
                if (problem == null) return;
              
                await Bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id, "Створюємо запит!");
               
                //return;
              //  12345
                ResponseOnCreateJiraTask result;
               if (u.email!=null && u.email!="")
                    result = Jira.CreateNewTask(e.CallbackQuery.From.Id,problem.type_Device_And_Programs.name + " " + problem.text_problem, text_message, u.email).Result;
                else
                {
                    string text = text_message+"\nКористувач: " + u.name + " " + u.surname + " " + u.telegram_data.phone_number + " WorkPosition: " + u.work_position + "\nTelegramId: " + u.telegram_data.telegram_id;

                    result = Jira.CreateNewTask(e.CallbackQuery.From.Id,problem.type_Device_And_Programs.name + " " + problem.text_problem, text, "t-bot_sd@kernel.ua").Result;

                
                }
                if (result!=null)
                {
                  
                    string url_create_task = "https://sd.kernel.ua/plugins/servlet/theme/portal/2/" + result.key;

                    await Bot.EditMessageTextAsync(e.CallbackQuery.From.Id, e.CallbackQuery.Message.MessageId, "ЗАПИТ СТВОРЕН: <b>" + result.key + "</b>\n" + problem.type_Device_And_Programs.name + " " + problem.text_problem +"\n"+ text_message, parseMode: ParseMode.Html,replyMarkup:new InlineKeyboardMarkup(new InlineKeyboardButton[] {InlineKeyboardButton.WithWebApp(""+result.key,new WebAppInfo() { Url=url_create_task}) }));

                   // await Bot.SendTextMessageAsync(id_admin_chat,$"Пользователь {u.name} {u.surname} {u.email} создал заявку: {problem.text_problem} \n{result.key}");
                   
                    DateTime currentTime = DateTime.Now;
                    bool isWeekday = currentTime.DayOfWeek >= DayOfWeek.Monday && currentTime.DayOfWeek <= DayOfWeek.Friday;
                    bool isWorkingHours = currentTime.TimeOfDay >= new TimeSpan(8, 0, 0) && currentTime.TimeOfDay < new TimeSpan(18, 0, 0);
                    if(isWeekday==false || isWorkingHours==false || currentTime.Day==25 || currentTime.Day == 1)
                    {                       
                        List<Organization> organizations = db.GetListOrganization();
                        int id = 0;
                        id = (from t in organizations where t.name == u.work_position select t.id).FirstOrDefault();
                        IT_HUB hub = db.Get_IT_HUB_BY_ORGANIZATION_ID(id).Result;
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
                text = text.Replace("Перевірте та підтвердіть заявку", "");
                
                bool res = db.Update_options_for_create_task(e.CallbackQuery.From.Id, "").Result;
                {
                        ResponseOnCreateJiraTask result;
                       if (u.email != "" && u.email != null)
                          result = Jira.CreateNewTask(e.CallbackQuery.From.Id, tema, text, u.email).Result;
                      else
                       {

                          text += "\nКористувач: " + u.name + " " + u.surname + " " + u.telegram_data.phone_number + " WorkPosition: "+u.work_position+"\nTelegramId: " + u.telegram_data.telegram_id;
                           result = Jira.CreateNewTask(e.CallbackQuery.From.Id, tema, text, "t-bot_sd@kernel.ua").Result;
                       }
                        if (result != null)
                      {

                            string url_create_task = "https://sd.kernel.ua/plugins/servlet/theme/portal/2/" + result.key;
                           Console.WriteLine("Створений  Запит\n" + tema + " : " + text+"\n"+url_create_task);
                          try
                           {
                                await Bot.SendTextMessageAsync(
                                    chatId: e.CallbackQuery.From.Id, 
                                   text: "Запит створен: <b>" + result.key + "</b>\n"
                                + tema + "\n" + text, 
                                    parseMode: ParseMode.Html,
                                    replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton[] { InlineKeyboardButton.WithWebApp("" + result.key, new WebAppInfo() { Url = url_create_task }) }),
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
                    

                        try
                        {
                            await Bot.EditMessageTextAsync(e.CallbackQuery.From.Id, e.CallbackQuery.Message.MessageId, text_comments, replyMarkup: inlinekeyboard, parseMode: ParseMode.Html);

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
            else if (e.CallbackQuery.Data.Contains("vidpovistu"))
                {
                    try
                    {
                        if (db.Update_options_for_create_task(e.CallbackQuery.From.Id, "new_comment"+e.CallbackQuery.Data.Replace("vidpovistu","")).Result == true)
                        {
                            await Bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id, "Напишіть коментар, я його відправлю");
                            await Bot.SendTextMessageAsync(e.CallbackQuery.From.Id, text: "Напишіть коментар, я його відправлю", replyToMessageId: e.CallbackQuery.Message.MessageId);

                        }
                    }
                    catch(Exception ex)
                    {

                    }

                 }

            

        }
         static Task Error(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
        {
            throw new NotImplementedException();
        }
         static async void ForStart(Update e)
        {
            User u = db.getUserBytelegramId(e.Message.From.Id);


            if (u == null || u.actice==false)
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
                if (u.name == null || u.name == "") { NetPravNaBota(e);return; }
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
            if (db.Update_options_for_create_task(e.Message.From.Id, e.Message.Text).Result==true)
            await Bot.SendTextMessageAsync(e.Message.From.Id, SendText,  parseMode: ParseMode.Html);
        }


         static async void Text_For_Create_New_Request(Update e)
        {
            string tema = db.Get_options_for_create_task(e.Message.From.Id).Result;
            string text = e.Message.Text;
            InlineKeyboardMarkup replyMarkup = new InlineKeyboardMarkup(new []{
           
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
               
              
            //      return;
          
        }
        static async void Text_For_Create_New_Comment(Update e, string key_task)
        {
           
            string text = e.Message.Text;
            InlineKeyboardMarkup replyMarkup = new InlineKeyboardMarkup(new[]{
                 new []
            {
                 InlineKeyboardButton.WithCallbackData("✅ Відправити","create_comment"),
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
