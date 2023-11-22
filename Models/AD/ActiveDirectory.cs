using KernelHelpBot.Models.People_Information;
using System.DirectoryServices;
using System.Reflection.PortableExecutable;
using DirectoryEntry = System.DirectoryServices.DirectoryEntry;
namespace KernelHelpBot.Models.AD
{
    public static class ActiveDirectory
    {
       
        public static User UpdateUserByPhoneNumber(User user)
        {
            string domainController = "LDAP://kernel.local"; // Замените на адрес своего домен-контроллера
            string username = "b.doroshkov"; // Замените на свое имя пользователя
            string password = "Bogdan02121998";
            try
            {
                using (DirectoryEntry entry = new DirectoryEntry(domainController, username, password))
                {
                    using (DirectorySearcher searcher = new DirectorySearcher(entry))
                    {

                        // searcher.Filter = $"(&(objectClass=user)(mobileForFind={user.telegram_data.phone_number}))";
                        searcher.Filter = $"(&(objectClass=user)(|(mobileForFind={user.telegram_data.phone_number})(mobile={user.telegram_data.phone_number})(personalMobilePhone={user.telegram_data.phone_number})))";


                        searcher.PropertiesToLoad.Add("givenName"); // Имя пользователя
                        searcher.PropertiesToLoad.Add("sn"); // Фамилия пользователя
                        searcher.PropertiesToLoad.Add("mail"); // Email пользователя
                        searcher.PropertiesToLoad.Add("extensionAttribute5"); // Должность пользователя
                        searcher.PropertiesToLoad.Add("department"); // department пользователя



                        SearchResult result = searcher.FindOne();

                        if (result != null)
                        {
                            user.name = result.Properties["givenName"][0].ToString();
                            user.surname = result.Properties["sn"][0].ToString();
                            user.email = result.Properties["mail"][0].ToString();
                            user.department = result.Properties["department"][0].ToString();
                            user.work_position = result.Properties["extensionAttribute5"][0].ToString();
                            Console.WriteLine($"Данные пользователя с номером телефона {user.telegram_data.phone_number} в АД найдены.");
                        }
                        else
                        {
                            Console.WriteLine($"Пользователь с номером телефона {user.telegram_data.phone_number} в АД не найден.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }


            return user;
        }
    }
}

