using EmployeeManagement.Application.Dtos.Employee;
using EmployeeManagement.Application.Service.Employees;
using EmployeeManagement.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace EmployeeManagement.Api.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<EmployeeController> _logger;

        public EmployeeController(IEmployeeService employeeService, ILogger<EmployeeController> logger)
        {
            _employeeService = employeeService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<EmployeeDto>>> GetAll()
        {
            _logger.LogInformation("Fetching employees {Time} ", DateTime.UtcNow);


            var result = await _employeeService.GetAllAsync();

            return Ok(result);
        }

        [HttpPost("paged")]
        public async Task<ActionResult<PaginatedResult<EmployeeDto>>> GetPaged([FromBody] EmployeePagedQuery query)
        {
            _logger.LogInformation("Fetching employees {Time} {query}", DateTime.UtcNow, query);

            var result = await _employeeService.GetPagedAsync(query);

            return Ok(result);
        }



        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeDto>> GetById(Guid id)
        {
            _logger.LogInformation("Fetching employees with id {id} - at {Time}", id, DateTime.UtcNow);

            var employee = await _employeeService.GetByIdAsync(id);
            if (employee is null) return NotFound();
            return Ok(employee);
        }

        [HttpPost]
        public async Task<ActionResult<EmployeeDto>> Create(CreateEmployeeDto dto)
        {
            _logger.LogInformation("Creating employee {dto} - at {Time}", dto, DateTime.UtcNow);

            var created = await _employeeService.CreateAsync(dto);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<EmployeeDto>> Update(Guid id, [FromBody] UpdateEmployeeDto dto)
        {
            _logger.LogInformation("Updating employee with ID {EmployeeId}, data: {@UpdateDto} - at {Timestamp}", id, dto, DateTime.UtcNow);

            var updated = await _employeeService.UpdateAsync(id, dto);
            return updated is null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            _logger.LogInformation("Delete employee with id {id} - at {Time}", id,  DateTime.UtcNow);

            await _employeeService.DeleteAsync(id);
            return NoContent();
        }
    }
}
