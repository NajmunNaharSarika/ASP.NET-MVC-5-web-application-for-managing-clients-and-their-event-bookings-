# 🎉 Event Management System

> A full-stack **ASP.NET MVC 5** web application for managing clients and their event bookings, featuring ASP.NET Identity authentication, Entity Framework (Database-First), and a custom dark-themed UI.

---

## 📋 Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Tech Stack](#tech-stack)
- [Project Structure](#project-structure)
- [Database Schema](#database-schema)
- [Controllers & Routes](#controllers--routes)
- [Authentication & Authorization](#authentication--authorization)
- [Getting Started](#getting-started)
- [Key Implementation Details](#key-implementation-details)

---

## Overview

The Event Management System allows administrators to manage **Clients** and **Events**, and link them together through **Event Services**. Clients can be registered with personal details and a profile picture, then enrolled in one or more events. The system enforces login-based access control — public visitors can browse the event and client lists, but all create/edit/delete operations require authentication.

---

## Features

- ✅ **Client Management** — Create, edit, and delete clients with profile photo upload
- ✅ **Event Management** — Full CRUD for events with safe cascading delete
- ✅ **Event Booking** — Assign multiple events to a single client via `EventService` junction table
- ✅ **Profile Picture Upload** — Client images saved to `/Images/` with timestamped filenames
- ✅ **ASP.NET Identity** — Login, registration, password management, two-factor auth support
- ✅ **Authorization** — `[Authorize]` on all write operations; `[AllowAnonymous]` on index/browse views
- ✅ **Partial Views** — `_addNewEvent` partial for dynamic event selection on client forms
- ✅ **Cascading Delete** — Child `EventService` records removed before deleting a parent Event or Client
- ✅ **Dark UI Theme** — Custom purple/magenta gradient design system with CSS variables

---

## Tech Stack

| Layer | Technology |
|---|---|
| **Framework** | ASP.NET MVC 5 (.NET Framework 4.x) |
| **ORM** | Entity Framework 6 — Database First (EDMX) |
| **Auth** | ASP.NET Identity 2 (OWIN middleware) |
| **Database** | Microsoft SQL Server |
| **Views** | Razor (`.cshtml`) |
| **Frontend** | Bootstrap, Font Awesome, custom CSS |

---

## Project Structure

```
Event_Management_System/
│
├── Controllers/
│   ├── AccountController.cs      # Login, Register, external logins (ASP.NET Identity)
│   ├── ManageController.cs       # Password change, phone number, 2FA management
│   ├── ClientsController.cs      # Client CRUD + event assignment + image upload
│   ├── EventsController.cs       # Event CRUD + cascading delete
│   └── HomeController.cs         # Home, About, Contact pages
│
├── Models/
│   ├── Client.cs                 # Client entity (EF generated)
│   ├── Event.cs                  # Event entity (EF generated)
│   ├── EventService.cs           # Junction table entity (Client <-> Event)
│   ├── DbModel.Context.cs        # EF DbContext — EventMangementDbContex
│   └── DbModel.edmx              # EDMX diagram (Database First)
│
├── Views/
│   ├── Clients/
│   │   ├── Index.cshtml          # Client directory with event bookings
│   │   ├── Create.cshtml         # Add new client + event selection
│   │   ├── Edit.cshtml           # Edit client details
│   │   ├── Delete.cshtml         # Delete confirmation
│   │   └── _addNewEvent.cshtml   # Partial — event dropdown selector
│   └── Events/
│       ├── Index.cshtml
│       ├── Create.cshtml
│       ├── Edit.cshtml
│       └── Delete.cshtml
│
└── Images/                       # Uploaded client profile pictures
```

---

## Database Schema

**Client**

| Column | Type | Notes |
|---|---|---|
| `ClientId` | int (PK) | Auto-increment |
| `ClientName` | string | Required |
| `BirthDate` | date | Required, formatted `yyyy-MM-dd` |
| `Age` | int | |
| `Picture` | string | File path to uploaded image |
| `MaritalStatus` | bool | |

**Event**

| Column | Type | Notes |
|---|---|---|
| `EventId` | int (PK) | Auto-increment |
| `EventName` | string | Required |

**EventService** *(junction table)*

| Column | Type | Notes |
|---|---|---|
| `EventServicesId` | int (PK) | |
| `ClientId` | int (FK → Client) | |
| `EventId` | int (FK → Event) | |
| `ServiceName` | string | Required |
| `VendorName` | string | Required |
| `Cost` | decimal? | Nullable |

**Relationships**

```
Client ──< EventService >── Event
  (one-to-many)        (one-to-many)
```

One client can be enrolled in many events; one event can have many clients — all linked through the `EventService` junction table.

---

## Controllers & Routes

### `ClientsController`

| Method | Route | Auth | Description |
|---|---|---|---|
| GET | `/Clients` | Public | List all clients with booked events |
| GET | `/Clients/Create` | Required | Show create form |
| POST | `/Clients/Create` | Required | Save new client + event assignments + image |
| GET | `/Clients/Edit/{id}` | Required | Load edit form pre-filled |
| POST | `/Clients/Edit/{id}` | Required | Update client, re-assign events |
| GET | `/Clients/Delete/{id}` | Required | Delete confirmation view |
| POST | `/Clients/Delete/{id}` | Required | Remove EventService records then Client |
| GET | `/Clients/AddNewEvent` | Required | Returns `_addNewEvent` partial view |

### `EventsController`

| Method | Route | Auth | Description |
|---|---|---|---|
| GET | `/Events` | Public | List all events |
| GET | `/Events/Create` | Required | Show create form |
| POST | `/Events/Create` | Required | Save new event |
| GET | `/Events/Edit/{id}` | Required | Show edit form |
| POST | `/Events/Edit/{id}` | Required | Update event name |
| GET | `/Events/Delete/{id}` | Required | Delete confirmation |
| POST | `/Events/Delete/{id}` | Required | Cascade-delete EventServices then Event |

---

## Authentication & Authorization

The app uses **ASP.NET Identity 2** with OWIN middleware.

- All controllers carry `[Authorize]` at the class level
- `Index` actions on Clients and Events are decorated with `[AllowAnonymous]` for public browsing
- `AccountController` handles login, registration, and external OAuth logins
- `ManageController` handles password change, phone verification, and two-factor authentication

```csharp
[Authorize]
public class ClientsController : Controller
{
    [AllowAnonymous]
    public ActionResult Index() { ... }  // publicly accessible

    public ActionResult Create() { ... } // login required
}
```

---

## Getting Started

### Prerequisites

- Visual Studio 2019+
- .NET Framework 4.x
- Microsoft SQL Server (LocalDB or full instance)

### Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/your-username/EventManagementSystem.git
   cd EventManagementSystem
   ```

2. **Restore NuGet packages**
   - Open `Event_Management_System.sln` in Visual Studio
   - Right-click solution → *Restore NuGet Packages*

3. **Configure the connection string**
   In `Web.config`, update to point at your SQL Server:
   ```xml
   <connectionStrings>
     <add name="EventMangementDbContex"
          connectionString="Data Source=.;Initial Catalog=EventManagementDb;Integrated Security=True"
          providerName="System.Data.SqlClient" />
   </connectionStrings>
   ```

4. **Create the database**
   Use the EDMX designer to generate the schema (*Update Database from Model*), or run the generated SQL script against your server.

5. **Create the Images folder** in the project root:
   ```
   Event_Management_System/Images/
   ```

6. **Run the application**
   - Press `F5` or `Ctrl + F5`
   - Register an account to access all management features

---

## Key Implementation Details

### Cascading Delete
Before removing an Event or Client, all linked `EventService` records are deleted first to avoid foreign key constraint violations:
```csharp
var relatedOrders = db.EventServices.Where(x => x.EventId == id).ToList();
if (relatedOrders.Any())
    db.EventServices.RemoveRange(relatedOrders);
db.Events.Remove(@event);
db.SaveChanges();
```

### Profile Picture Upload
Client photos are stored in `/Images/` with a `DateTime.Ticks`-based filename to prevent naming collisions:
```csharp
string fileName = Path.Combine("/Images/",
    DateTime.Now.Ticks.ToString() + Path.GetExtension(file.FileName));
file.SaveAs(Server.MapPath(fileName));
client.Picture = fileName;
```

### Dynamic Event Assignment
The `_addNewEvent` partial view renders a `SelectList` of all events via `ViewBag`. On the Create and Edit forms, multiple events can be selected and submitted as `int[] eventId`. The controller loops through these IDs to create the corresponding `EventService` records.

### View Model Pattern
`ClientVM` separates form concerns from the EF entity — it includes `HttpPostedFileBase PictureFile` for file upload and `List<int> EventList` for pre-populating the event selection on the Edit form, without polluting the entity model.

---

*Built with ASP.NET MVC 5 · Entity Framework 6 · ASP.NET Identity 2*
