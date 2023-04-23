using MiniORM.App.Data;
using MiniORM.App.Data.Entities;

namespace MiniORM.App;

public class StartUp
{
    static void Main(string[] args)
    {
        SoftUniDbContext dbContext = new SoftUniDbContext(Config.ConnectionString);

        Console.WriteLine("Connection Successfull!");

        //dbContext.Employees.Add(new Employee
        //{
        //    FirstName = "Gosho",
        //    LastName = "Inserted",
        //    DepartmentId = dbContext.Departments.First().Id,
        //    IsEmployed = true
        //});

        Employee employeeToDelete = dbContext.Employees.First(e => e.Id == 7);
        dbContext.Employees.Remove(employeeToDelete);

        Employee lastEmployee = dbContext.Employees.Last();
        Console.WriteLine(lastEmployee.ToString());
        Console.WriteLine();
        Console.WriteLine($"First name of the new employee: {lastEmployee.FirstName}");
        Console.WriteLine();
        Console.WriteLine($"Last name of the new employee: {lastEmployee.LastName}");
        Console.WriteLine();

        //lastEmployee.FirstName = "Modified";
        //Console.WriteLine($"The first name has been changed to: {lastEmployee.FirstName}");

        dbContext.SaveChanges();
    }
}