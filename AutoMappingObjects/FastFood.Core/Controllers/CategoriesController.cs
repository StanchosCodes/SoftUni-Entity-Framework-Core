namespace FastFood.Web.Controllers
{
    using System;
    using AutoMapper;
    using Data;
    using FastFood.Services.Data;
    using Microsoft.AspNetCore.Mvc;
    using ViewModels.Categories;

    public class CategoriesController : Controller
    {
        private readonly ICategoryService categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryInputModel model)
        {
            // Check if the validation of the input passes
            if (!this.ModelState.IsValid)
            {
                // if make an error action and redirect to Home page
                return this.RedirectToAction("Error", "Home");
            }

            await this.categoryService.CreateAsync(model);

            // if everything goes ok we redirect to "All page" in category page
            return this.RedirectToAction("All");
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            IEnumerable<CategoryAllViewModel> categories = await this.categoryService.GetAllAsync();

            return this.View(categories.ToList());
        }
    }
}
