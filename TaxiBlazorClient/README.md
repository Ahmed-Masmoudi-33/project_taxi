# Taxi Management System - Blazor WebAssembly Frontend

This is the Blazor WebAssembly frontend for the Taxi Management System.

## Features

- **User Registration**: Full-page registration form for new users
- **User Login**: Login page with authentication
- **Taxi Management** (Boss role): Create, edit, and manage taxis
- **Rides Management** (Employee role): Add and view rides for taxis
- **Commission Management**: Adjustable commission percentage per taxi (Boss)
- **Reports**: Monthly reports showing revenue, expenses, commission, and net profit (Boss)
- **Expenses**: Add and manage expenses for taxis (Boss)

## Yellow Theme

The application uses a beautiful yellow/gold theme throughout with:
- Primary color: #FFD700 (Gold)
- Dark yellow: #FFA500 (Orange)
- Light yellow: #FFF8DC (Cornsilk)
- Accent yellow: #FFC107 (Amber)

## Configuration

Update the API base address in `appsettings.json`:

```json
{
  "ApiBaseAddress": "https://localhost:7000/"
}
```

Make sure this matches your backend API URL.

## Running the Application

1. Ensure your backend API is running
2. Update the `ApiBaseAddress` in `appsettings.json` if needed
3. Run the Blazor WebAssembly app:

```bash
cd TaxiBlazorClient
dotnet run
```

4. Open your browser and navigate to the URL shown in the console (typically `https://localhost:5001`)

## Pages

- `/` - Home (redirects based on authentication status)
- `/register` - User registration
- `/login` - User login
- `/taxis` - Taxi management (Boss only)
- `/rides` - Rides management (Employee)
- `/reports` - Monthly reports (Boss)
- `/expenses` - Expenses management (Boss)

## Authentication

The application uses JWT tokens stored in browser localStorage. After successful login, the token is automatically included in API requests.

## User Roles

- **BOSS**: Can manage taxis, view reports, manage expenses, and set commission
- **EMPLOYEE**: Can add rides and view their rides

