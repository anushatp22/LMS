A role-based Leave Management System built with .NET Web API, Angular, and SQL Server. Implements JWT authentication, employee onboarding, leave type management, leave balance logic, and leave application APIs.

Features
Authentication
• JWT-based login
• Role-based access: Admin & Employee
• First-login password reset
• Secure password hashing
Admin Features
• Create employees (no self-registration)
• Assign temporary password (email or manual sharing)
• Manage leave types (name + gender-based restrictions)
• Automatically assign initial leave balance on registration
• View employee leave history
Employee Features
• Login and change password
• View leave balance (per leave type)
• Apply for leave
• View applied leave status & history
Leave Management Logic
• LeaveType table (ID, Name, Gender restriction)
• LeaveBalance table (Allocated, Used, Year)
• Leave allocation on employee creation
• Yearly leave reset logic (on January 1st)
⸻
Tech Stack
Backend
• .NET 8 Web API
• C#
• Entity Framework Core
• SQL Server
Frontend
• Angular
• TypeScript
• HTML/CSS
Tools
• Visual Studio / VS Code
• Git & GitHub
• Postman

Backend (.NET API)
/Controllers
/Models
/DTOs
/Services
/Repositories
/Data
appsettings.json
Program.cs

Frontend (Angular)
/src
 /app
   /components
   /services
 index.html
 styles.css
