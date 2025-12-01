using Active_Blog_Service.Models;

namespace Active_Blog_Service_API.Repositories.Contract
{
    public interface IEmployeeRepository : IAddScoped
    {
        List<Employee> GetEmployees();
        Employee GetEmployeeById(int id);
        void AddEmployee(Employee employee);
    }
}
