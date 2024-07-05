using KernelHelpBot.Models.People_Information;
using KernelHelpBot.Models.TechniksInformation;

using MySql.Data.MySqlClient;
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using Telegram.Bot.Types;

using static System.Net.Mime.MediaTypeNames;
using User = KernelHelpBot.Models.People_Information.User;

namespace KernelHelpBot.Models.Databases
{
    public class Database
    {
        string path;
        public Database(string path)
        {
            this.path = path;

        }
        public bool AddOrUpdateUser(User user)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(path))
                {
                    connection.Open();

                    // Проверка существования пользователя
                    string checkUserQuery = "SELECT COUNT(*) FROM kernelhelpbot.users WHERE telegram_id = @telegram_id";
                    using (MySqlCommand checkUserCommand = new MySqlCommand(checkUserQuery, connection))
                    {
                        checkUserCommand.Parameters.AddWithValue("@telegram_id", user.telegram_data.telegram_id);
                        int resultRequestCheckUser = Convert.ToInt32(checkUserCommand.ExecuteScalar());

                        if (resultRequestCheckUser == 0)
                        {
                            // Вставка нового пользователя
                            string insertUserQuery = "INSERT INTO `kernelhelpbot`.`users` " +
                                                     "(`telegram_id`, `name`, `surname`, `email`, `phone_number`, " +
                                                     "`work_position`, `username`, `fisrtname`, `lastname`, " +
                                                     "`last_message`, `description`, `departament`, `active`, `project`) " +
                                                     "VALUES " +
                                                     "(@telegram_id, @name, @surname, @email, @phone_number, " +
                                                     "@work_position, @username, @firstname, @lastname, " +
                                                     "@last_message, @description, @department, @active, @project)";

                            using (MySqlCommand insertUserCommand = new MySqlCommand(insertUserQuery, connection))
                            {
                                SetUserParameters(insertUserCommand, user);
                                insertUserCommand.ExecuteNonQuery();
                            }

                            return true;
                        }
                        else
                        {
                            // Обновление существующего пользователя
                            string updateUserQuery = "UPDATE `kernelhelpbot`.`users` SET " +
                                                    "`name` = @name, " +
                                                    "`surname` = @surname, " +
                                                    "`email` = @email, " +
                                                    "`phone_number` = @phone_number, " +
                                                    "`work_position` = @work_position, " +
                                                    "`username` = @username, " +
                                                    "`last_message` = @last_message, " +
                                                    "`description` = @description, " +
                                                    "`departament` = @department, " +
                                                    "`active` = @active, " +
                                                    "`project` = @project " +
                                                    "WHERE (`telegram_id` = @telegram_id);";

                            using (MySqlCommand updateUserCommand = new MySqlCommand(updateUserQuery, connection))
                            {
                                SetUserParameters(updateUserCommand, user);
                                updateUserCommand.ExecuteNonQuery();
                            }

                            Console.WriteLine("Користувач вже є у базі даних");
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Виникла помилка. Помилка у DatabaseUserInfo, у методі AddOrUpdateUser. Помилка:\n" + ex.Message);
                return false;
            }
        }

        private void SetUserParameters(MySqlCommand command, User user)
        {
            command.Parameters.AddWithValue("@telegram_id", user.telegram_data.telegram_id);
            command.Parameters.AddWithValue("@name", user.name);
            command.Parameters.AddWithValue("@surname", user.surname);
            command.Parameters.AddWithValue("@email", user.email);
            command.Parameters.AddWithValue("@phone_number", user.telegram_data.phone_number);
            command.Parameters.AddWithValue("@work_position", user.work_position);
            command.Parameters.AddWithValue("@username", user.telegram_data.username);
            command.Parameters.AddWithValue("@firstname", string.Empty);
            command.Parameters.AddWithValue("@lastname", string.Empty);
            command.Parameters.AddWithValue("@last_message", user.telegram_data.last_message);
            command.Parameters.AddWithValue("@description", user.description);
            command.Parameters.AddWithValue("@department", user.department);
            command.Parameters.AddWithValue("@active", user.active);
            command.Parameters.AddWithValue("@project", user.project);
        }

        public bool Delete_dev_and_programs(int id)
        {
            try
            {
                MySqlConnection connection = new MySqlConnection(path);
                connection.Open();
                string request = $"DELETE FROM `kernelhelpbot`.`list_device_and_programs` WHERE (`id` = '{id}');";
                MySqlCommand command = new MySqlCommand(request, connection);
                int rowsAffected = command.ExecuteNonQuery();

                connection.Close();

                return rowsAffected > 0;

                
            }
            catch (Exception ex)
            {
                Console.WriteLine("" + ex.Message);
                return false;
            }
        }
        public User getUserBytelegramId(long telegramId)
        {
            try
            {
                User u = null;
                MySqlConnection connection = new MySqlConnection(path);
                connection.Open();
                string sql_zapros1 = $"SELECT id, telegram_id, name, surname, email, phone_number, work_position, username, fisrtname, " +
                $"lastname, last_message, access, description, departament,active,project,RandomCode,TimeHealthCode,temporary_mail FROM users WHERE telegram_id='{telegramId}'";

                MySqlCommand command = new MySqlCommand(sql_zapros1, connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    u = new User();
                    u.id = Convert.ToInt32(reader[0]);
                    u.telegram_data.telegram_id = Convert.ToInt64(reader[1]);
                    u.name = reader[2].ToString();
                    u.surname = reader[3].ToString();
                    u.email = reader[4].ToString();
                    u.telegram_data.phone_number = reader[5].ToString();
                    u.work_position = reader[6].ToString();
                    u.telegram_data.username = reader[7].ToString();
                    u.telegram_data.fisrtname = reader[8].ToString();
                    u.telegram_data.lastname = reader[9].ToString();
                    u.telegram_data.last_message = reader[10].ToString();
                    u.description = reader[12].ToString();
                    u.department = reader[13].ToString();
                    u.active = Convert.ToBoolean(reader[14]);
                    u.project = reader[15].ToString();
                    u.RandomCode = reader[16].ToString();
                    u.TimeHealthCode = reader[17].ToString();
                    u.temporary_mail = reader[18].ToString();
                }
                connection.Close();
                return u;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Помилка в отриманні даних користувача з Бази даних. \nПомилка: " + ex.Message);

                return null;
            }
        }
        public User getUserBytelegramId(string email)
        {
            try
            {
                User u = null;
                MySqlConnection connection = new MySqlConnection(path);
                connection.Open();
                string sql_zapros1 = $"SELECT id, telegram_id, name, surname, email, phone_number, work_position, username, fisrtname, " +
                $"lastname, last_message, access, description, departament,active,project,RandomCode,TimeHealthCode,temporary_mail FROM users WHERE (id>0 AND email='{email}')";

                MySqlCommand command = new MySqlCommand(sql_zapros1, connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    u = new User();
                    u.id = Convert.ToInt32(reader[0]);
                   // u.telegram_data.telegram_id = Convert.ToInt64(reader[1]);
                    u.name = reader[2].ToString();
                    u.surname = reader[3].ToString();
                    u.email = reader[4].ToString();
                    u.telegram_data.phone_number = reader[5].ToString();
                    u.work_position = reader[6].ToString();
                    u.telegram_data.username = reader[7].ToString();
                    u.telegram_data.fisrtname = reader[8].ToString();
                    u.telegram_data.lastname = reader[9].ToString();
                    u.telegram_data.last_message = reader[10].ToString();
                    u.description = reader[12].ToString();
                    u.department = reader[13].ToString();
                    u.active = Convert.ToBoolean(reader[14]);
                    u.project = reader[15].ToString();
                    u.RandomCode = reader[16].ToString();
                    u.TimeHealthCode = reader[17].ToString();
                    u.temporary_mail = reader[18].ToString();
                }
                connection.Close();
                return u;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Помилка в отриманні даних користувача з Бази даних. \nПомилка: " + ex.Message);

                return null;
            }
        }
        public async Task<bool> UpdateUserInfo(long telegram_id, string column, string value)
        {

            try
            {
                using (MySqlConnection connection = new MySqlConnection(path))
                {
                    await connection.OpenAsync();
                    string request = $" UPDATE `kernelhelpbot`.`users` SET `"+ column + "` = '" + value + "' WHERE(`telegram_id` = '" + telegram_id + "');";
                    MySqlCommand command = new MySqlCommand(request, connection);
                    await command.ExecuteScalarAsync();

                    return true;
                }

            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            return false;
        }
        public async Task<bool> DeleteUser(long telegram_id)
        {

            try
            {
                using (MySqlConnection connection = new MySqlConnection(path))
                {
                    await connection.OpenAsync();
                    string request = $"DELETE FROM `kernelhelpbot`.`users` WHERE (`id` = '{telegram_id}')";
                    MySqlCommand command = new MySqlCommand(request, connection);
                    await command.ExecuteScalarAsync();

                    return true;
                }

            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            return false;
        }

        public async Task<bool> UpdateRandomCodeAndTime(long tel_id, string code, string time)
        {
          
            
            try
            {
                using (MySqlConnection connection = new MySqlConnection(path))
                {
                    await connection.OpenAsync();
                    string request = $" UPDATE `kernelhelpbot`.`users` SET `RandomCode` = '"+ code + "', `TimeHealthCode` = '"+time+ "' WHERE(`telegram_id` = '" + tel_id+"');";
                    MySqlCommand command = new MySqlCommand(request, connection);
                   await  command.ExecuteScalarAsync();

                    return true;
                }

            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            return false;
        }
        public async Task<bool> CheckActionCode(long tel_id, string code)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(path))
                {
                    await connection.OpenAsync();
                    string request = $"SELECT RandomCode, TimeHealthCode FROM kernelhelpbot.users WHERE telegram_id='{tel_id}';";
                    MySqlCommand command = new MySqlCommand(request, connection);

                    MySqlDataReader reader =  command.ExecuteReader();

                    while ( reader.Read())
                    {
                        string codeFromDB = reader.GetString(0);
                        DateTime timeHealthCodeFromDB = reader.GetDateTime(1);

                        // Проверяем, совпадает ли код и время
                        if (codeFromDB == code && timeHealthCodeFromDB > DateTime.Now)
                        {
                            return true;
                        }
                        else return false;
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }



            return false;
        }
        public async Task<bool> UpdateAction(long tel_id, bool active)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(path))
                {
                    await connection.OpenAsync();
                    string request = $"UPDATE `kernelhelpbot`.`users` SET `active` = '{Convert.ToInt32(active)}' WHERE (`telegram_id` = '{tel_id}');";
                    MySqlCommand command = new MySqlCommand(request, connection);

                   await command.ExecuteNonQueryAsync();

                    return true;
                }

            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            return false;
        }
        public async Task<bool> UpdateTemporary_mail(long tel_id, string temporary_mail)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(path))
                {
                    await connection.OpenAsync();
                    string request = $"UPDATE `kernelhelpbot`.`users` SET `temporary_mail` = '{temporary_mail}' WHERE (`telegram_id` = '{tel_id}');";
                    MySqlCommand command = new MySqlCommand(request, connection);

                    await command.ExecuteNonQueryAsync();

                    return true;
                }

            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            return false;
        }
        public async Task<List<User>> GetAllUsers()
        {
            List<User> users = new List<User>();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(path))
                {
                    await connection.OpenAsync();
                    string request = $"SELECT id, telegram_id, name, surname, email, phone_number, work_position, username, fisrtname, lastname, last_message, access, description, departament, active ,project FROM users";
                    MySqlCommand command = new MySqlCommand(request, connection);
                 
                    MySqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        User u = new User();
                        u.id = Convert.ToInt32(reader[0]);
                        if (reader[1].ToString == null || reader[1].ToString() == "")
                            continue;
                        u.telegram_data.telegram_id = Convert.ToInt64(reader[1]);
                        u.name = reader[2].ToString();
                        u.surname = reader[3].ToString();
                        u.email = reader[4].ToString();
                        u.telegram_data.phone_number = reader[5].ToString();
                        u.work_position = reader[6].ToString();
                        u.telegram_data.username = reader[7].ToString();
                        u.telegram_data.fisrtname = reader[8].ToString();
                        u.telegram_data.lastname = reader[9].ToString();
                        u.telegram_data.last_message = reader[10].ToString();
                        u.description = reader[12].ToString();
                        u.department = reader[13].ToString();
                        u.active = Convert.ToBoolean(reader[14]);
                        u.project = reader[15].ToString();
                        if (u.project != "ITSD")
                            continue;
                        users.Add(u);
                    }
                    connection.Close();
                    return users;
                }

            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            return null;

        }
        public async Task<bool> Update_options_for_create_task(long telegramId, string text)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(path))
                {
                    await connection.OpenAsync();
                    string request = $"UPDATE `kernelhelpbot`.`users` SET `options_for_create_task` = '{text}' WHERE (`telegram_id` = '{telegramId}');";
                    MySqlCommand command = new MySqlCommand(request, connection);

                    int rowsAffected = await command.ExecuteNonQueryAsync();

                    return rowsAffected > 0;
                }

            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            return false;
        }
        public async Task<string> Get_options_for_create_task(long telegramId)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(path))
                {
                    await connection.OpenAsync();
                    string request = $"SELECT options_for_create_task FROM kernelhelpbot.users WHERE telegram_id='{telegramId}';";
                    MySqlCommand command = new MySqlCommand(request, connection);

                    object resultObject = await command.ExecuteScalarAsync();

                    if (resultObject != null)
                    {
                        string result = resultObject.ToString();
                        return result;
                    }
                    else return "";
                }

            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            return "";
        }
        public Device_and_Programs GetDevice_and_Programs(int id)
        {

            try
            {
                Device_and_Programs device_And_Programs=null;
                MySqlConnection connection = new MySqlConnection(path);
                connection.Open();
                string sql_zapros1 = $"SELECT id, id_type_device_and_programs, name, description,id_organization FROM list_device_and_programs WHERE id='{id}'";

                MySqlCommand command = new MySqlCommand(sql_zapros1, connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    device_And_Programs = new Device_and_Programs();
                    device_And_Programs.id = Convert.ToInt32(reader[0]);
                    device_And_Programs.type_Device_And_Programs = GetType_Device_And_Programs(Convert.ToInt32(reader[1]));
                    device_And_Programs.name = Convert.ToString(reader[2]);
                    device_And_Programs.description = Convert.ToString(reader[3]);
                    device_And_Programs.list_problems = GetListWithProblems_device_and_programs(device_And_Programs.type_Device_And_Programs.id);
                    device_And_Programs.assigned_organization = GetOrganization(Convert.ToInt32(reader[4]));
                }
                connection.Close();
                return device_And_Programs;
            }
            catch (Exception ex) { Console.WriteLine(   ex.Message); }


            return null;
        }
        public List<Device_and_Programs> GetListDevice_and_ProgramsByOrganization(int id_organization)
        {

            try
            {
                List<Device_and_Programs> list_device_And_Programs = new List<Device_and_Programs>();
                MySqlConnection connection = new MySqlConnection(path);
                Organization o = GetOrganization(id_organization);
                connection.Open();
                string sql_zapros1 = $"SELECT id, id_type_device_and_programs, name, description,id_organization FROM list_device_and_programs WHERE id_organization='{id_organization}'";

                MySqlCommand command = new MySqlCommand(sql_zapros1, connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {

                    Device_and_Programs device_And_Programs = new Device_and_Programs();
                    device_And_Programs.id = Convert.ToInt32(reader[0]);
                    device_And_Programs.type_Device_And_Programs = GetType_Device_And_Programs(Convert.ToInt32(reader[1]));
                    device_And_Programs.name = Convert.ToString(reader[2]);
                    device_And_Programs.description = Convert.ToString(reader[3]);
                    device_And_Programs.list_problems = GetListWithProblems_device_and_programs(device_And_Programs.type_Device_And_Programs.id);
                    device_And_Programs.assigned_organization =o;
                    list_device_And_Programs.Add(device_And_Programs);
                }
                connection.Close();
                return list_device_And_Programs;
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }


            return null;
        }
        public List<Device_and_Programs> GetListDevice_and_ProgramsByOrganization_and_Type_dev(int id_organization, int type)
        {

            try
            {
                List<Device_and_Programs> list_device_And_Programs = new List<Device_and_Programs>();
                MySqlConnection connection = new MySqlConnection(path);
                Organization o = GetOrganization(id_organization);
                connection.Open();
                string sql_zapros1 = $"SELECT id, id_type_device_and_programs, name, description,id_organization FROM list_device_and_programs WHERE id_organization='{id_organization}' AND id_type_device_and_programs = '"+type+"'";

                MySqlCommand command = new MySqlCommand(sql_zapros1, connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {

                    Device_and_Programs device_And_Programs = new Device_and_Programs();
                    device_And_Programs.id = Convert.ToInt32(reader[0]);
                    device_And_Programs.type_Device_And_Programs = GetType_Device_And_Programs(Convert.ToInt32(reader[1]));
                    device_And_Programs.name = Convert.ToString(reader[2]);
                    device_And_Programs.description = Convert.ToString(reader[3]);
                    device_And_Programs.list_problems = GetListWithProblems_device_and_programs(device_And_Programs.type_Device_And_Programs.id);
                    device_And_Programs.assigned_organization = o;
                    list_device_And_Programs.Add(device_And_Programs);
                }
                connection.Close();
                return list_device_And_Programs;
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }


            return null;
        }
        public List<Info_for_type_device_and_programs> GetList_info_for_type_device_and_programs(int id_type_dev)
        {
            try
            {
                List<Info_for_type_device_and_programs> list_info = new List<Info_for_type_device_and_programs>();
                MySqlConnection connection = new MySqlConnection(path);
               
                connection.Open();
                string sql_zapros1 = $"SELECT id, id_type_device_and_programs, part_description FROM info_for_type_device_and_programs WHERE id_type_device_and_programs ='"+id_type_dev+"';";

                MySqlCommand command = new MySqlCommand(sql_zapros1, connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {

                    Info_for_type_device_and_programs info = new Info_for_type_device_and_programs();
                    info.id = Convert.ToInt32(reader[0]);
                    info.name = reader[2].ToString();
                    list_info.Add(info);
                }
                connection.Close();
                return list_info;
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }


            return null;
        }
        public int AddNewDev_and_programs(int id_type_device_and_programs,string name,string description,int id_organization )
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(path))
                {
                    connection.Open();

                    string query = "INSERT INTO list_device_and_programs (id_type_device_and_programs, name, description, id_organization) " +
                                   "VALUES (@id_type_device_and_programs, @name, @description, @id_organization);" +
                                   "SELECT LAST_INSERT_ID();"; // Получаем ID последней вставленной записи

                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@id_type_device_and_programs", id_type_device_and_programs);
                    command.Parameters.AddWithValue("@name", name);
                    command.Parameters.AddWithValue("@description", description);
                    command.Parameters.AddWithValue("@id_organization", id_organization);

                    // Выполняем запрос и получаем ID последней вставленной записи
                    int insertedId = Convert.ToInt32(command.ExecuteScalar());
                    return insertedId;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при выполнении запроса: " + ex.Message);
                return -1;
            }
        }
        public List<Problem_for_type_device_and_programs> GetListWithProblems_device_and_programs(int idTypeOfDeviceAndPrograms)
        {

            try
            {
                Type_device_and_programs type_Device_And_Programs = GetType_Device_And_Programs(idTypeOfDeviceAndPrograms);
               List<Problem_for_type_device_and_programs> list_problem_for_type_device_and_programs = new List<Problem_for_type_device_and_programs>();
                MySqlConnection connection = new MySqlConnection(path);
                connection.Open();
                string sql_zapros1 = $"SELECT id, text_problem FROM propblem_for_type_device_and_programs WHERE id_type_technik='{idTypeOfDeviceAndPrograms}'";

                MySqlCommand command = new MySqlCommand(sql_zapros1, connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Problem_for_type_device_and_programs p = new Problem_for_type_device_and_programs();
                    p.id = Convert.ToInt32(reader[0]);
                    p.text_problem = reader[1].ToString();
                    p.type_Device_And_Programs = type_Device_And_Programs;
                    list_problem_for_type_device_and_programs.Add(p);
                }
                connection.Close();
                return list_problem_for_type_device_and_programs;
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }


            return null;
        }
        public Problem_for_type_device_and_programs GetProblems_device_and_programsByProblemId(int problemId)
        {

            try
            {

                Problem_for_type_device_and_programs p=null;
                MySqlConnection connection = new MySqlConnection(path);
                connection.Open();
                string sql_zapros1 = $"SELECT id,id_type_technik, text_problem FROM propblem_for_type_device_and_programs WHERE id='{problemId}'";

                MySqlCommand command = new MySqlCommand(sql_zapros1, connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    p= new Problem_for_type_device_and_programs();
                    p.id = Convert.ToInt32(reader[0]);
                    p.type_Device_And_Programs = GetType_Device_And_Programs(Convert.ToInt32(reader[1]));
                    p.text_problem = reader[2].ToString();
                   
                }
                connection.Close();
                return p;
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }


            return null;
        }
        public List<Type_device_and_programs> Get_ALL_Type_Device_And_Programs()
        {
            try
            {
                List<Type_device_and_programs> list = new List<Type_device_and_programs>();
                MySqlConnection connection = new MySqlConnection(path);
                connection.Open();
                string sql_zapros1 = $"SELECT id, name FROM type_device_and_programs;";

                MySqlCommand command = new MySqlCommand(sql_zapros1, connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Type_device_and_programs type_Device_And_Programs = new Type_device_and_programs();
                    type_Device_And_Programs.id = Convert.ToInt32(reader[0]);

                    type_Device_And_Programs.name = Convert.ToString(reader[1]);
                    list.Add(type_Device_And_Programs);
                }
                connection.Close();
                return list;
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }


            return null;
        }
        public Type_device_and_programs GetType_Device_And_Programs(int id)
        {
            try
            {
                Type_device_and_programs type_Device_And_Programs = null;
                MySqlConnection connection = new MySqlConnection(path);
                connection.Open();
                string sql_zapros1 = $"SELECT id, name FROM type_device_and_programs WHERE id='{id}'";

                MySqlCommand command = new MySqlCommand(sql_zapros1, connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    type_Device_And_Programs = new Type_device_and_programs();
                    type_Device_And_Programs.id = Convert.ToInt32(reader[0]);

                    type_Device_And_Programs.name = Convert.ToString(reader[1]);
                 
                }
                connection.Close();
                return type_Device_And_Programs;
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }


            return null;
        }
        public Organization GetOrganization(int id_organization)
        {
            try 
            {
                Organization o = null;
                MySqlConnection connection = new MySqlConnection(path);
                connection.Open();
                string sql_zapros1 = $"SELECT id, name FROM organization WHERE id='{id_organization}'";

                MySqlCommand command = new MySqlCommand(sql_zapros1, connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    o = new Organization();
                    o.id = Convert.ToInt32(reader[0]);

                    o.name = Convert.ToString(reader[1]);
                   
                }
                connection.Close();
                return o;
            } 
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            return null;
        }
        public List<Organization> GetListOrganization()
        {
            List<Organization> list_organization = new List<Organization>();
            MySqlConnection connection = new MySqlConnection(path);
            connection.Open();
            string sql_zapros1 = "SELECT id, name FROM organization;";

            MySqlCommand command = new MySqlCommand(sql_zapros1, connection);
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                list_organization.Add(new Organization() { id = Convert.ToInt32(reader[0]), name = reader[1].ToString() });

            }
            connection.Close();
            
                return list_organization;
           
        }
        public List<Organization> GetListOrganizationBy_IT_HUB(int id_HUB)
        {
            List<Organization> list_organization = new List<Organization>();
            MySqlConnection connection = new MySqlConnection(path);
            connection.Open();
            string sql_zapros1 = "SELECT id, name,id_hub FROM organization WHERE id_hub='"+id_HUB+"';";

            MySqlCommand command = new MySqlCommand(sql_zapros1, connection);
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                list_organization.Add(new Organization() { id = Convert.ToInt32(reader[0]), name = reader[1].ToString() });

            }
            connection.Close();

            return list_organization;

        }
        public List<Dovidnuk> GetDovidnukData()
        {
            List<Dovidnuk> data = new List<Dovidnuk>();
            MySqlConnection connection = new MySqlConnection(path);
            connection.Open();
            string sql_zapros1 = "SELECT id, name, description FROM kernelhelpbot.dovidnuk;";

            MySqlCommand command = new MySqlCommand(sql_zapros1, connection);
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                data.Add(new Dovidnuk() { id = Convert.ToInt32(reader[0]), name = reader[1].ToString(), description = reader[2].ToString() });

            }
            connection.Close();
            if(data.Count>0)
            return data;
            else
            return null;
        }
        public List<IT_HUB> Get_IT_HUBs()
        {
            try
            {
                List<IT_HUB> list_hubs = new List<IT_HUB>();
                MySqlConnection connection = new MySqlConnection(path);
                connection.Open();
                string sql_zapros1 = "SELECT id, name, otvetstvenniy FROM it_hub;";

                MySqlCommand command = new MySqlCommand(sql_zapros1, connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    list_hubs.Add(new IT_HUB() { id = Convert.ToInt32(reader[0]), name = reader[1].ToString(), otvetstvenniy = reader[2].ToString() });
                   
                }
                connection.Close();

                foreach (IT_HUB hub in list_hubs)
                {
                    hub.organizations = GetListOrganizationBy_IT_HUB(hub.id);
                }


                return list_hubs;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
            
        }
        public IT_HUB Get_IT_HUB(int id)
        {
            try
            {
                IT_HUB hub = null;
                MySqlConnection connection = new MySqlConnection(path);
                connection.Open();
                string sql_zapros1 = "SELECT id, name, otvetstvenniy FROM kernelhelpbot.it_hub WHERE id = '" + id+"';";

                MySqlCommand command = new MySqlCommand(sql_zapros1, connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    hub = new IT_HUB();
                    hub.id = Convert.ToInt32(reader[0]);
                    hub.name = reader[1].ToString();
                    hub.otvetstvenniy = reader[2].ToString();

                }
                connection.Close();

                return hub;

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }
        public async Task<IT_HUB> Get_IT_HUB_BY_ORGANIZATION_ID(int id)
        {
            try
            {
                IT_HUB hub = null;
                MySqlConnection connection = new MySqlConnection(path);
                connection.Open();

                string sql_zapros1 = "SELECT id_hub FROM organization WHERE id='"+id+"';";
                MySqlCommand command = new MySqlCommand(sql_zapros1, connection);
                int id_hub =  Convert.ToInt32(command.ExecuteScalarAsync().Result);

                sql_zapros1 = "SELECT id, name, otvetstvenniy,phone_number FROM kernelhelpbot.it_hub WHERE id = '" + id_hub + "';";

                 command = new MySqlCommand(sql_zapros1, connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    hub = new IT_HUB();
                    hub.id = Convert.ToInt32(reader[0]);
                    hub.name = reader[1].ToString();
                    hub.otvetstvenniy = reader[2].ToString();
                    hub.phone_number = reader[3].ToString();

                }
                connection.Close();

                return hub;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }
        public List<Otvetstvenniy>GetOtvetstvenniys()
        {
            try
            {
                List<Otvetstvenniy> otvetstvenniys = new List<Otvetstvenniy>();
                MySqlConnection connection = new MySqlConnection(path);
                connection.Open();
                string sql_zapros1 = "SELECT id, fio, phone_number FROM otvetstvennie_in_it_hub;";

                MySqlCommand command = new MySqlCommand(sql_zapros1, connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    otvetstvenniys.Add(new Otvetstvenniy() { id = Convert.ToInt32(reader[0]), fio = reader[1].ToString(), phone_number = reader[2].ToString() });

                }
                connection.Close();


                return otvetstvenniys;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }
        public bool UpdateNewOtvetstvenniy (int id_hub, string otv)
        {
            try
            {
                MySqlConnection connection = new MySqlConnection(path);
                connection.Open();
                string request = $"UPDATE `kernelhelpbot`.`it_hub` SET `otvetstvenniy` = '{otv}' WHERE (`id` = '{id_hub}');\r\n";
                MySqlCommand command = new MySqlCommand(request, connection);
                int rowsAffected = command.ExecuteNonQuery();

                connection.Close();

                return rowsAffected > 0;


            }
            catch (Exception ex)
            {
                Console.WriteLine("" + ex.Message);
                return false;
            }
        }
    }
}
