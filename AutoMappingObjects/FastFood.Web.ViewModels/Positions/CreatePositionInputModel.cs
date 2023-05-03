using System.ComponentModel.DataAnnotations;

namespace FastFood.Web.ViewModels.Positions
{
    public class CreatePositionInputModel
    {

        [StringLength(30, MinimumLength = 3, ErrorMessage = "Position Name should be between 3 and 30 charecters!")]
        public string PositionName { get; set; } = null!;
    }
}