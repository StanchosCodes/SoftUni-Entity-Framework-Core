namespace FastFood.Web.Controllers
{
    using System;
    using AutoMapper;
    using Data;
    using FastFood.Services.Data;
    using Microsoft.AspNetCore.Mvc;
    using ViewModels.Employees;

    public class EmployeesController : Controller
    {
        private readonly EmployeeService employeeService;

        public EmployeesController(EmployeeService employeeService)
        {
            this.employeeService = employeeService;
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            IEnumerable<RegisterEmployeeViewModel> availablePositions = 
                await this.employeeService.GetPositionsIdAsync();

            return View(availablePositions);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterEmployeeInputModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToAction("Error", "Home");
            }

            await this.employeeService.RegisterAsync(model);

            return this.RedirectToAction("All");
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            IEnumerable<EmployeesAllViewModel> employees = await this.employeeService.GetAllAsync();

            return this.View(employees.ToList());
        }
    }
}
