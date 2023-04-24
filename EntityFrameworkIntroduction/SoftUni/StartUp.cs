using SoftUni.Data;
using SoftUni.Models;
using System.Data.SqlTypes;
using System.Text;

namespace SoftUni;

public class StartUp
{
    static void Main(string[] args)
    {
        SoftUniContext context = new SoftUniContext();

        // string employees = GetEmployeesFullInformation(context); // third task
        // string employees = GetEmployeesWithSalaryOver50000(context); // fourth task
        // string employees = GetEmployeesFromResearchAndDevelopment(context); // fifth task
        // string addresses = AddNewAddressToEmployee(context); // sixth task
        // string employees = GetEmployeesInPeriod(context); // seventh task
        // string addresses = GetAddressesByTown(context); // eight task
        // string employee = GetEmployee147(context); // ninth task
        // string department = GetDepartmentsWithMoreThan5Employees(context); // tenth task
        // string projects = GetLatestProjects(context); // eleventh task
        // string employees = IncreaseSalaries(context); // twelved task
        // string employees = GetEmployeesByFirstNameStartingWithSa(context); // thirteened task
        // string projects = DeleteProjectById(context); // fourteened task
        string deletedAddressesCount = RemoveTown(context); // fifteened task

        Console.WriteLine(deletedAddressesCount);
    }

    // Third task
    public static string GetEmployeesFullInformation(SoftUniContext context)
    {
        StringBuilder result = new StringBuilder();

        var employees = context.Employees
            .OrderBy(e => e.EmployeeId)
            .Select(e => new
            {
                e.FirstName,
                e.LastName,
                e.MiddleName,
                e.JobTitle,
                e.Salary
            })
            .ToArray(); // materializes the query and detaches it from the change tracker

        foreach (var e in employees)
        {
            result.AppendLine($"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle} {e.Salary:f2}");
        }

        return result.ToString();
    }

    // Fourth task
    public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
    {
        StringBuilder result = new StringBuilder();

        var employees = context.Employees
            .OrderBy(e => e.FirstName)
            .Select(e => new
            {
                e.FirstName,
                e.Salary
            })
            .Where(e => e.Salary > 50000)
            .ToArray();

        foreach (var e in employees)
        {
            result.AppendLine($"{e.FirstName} - {e.Salary:f2}");
        }

        return result.ToString();
    }

    // Fifth task
    public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
    {
        StringBuilder result = new StringBuilder();

        var employees = context.Employees
            .Where(e => e.Department.Name == "Research and Development")
            .OrderBy(e => e.Salary)
            .ThenByDescending(e => e.FirstName)
            .Select(e => new
            {
                e.FirstName,
                e.LastName,
                e.Department.Name,
                e.Salary
            })
            .ToArray();

        foreach (var e in employees)
        {
            result.AppendLine($"{e.FirstName} {e.LastName} from {e.Name} - ${e.Salary:f2}");
        }

        return result.ToString();
    }

    // Sixth task
    public static string AddNewAddressToEmployee(SoftUniContext context)
    {
        Address newAddress = new Address // making a new address(Address object)
        {
            AddressText = "Vitoshka 15",
            TownId = 4
        };

        // context.Addresses.Add(newAddress); - that how we add new entity to a table in the database - however we dont need to do it because entityFramework checks if the address is in the Addresses table when we try to set it to the employee and if not it includes it implicitly, we dont need to do it explicitly

        Employee? searchedEmployee = context.Employees // taking the employee we want
            .FirstOrDefault(e => e.LastName == "Nakov");

        searchedEmployee!.Address = newAddress; // setting the new address to the desired employee

        context.SaveChanges(); // saves the made changes in the database

        string[] addresses = context.Employees
            .OrderByDescending(e => e.AddressId)
            .Take(10)
            .Select(e => e.Address!.AddressText)
            .ToArray();

        return String.Join(Environment.NewLine, addresses);
    }

