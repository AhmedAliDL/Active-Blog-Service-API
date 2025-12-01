using Active_Blog_Service.Models;
using Active_Blog_Service_API.Dto;
using Active_Blog_Service_API.Repositories.Contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Active_Blog_Service_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TeamController : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepository;
       
        public TeamController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }
        [HttpGet("index")]
        public IActionResult Index()
        {
            var employees = _employeeRepository.GetEmployees();
            return Ok(employees);
        }
        [HttpPost("AddEmployee")]
        public async Task<IActionResult> Add([FromForm]AddEmployeeDto employee)
        {
            if (ModelState.IsValid)
            {
                var emp = new Employee
                {
                    Name = employee.Name,
                    Address = employee.Address,
                    Salary  = employee.Salary,
                    Age = employee.Age,
                    Email = employee.Email,
                    PhoneNumber = employee.PhoneNumber,
                    Role = employee.Role,
                    RoleDescription = employee.RoleDescription,
                    DepartmentId = employee.DepartmentId,
                };
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/EmployeeImages"); // Ensure this folder exists
                var fileName = Path.GetFileName(employee.Image.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                // Save the file to the specified path
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await employee.Image.CopyToAsync(stream);
                }
               
                // Save the image path to the user model
                emp.Image = $"/EmployeeImages/{fileName}";
                _employeeRepository.AddEmployee(emp);
                return Ok();
            }
            return BadRequest("Data has somthing wrong!");
        }
    }
}

