namespace TestApiProj.Models.FakerapiModel
{
    public class EmployeeResponse
    {
        public string Status { get; set; }
        public int Code { get; set; }
        public string Locale { get; set; }
        public object Seed { get; set; }
        public int Total { get; set; }
        public List<Employee> Data { get; set; }
    }
}
