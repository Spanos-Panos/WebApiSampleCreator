using System;
using System.Diagnostics;
using System.IO;





namespace WebApiSampleCreator
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.Title = "WEBAPI Sample";
                Console.WriteLine("Creating new ASP.NET Web API project...");

                Console.Write("Please enter the name of the Project: ");
                string projectName = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(projectName) || projectName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid project name. Avoid using characters like : ? * < > | \\\" / ");
                    Console.WriteLine("A detailed error has been saved to error.log on your Desktop.");
                    Console.WriteLine("Press any key to close the app.");
                    Console.ResetColor();
                    string desktopPathForLog = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                    string logPath = Path.Combine(desktopPathForLog, "error.log");
                    string tip = "Tip: Avoid using invalid characters in project names (e.g., : ? * < > | \\\" / ).";
                    string errorMsg = "Error: The project name contains invalid characters.";
                    string solution = "Solution: Use only letters, numbers, underscores, or dashes in your project name.";
                    File.AppendAllText(logPath, $"{tip}\n\n{errorMsg}\n\n{solution}\n\n[{DateTime.Now}]\n\n");
                    Console.ReadKey();
                    return;
                }

                Console.WriteLine($"Creating Project: {projectName}");
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string projectPath = Path.Combine(desktopPath, projectName);
                // Create the project directly on the desktop
                Process.Start("cmd.exe", $"/C dotnet new webapi -n {projectName} -o \"{projectPath}\"").WaitForExit();

                // Add or overwrite custom files in the generated project folder
                Directory.CreateDirectory(Path.Combine(projectPath, "Models"));
                Directory.CreateDirectory(Path.Combine(projectPath, "Data"));
                Directory.CreateDirectory(Path.Combine(projectPath, "Controllers"));

                File.WriteAllText(Path.Combine(projectPath, "Models", "SampleItem.cs"), SampleItemCode());
                File.WriteAllText(Path.Combine(projectPath, "Data", "SampleDbContext.cs"), SampleDbContextCode());
                File.WriteAllText(Path.Combine(projectPath, "Controllers", "SampleController.cs"), SampleControllerCode());
                File.WriteAllText(Path.Combine(projectPath, "appsettings.json"), AppSettingsJson());
                File.WriteAllText(Path.Combine(projectPath, "Program.cs"), ProgramCsCode());

                Console.WriteLine("Installing NuGet Packages...");
                string[] packages =
                {
                    "Microsoft.EntityFrameworkCore.Sqlite",
                    "Microsoft.EntityFrameworkCore.Tools",
                    "Microsoft.EntityFrameworkCore.Design",
                    "Swashbuckle.AspNetCore"
                };
                foreach (var pkg in packages)
                {
                    Process.Start("cmd.exe", $"/C cd \"{projectPath}\" && dotnet add package {pkg}").WaitForExit();
                }

                // Run EF Core migration commands to create the database and tables
                Console.WriteLine("\nRunning Entity Framework Core migrations...");
                Process.Start("cmd.exe", $"/C cd \"{projectPath}\" && dotnet ef migrations add InitialCreate").WaitForExit();
                Process.Start("cmd.exe", $"/C cd \"{projectPath}\" && dotnet ef database update").WaitForExit();

                Console.WriteLine("\nProject created!! You can find the folder on your desktop.\n");
                Console.Write("Open the project folder now? (Y/N): ");
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Y)
                {
                    Process.Start("cmd.exe", $"/C cd \"{projectPath}\" && code .");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }

        }

        static void HandleError(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            string tip = "";
            string errorMsg = "";
            string solution = "";

            // .NET not installed
            if (ex is System.ComponentModel.Win32Exception win32Ex &&
                win32Ex.Message.Contains("The system cannot find the file specified"))
            {
                Console.WriteLine("Error: The .NET SDK was not found on your system.");
                Console.WriteLine("Please install .NET 9.0 from: https://dotnet.microsoft.com/en-us/download/dotnet/9.0");
                tip = "Tip: Install .NET 9.0 from https://dotnet.microsoft.com/en-us/download/dotnet/9.0";
                errorMsg = "Error: The .NET SDK was not found on your system.";
                solution = "Solution: Download and install .NET 9.0 from the official website.";
            }
            else if (ex is IOException)
            {
                Console.WriteLine("File or directory error: " + ex.Message);
                Console.WriteLine("Please check if the files or folders are in use or if you have enough disk space.");
                Console.WriteLine("You may try again after closing any open files.");
                tip = "Tip: Avoid using invalid characters in project names (e.g., : ? * < > | \\\" / ). Close any open files or folders and try again.";
                errorMsg = "Error: File or directory error: " + ex.Message;
                solution = "Solution: Use only valid characters in names and ensure files/folders are not in use.";
            }
            else if (ex is UnauthorizedAccessException)
            {
                Console.WriteLine("Permission error: " + ex.Message);
                Console.WriteLine("Try running the app as administrator.");
                tip = "Tip: Run the app as administrator if you get permission errors.";
                errorMsg = "Error: Permission error: " + ex.Message;
                solution = "Solution: Right-click the app and select 'Run as administrator'.";
            }
            else if (ex is ArgumentException)
            {
                Console.WriteLine("Invalid argument: " + ex.Message);
                Console.WriteLine("Please check your input and try again.");
                tip = "Tip: Check your input for invalid characters or values.";
                errorMsg = "Error: Invalid argument: " + ex.Message;
                solution = "Solution: Use only valid input values.";
            }
            else if (ex is System.ComponentModel.Win32Exception win32Ex2 &&
                     win32Ex2.Message.Contains("exited with code"))
            {
                Console.WriteLine("A system command failed to execute properly. Please check your .NET installation and project files.");
                tip = "Tip: Make sure .NET is installed and your project files are not corrupted.";
                errorMsg = "Error: A system command failed to execute properly.";
                solution = "Solution: Reinstall .NET and check your project files.";
            }
            else
            {
                Console.WriteLine("An unexpected error occurred: " + ex.Message);
                tip = "Tip: See the error details below and check your environment or input.";
                errorMsg = "Error: " + ex.Message;
                solution = "Solution: Review the error details and check your environment or input.";
            }

            Console.ResetColor();

            // Log the tip, error, and solution to error.log on the Desktop
            try
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string logPath = Path.Combine(desktopPath, "error.log");
                File.AppendAllText(logPath, $"{tip}\n\n{errorMsg}\n{solution}\n\n[{DateTime.Now}]\n\nStackTrace:\n{ex.StackTrace}\n\n");
            }
            catch { }

            string logPathMsg = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "error.log");
            Console.WriteLine("\nSomething went wrong. Details have been saved to 'error.log' on your Desktop.");
            Console.WriteLine($"You can find the error log at: {logPathMsg}");
            Console.WriteLine("This file was created to help you or a developer understand what went wrong. Please review the tip, error, and solution in the log file.");
            Console.WriteLine("Press any key to close the app.");
            Console.ReadKey();
        }

        static string SampleItemCode() => """
namespace WebApiSample.Models
{
    public class SampleItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsCompleted { get; set; }
    }
}
""";

        static string SampleDbContextCode() => """
using Microsoft.EntityFrameworkCore;
using WebApiSample.Models;

namespace WebApiSample.Data
{
    public class SampleDbContext : DbContext
    {
        public SampleDbContext(DbContextOptions<SampleDbContext> options)
            : base(options) { }

        public DbSet<SampleItem> Items { get; set; }
    }
}
""";

        static string SampleControllerCode() => """
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiSample.Data;
using WebApiSample.Models;

namespace WebApiSample.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SampleController : ControllerBase
    {
        private readonly SampleDbContext _context;

        public SampleController(SampleDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SampleItem>>> Get()
        {
            return await _context.Items.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<SampleItem>> Post(SampleItem item)
        {
            _context.Items.Add(item);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, SampleItem item)
        {
            if (id != item.Id)
                return BadRequest();

            _context.Entry(item).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Items.Any(e => e.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
                return NotFound();

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
""";

        static string AppSettingsJson() => """
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=sample.db"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
""";

        static string ProgramCsCode() => """
using Microsoft.EntityFrameworkCore;
using WebApiSample.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<SampleDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
""";
    }
}
