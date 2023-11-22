using KernelHelpBot.Models.People_Information;

namespace KernelHelpBot.Models.TechniksInformation
{
    public class Device_and_Programs
    {
        public int id {  get; set; }    
        public Type_device_and_programs type_Device_And_Programs { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public Organization assigned_organization { get; set; }
        public List<Problem_for_type_device_and_programs> list_problems { get; set; }
    }
}
