🚀 WebApiSampleCreator

A C# console application that scaffolds a fully functional ASP.NET Core Web API project with just one prompt.  
Ideal for beginners or professionals who want a clean, ready-to-use API base with SQLite and Swagger support.

---

✨ Features

✅ Prompts for a project name  
📁 Creates a full Web API project on your Desktop  
📂 Automatically adds folders: Controllers, Models, Data  
📦 Installs essential NuGet packages (EF Core, SQLite, Swagger)  
🔧 Adds test GET/POST/PUT/DELETE endpoints  
📄 Enables Swagger UI  
💻 Asks to open project in VS Code

---

🛠 Technologies Used

.NET 9.0  
ASP.NET Core Web API  
Entity Framework Core (with SQLite)  
Swashbuckle (Swagger)  
System.Diagnostics, System.IO

---

📦 How to Use

Prerequisites

✔️ .NET 9.0 SDK  (https://dotnet.microsoft.com/)  
✔️ Visual Studio Code (https://code.visualstudio.com/) or another C# IDE  
❓ Git (optional)

Steps

1️⃣ Clone or download this repo  
2️⃣ Open terminal in the project folder  
3️⃣ Run:

   dotnet build  
   dotnet run

4️⃣ Enter a project name when prompted

The project will appear as a new folder on your Desktop

Open it in VS Code and run:

   dotnet restore  
   dotnet run

Navigate to /swagger to test the API

---

🌐 API Info  
Sample endpoints:

GET /api/sample  
POST /api/sample  
Swagger UI: https://localhost:{port}/swagger

---

📥 Download Demo  
Download compiled .exe (coming soon)

---

⚠️ Known Limitations

🖥️ Windows only (uses cmd.exe)  
💿 .NET SDK 9.0 is required  
🚫 Project name must not include symbols like / \ : * ? " < > |  
🔐 No authentication or validation logic (yet)  
🧪 No unit tests (yet)

---

🤝 Contribute  
Feel free to fork, use, and improve the project.  
Suggestions? Open an issue or pull request!
