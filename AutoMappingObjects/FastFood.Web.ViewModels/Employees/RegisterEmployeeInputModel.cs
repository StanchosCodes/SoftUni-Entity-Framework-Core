using System.ComponentModel.DataAnnotations;

namespace FastFood.Web.ViewModels.Employees
{
    public class RegisterEmployeeInputModel
    {
        [StringLength(30, MinimumLength = 3)]
        public string Name { get; set; } = null!;

        [Range(15, 80)]
        public int Age { get; set; }

        public int PositionId { get; set; }

        public string PositionName { get; set; } = null!;

        [StringLength(30, MinimumLength = 3)]
        public string Address { get; set; } = null!;
    }
}
