using KernelHelpBot.Models.People_Information;
using KernelHelpBot.Models.TechniksInformation;

using MySql.Data.MySqlClient;
using System.IO;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;

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
                MySqlConnection connection = new MySqlConnection(path);
                connection.Open();
                string request = $"SELECT COUNT(*) FROM kernelhelpbot.users WHERE telegram_id = '{user.telegram_data.telegram_id}'";
                MySqlCommand command = new MySqlCommand(request, connection);
                int result_request_check_user = Convert.ToInt32(command.ExecuteScalar());
                if (result_request_check_user == 0)
                {
                    request = $"INSERT INTO `kernelhelpbot`.`users` (`telegram_id`, `name`, `surname`, `email`, `phone_number`, `work_position`, `username`, `fisrtname`, `lastname`, `last_message`, `description`, `departament`) VALUES ('{user.telegram_data.telegram_id}', '{user.name}', '{user.surname}', '{user.email}', '{user.telegram_data.phone_number}', '{user.work_position}', '{user.telegram_data.username}', '{user.telegram_data.fisrtname}', '{user.telegram_data.lastname}', '{user.telegram_data.last_message}', '{user.description}', '{user.department}');\r\n";
                    command = new MySqlCommand(request, connection);
                    command.ExecuteScalar();

                    connection.Close();
                    return true;
                }
                else
                {

                    request = $"UPDATE `kernelhelpbot`.`users` SET `name` = '{user.name}', `surname` = '{user.surname}', `email` = '{user.email}', `phone_number` = '{user.telegram_data.phone_number}', `work_position` = '{user.work_position}', `username` = '{user.telegram_data.username}', `fisrtname` = '{user.telegram_data.fisrtname}', `lastname` = '{user.telegram_data.lastname}', `last_message` = '{user.telegram_data.last_message}', `description` = '{user.description}', `departament` = '{user.department}' WHERE(`id` = '{user.telegram_data.telegram_id}');";
                    command = new MySqlCommand(request, connection);
                    command.ExecuteScalar();
                    connection.Close();
                    Console.WriteLine("Користувач вже є у базі даних");
                    return true;
                }

            }
                catch (Exception ex)
            {
                Console.WriteLine("Виникла помилка. Помилка у DatabaseUserInfo, у методі AddNewUser. Помилка:\n" + ex.Message);
                return false;
            }
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
                $"lastname, last_message, access, description, departament,active FROM users WHERE telegram_id='{telegramId}'";

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
                    u.actice = Convert.ToBoolean(reader[14]);

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
        public async Task<List<User>> GetAllUsers()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(path))
                {
                    await connection.OpenAsync();
                    string request = $"SELECT id, telegram_id, name, surname, email, phone_number, work_position, username, fisrtname, lastname, last_message, access, description, departament, active FROM users";
                    MySqlCommand command = new MySqlCommand(request, connection);
                    List<User> users = new List<User>();
                    MySqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        User u = new User();
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
                        u.actice = Convert.ToBoolean(reader[14]);
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
    }
}
