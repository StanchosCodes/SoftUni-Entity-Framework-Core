using System.ComponentModel.DataAnnotations;

namespace FastFood.Web.ViewModels.Categories
{
    public class CreateCategoryInputModel
    {
        // Can also be done with two attributes (Min and Max length)
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Category Name should be between 3 and 30 charecters!")]
        public string CategoryName { get; set; } = null!;
    }
}