    // Seventh task
    public static string GetEmployeesInPeriod(SoftUniContext context)
    {
        StringBuilder result = new StringBuilder();

        var employees = context.Employees
            .Take(10)
            .Select(e => new // Taking employees and managers info
            {
                e.FirstName,
                e.LastName,
                ManagerFirstName = e.Manager!.FirstName,
                ManagerLastName = e.Manager.LastName,
                Projects = e.EmployeesProjects // Taking all projects assigned to the employees
                .Select(ep => new
                {
                    ep.Project.Name,
                    ProjectYear = ep.Project.StartDate.Year, // taking only the year of the startDate to compare it later in foreach
                    ProjectStartDate = ep.Project.StartDate.ToString("M/d/yyyy h:mm:ss tt"), // converting the date
                    ProjectEndDate = ep.Project.EndDate.HasValue // checking if there is EndDate
                         ? ep.Project.EndDate.Value.ToString("M/d/yyyy h:mm:ss tt") // if true taking it
                         : "not finished" // if false setting desired string
                })
                .ToArray()
            })
            .ToArray();

        foreach (var e in employees)
        {
            result.AppendLine($"{e.FirstName} {e.LastName} - Manager: {e.ManagerFirstName} {e.ManagerLastName}");

            foreach (var p in e.Projects)
            {
                if (p.ProjectYear >= 2001 && p.ProjectYear <= 2003) // comparing the year of the startDate of the project
                {
                    result.AppendLine($"--{p.Name} - {p.ProjectStartDate} - {p.ProjectEndDate}");
                }
            }
        }

        return result.ToString();
    }

    // Eight task
    public static string GetAddressesByTown(SoftUniContext context)
    {
        StringBuilder result = new StringBuilder();

        var addresses = context.Addresses
            .OrderByDescending(a => a.Employees.Count())
            .ThenBy(a => a.Town!.Name)
            .ThenBy(a => a.AddressText)
            .Take(10)
            .Select(a => new
            {
                a.AddressText,
                TownName = a.Town!.Name,
                EmployeeCount = a.Employees.Count()
            })
            .ToArray();

        foreach (var a in addresses)
        {
            result.AppendLine($"{a.AddressText}, {a.TownName} - {a.EmployeeCount} employees");
        }

        return result.ToString();
    }

    // Ninth task
    public static string GetEmployee147(SoftUniContext context)
    {
        StringBuilder result = new StringBuilder();

        var employee = context.Employees
            .Where(e => e.EmployeeId == 147)
            .Select(e => new
            {
                e.FirstName,
                e.LastName,
                e.JobTitle,
                Projects = e.EmployeesProjects
                   .OrderBy(e => e.Project.Name)
                   .Select(ep => new
                   {
                       ProjectName = ep.Project.Name
                   })
                   .ToArray()
            })
            .ToArray();

        foreach (var e in employee) // There always will be only one employee with the given id
        {
            result.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle}");

            foreach (var p in e.Projects)
            {
                result.AppendLine(p.ProjectName);
            }
        }

