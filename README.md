# Repair Management System - Backend

This is the backend API for a repair management system designed to help service employees register, track, update, and manage repair orders efficiently.

## 🔧 Features

- Register a new repair order with customer and device details
- Track the status of repair orders
- Update or delete repair orders
- Search/filter by customer or device information
- View statistics (total orders, average revenue, total repair costs)
- Role-based data handling using user IDs

## 🧠 Technologies Used

- **.NET Core API**
- **MySQL Database**
- **Stored Procedures** for all CRUD operations
- **Functions** for statistical summaries
- **Triggers** for:
  - Preventing customer deletion if they have active orders
  - Automatically updating `UpdatedAt` timestamp on status changes


## 📌 How to Run

1. Clone the repository
2. Set up your MySQL database and run the SQL scripts for tables, stored procedures, functions, and triggers.
3. Configure your connection string in `appsettings.json`
4. Run the API:
   ```bash
   dotnet run

Make sure all triggers and functions are installed in the database before using the API.

This backend is built for integration with a simple frontend to be used by repair service employees.

🙋 Author
Developed by Husam Abdulraheem
