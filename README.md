# Notes Application Backend

This is the backend service for the Notes Application. It provides APIs to handle note-related CRUD operations and user authentication. 
Built with **C# ASP.NET Core Web API** and uses **SQL Server** as the database.


## Features

1. **User Authentication & Authorization**
   - Login and Register endpoints.
   - Users can only access their own notes.

2. **CRUD Operations**
   - Create, Read, Update, and Delete notes.
   - Notes have the following fields:
     - Title (string, required)
     - Content (string, optional)
     - CreatedAt (datetime, auto-generated)
     - UpdatedAt (datetime, auto-updated when edited).

3. **Database Integration**
   - SQL Server is used to store user and notes data.
   - **Dapper** is used as the ORM for database operations.

4. **Secure APIs**
   - Protect routes with authentication and authorization.

## Requirements

- .NET 6 or higher
- SQL Server
- Microsoft Visual Studio or VS Code
- ASP.NET Core Web API
- Dapper (ORM)
- JWT(Authentication)

## Setup Instructions

1. **Clone the Repository**:
   ```bash
   git clone <backend-repo-url>
   cd NotesAppBackend
   ```

2. **Configure the Database**:
   - Update the connection string in `appsettings.json` to match your SQL Server setup:
     ```json
     "ConnectionStrings": {
       "DefaultConnection": "Server=<Your_Server>;Database=NotesApp;User Id=<Your_Username>;Password=<Your_Password>;"
     }
     ```

3. **Run Database Migrations**:
   - Use the Entity Framework CLI (optional)
   - connect to your SMSS database. 
    log in with Win machine or SQL Auth      login(secure and preferred)

4. **Run the Application**:
   ```bash
   dotnet clean
   dotnet build
   dotnet run
   ```

## Notes
- Certificates setup for secure https requests from frontend. 
- Ensure the backend URL is properly configured in the frontend for API requests.
- Use tools ike Postman or Swagger(API Documentation) for testing APIs during development.