        return result.ToString();
    }
    
    // Tenth task
    public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
    {
        StringBuilder result = new StringBuilder();

        var departmentsEmployees = context.Departments
            .OrderBy(d => d.Employees.Count())
            .Where(d => d.Employees.Count() > 5)
            .Select(d => new
            {
                DepartmentName = d.Name,
                ManagerFirstName = d.Manager.FirstName,
                ManagerLastName = d.Manager.LastName,
                Employees = d.Employees
                  .OrderBy(e => e.FirstName)
                  .ThenBy(e => e.LastName)
                  .Select(e => new
                  {
                      EmployeeFirstName = e.FirstName,
                      EmployeeLastName = e.LastName,
                      EmployeeJobTitle = e.JobTitle
                  })
                  .ToArray()
            })
            .ToArray();

        foreach (var d in departmentsEmployees)
        {
            result.AppendLine($"{d.DepartmentName} - {d.ManagerFirstName} {d.ManagerLastName}");

            foreach (var e in d.Employees)
            {
                result.AppendLine($"{e.EmployeeFirstName} {e.EmployeeLastName} - {e.EmployeeJobTitle}");
            }
        }

        return result.ToString();
    }

    // Eleventh task
    public static string GetLatestProjects(SoftUniContext context)
    {
        StringBuilder result = new StringBuilder();

        var projects = context.Projects
            .OrderByDescending(p => p.StartDate)
            .Take(10)
            .OrderBy(p => p.Name)
            .Select(p => new
            {
                p.Name,
                p.Description,
                StartDate = p.StartDate.ToString("M/d/yyyy h:mm:ss tt")
            })
            .ToArray();

        foreach (var p in projects)
        {
            result.AppendLine($"{p.Name}");
            result.AppendLine($"{p.Description}");
            result.AppendLine($"{p.StartDate}");
        }

        return result.ToString();
    }

    // Twelved task
    public static string IncreaseSalaries(SoftUniContext context)
    {
        StringBuilder result = new StringBuilder();

        var employees = context.Employees
            .OrderBy(e => e.FirstName)
            .ThenBy(e => e.LastName)
            .Where(e => e.Department.Name == "Engineering" || e.Department.Name == "Tool Design" || e.Department.Name == "Marketing" || e.Department.Name == "Information Services");

        foreach (var e in employees)
        {
            e.Salary *= 1.12m;
        }

        context.SaveChanges();

        foreach (var e in employees)
        {
            result.AppendLine($"{e.FirstName} {e.LastName} (${e.Salary:f2})");
        }

        return result.ToString();
    }

    // Thirteened task
    public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
    {
        StringBuilder result = new StringBuilder();

        var employees = context.Employees
            .OrderBy(e => e.FirstName)
            .ThenBy(e => e.LastName)
            .Where(e => e.FirstName.Substring(0, 2).ToLower() == "sa")
            .Select(e => new
            {
                e.FirstName,
                e.LastName,
                e.JobTitle,
                e.Salary
            })
            .ToArray();

        foreach (var e in employees)
        {
            result.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle} - (${e.Salary:f2})");
        }

        return result.ToString();
    }

    // Fourteened task
    public static string DeleteProjectById(SoftUniContext context)
    {
        // deleting from employeesProjects first because of the reference with the projets table
        var empProjToDelete = context.EmployeesProjects // taking the employeesProjects that we have to delete
            .Where(ep => ep.ProjectId == 2);

        context.EmployeesProjects.RemoveRange(empProjToDelete); // removing the range of empProjects

        // taking the project than we want to delete
        Project projectToDelete = context.Projects.Find(2)!;

        context.Projects.Remove(projectToDelete); // Removing the project

        context.SaveChanges(); // Always saving the changes in the end

        // Taking the names of the projects
        string[] projectNames = context.Projects
            .Take(10)
            .Select(p => p.Name)
            .ToArray();

        return String.Join(Environment.NewLine, projectNames); // returning the project names each on a new line
    }

    // Fifteened task
    public static string RemoveTown(SoftUniContext context)
    {
        var townToDelete = context.Towns
            .FirstOrDefault(t => t.Name == "Seattle");

        var addressesToDelete = context.Addresses
            .Where(a => a.TownId == townToDelete!.TownId)
            .ToArray();

        foreach (var e in context.Employees)
        {
            if (addressesToDelete.Any(a => a.AddressId == e.AddressId))
            {
                e.AddressId = null;
            }
        }

        int nullSettedAddresses = addressesToDelete.Length;

        context.Addresses.RemoveRange(addressesToDelete);
        context.Towns.Remove(townToDelete!);

        context.SaveChanges();

        string result = $"{nullSettedAddresses} addresses in Seattle were deleted";

        return result;
    }
}