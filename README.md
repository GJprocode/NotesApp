# NotesApp

# Notes Application

This is a note-taking application built with Vue.js, C#, and SQL Server.

## Features

* **Create Notes:**
    * Users can create a note with the following fields:
        * Title (mandatory)
        * Content (optional)
        * Created At (auto-generated date)
        * Updated At (auto-generated when edited)
* **Read Notes:**
    * Users can view a list of their notes, showing the title and creation date.
    * Users can click on a note to view its content and full details.
* **Update Notes:**
    * Users can edit the title and content of a note.
    * The Updated At timestamp should be updated whenever the note is modified.
* **Delete Notes:**
    * Users can delete any note.
    * After deletion, the note should be removed from the list.

## Technical Stack

* **Front-End:** Vue.js, Typescript, Tailwind CSS
* **Back-End:** C# (ASP.NET Core Web API)
* **Database:** SQL Server
* **IDE:** Visual studio code
  

## Functional Requirements

### Frontend

* (Optional) Create login/register forms.
* Notes list page which covers all of these CRUD operations (Create, Read, Update, Delete).
* Simple filtering and sorting functionality.
* Search functionality.
* Responsive design using Tailwind CSS.
* Perform basic API integrations using Axios or Fetch.
* State management.

### Backend

* (Optional) Implement authentication and authorization.
* Create, read, update, and delete (CRUD) notes.
* User can only read, update, and delete their own notes.
* Using Dapper for ORM with a SQL Server database.

This is a basic outline for the Notes application. The specific implementation details may vary depending on your project requirements.
