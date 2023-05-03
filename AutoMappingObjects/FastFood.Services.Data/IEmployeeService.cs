using FastFood.Web.ViewModels.Employees;

namespace FastFood.Services.Data
{
    public interface IEmployeeService
    {
        Task RegisterAsync(RegisterEmployeeInputModel model);

        Task<IEnumerable<EmployeesAllViewModel>> GetAllAsync();

        Task<IEnumerable<RegisterEmployeeViewModel>> GetPositionsIdAsync();
    }
}
