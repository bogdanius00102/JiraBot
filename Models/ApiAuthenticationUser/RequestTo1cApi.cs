using KernelHelpBot.Models.People_Information;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Web;

namespace KernelHelpBot.Models.ApiAuthenticationUser
{
    public class RequestTo1cApi
    {
       public  static async Task<User> SearchUser(User u)
        {

            string apiUrl = "https://am-api-gw.kernel.ua/zup/zup_data_tbot?phone=" + u.telegram_data.phone_number;
            string username = "zuptbot";
            string password = "tbot@2023!!TBOT";
            string server_id = "wso2ei-node2|ZVtae|ZVtXq";
           
            using (var httpClient = new HttpClient())
            {
                var byteArray = System.Text.Encoding.UTF8.GetBytes($"{username}:{password}");
                var base64Credentials = Convert.ToBase64String(byteArray);

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64Credentials);
                httpClient.DefaultRequestHeaders.Add("Cookie", "SERVERID=" + server_id);
               
                try
                {
                    HttpResponseMessage response = await httpClient.GetAsync(apiUrl);
                 //   Console.WriteLine(response.StatusCode);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        if(responseContent== "{\"entries\":null}")
                        {
                            u.active = false;//Зареган по почте
                            return u;
                        }
                        responseContent = HttpUtility.UrlDecode(responseContent);
                        UserInfo _u = JsonConvert.DeserializeObject<UserInfo>(responseContent);
                        if(_u!=null)if(_u.entries!=null)if(_u.entries.entry!=null)
                        if (_u.entries.entry is JObject)
                        {
                            // Один объект
                            Entry e = JsonConvert.DeserializeObject<Entry>(_u.entries.entry.ToString());
                            // Обработка одиночного объекта...
                            u.name = e.Imya;
                            u.surname = e.Familiya;
                            u.email = e.Pochta;
                            u.work_position = e.MestoRaboty;
                                        u.project = "ITSD";
                                        if (e.LoginAccountDisabled==0)
                                        {
                                            u.active = true;
                                        }
                                        else
                                        {
                                            u.active = false;
                                        }
                        }
                        else if (_u.entries.entry is JArray)
                        {
                            // Массив объектов
                            List<Entry> multipleEntries = JsonConvert.DeserializeObject<List<Entry>>(_u.entries.entry.ToString());
                            // Обработка массива объектов...
                            foreach (Entry e in multipleEntries)
                            {
                                if (e.VidZanyatosti == "Основне місце роботи")
                                {

                                    u.name = e.Imya;
                                    u.surname = e.Familiya;
                                    u.email = e.Pochta;
                                    u.work_position = e.MestoRaboty;
                                                u.project = "ITSD";
                                                if (e.LoginAccountDisabled == 0)
                                                {
                                                    u.active = true;
                                                }
                                                else
                                                {
                                                    u.active = false;
                                                }
                                                return u;
                                }
                            }
                        }


                        

                        return u;
                    }
                    else
                    {
                        Console.WriteLine("Ошибка: " + response.StatusCode);
                        return u;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка: " + ex.Message);
                    return u;
                }
            }
        }
    }
}
