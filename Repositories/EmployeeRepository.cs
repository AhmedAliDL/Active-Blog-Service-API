using Active_Blog_Service.Models;
using Active_Blog_Service_API.Context;
using Active_Blog_Service_API.Repositories.Contract;

namespace Active_Blog_Service_API.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext _context;

        public EmployeeRepository(AppDbContext context)
        {
            _context = context;
        }
        public List<Employee> GetEmployees()
        {
            return _context.Employees.ToList();
        }

        public Employee GetEmployeeById(int id)
        {
            return _context.Employees.FirstOrDefault(e => e.Id == id)!;

        }
        public void AddEmployee(Employee employee)
        {
            _context.Employees.Add(employee);
            _context.SaveChanges();
        }


    }
}
