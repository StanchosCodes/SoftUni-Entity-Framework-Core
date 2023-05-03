namespace FastFood.Web.Controllers
{
    using System;
    using System.Linq;
    using AutoMapper;
    using Data;
    using FastFood.Services.Data;
    using Microsoft.AspNetCore.Mvc;
    using ViewModels.Orders;

    public class OrdersController : Controller
    {
        private readonly IOrderService orderService;

        public OrdersController(IOrderService orderService)
        {
            this.orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            //var viewOrder = new CreateOrderViewModel
            //{
            //    Items = _context.Items.Select(x => x.Id).ToList(),
            //    Employees = _context.Employees.Select(x => x.Id).ToList(),
            //};

            IEnumerable<CreateOrderInputModel> items = await this.orderService.GetItemsAsync();
            IEnumerable<CreateOrderInputModel> employees = await this.orderService.GetEmployeesAsync();

            var viewOrder = new CreateOrderViewModel
            {
                Items = items.Select(i => i.ItemId).ToList(),
                Employees = employees.Select(e => e.EmployeeId).ToList()
            };

            return View(viewOrder);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateOrderInputModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return RedirectToAction("Error", "Home");
            }

            await this.orderService.CreateAsync(model);

            return RedirectToAction("All", "Orders");
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            IEnumerable<OrderAllViewModel> orders = await this.orderService.GetAllAsync();

            return this.View(orders.ToList());
        }
    }
}
