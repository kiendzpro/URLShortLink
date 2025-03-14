# URL Shortener

A URL shortening application built with Vue.js frontend and ASP.NET Core backend.

## Features

- Generate short URLs from long URLs
- Track click statistics
- View recent shortened URLs
- Set expiration for URLs

## Technologies

### Frontend
- Vue.js 3
- Vue Router
- Bootstrap 5
- Axios for API communication

### Backend
- ASP.NET Core 7.0
- Entity Framework Core
- SQLite database

## Project Structure

```
├── Frontend/                     # Vue.js frontend
│   ├── src/                      # Source files
│   │   ├── components/           # Vue components
│   │   ├── services/             # API services
│   │   └── views/                # Vue views/pages
│   ├── index.html                # Main HTML file
│   └── vite.config.js            # Vite configuration
│
└── Backend/                      # ASP.NET Core backend
    ├── URLShortener.API/         # API project
    ├── URLShortener.Core/        # Core logic
    └── URLShortener.Infrastructure/  # Data access
```

## Setup & Running

### Backend

1. Navigate to the Backend directory:
   ```
   cd Backend
   ```

2. Restore NuGet packages:
   ```
   dotnet restore
   ```

3. Run the application:
   ```
   cd URLShortener.API
   dotnet run
   ```

The API will be available at https://localhost:5001.

### Frontend

1. Navigate to the Frontend directory:
   ```
   cd Frontend
   ```

2. Install dependencies:
   ```
   npm install
   ```

3. Run the development server:
   ```
   npm run dev
   ```

The frontend will be accessible at http://localhost:3000.

## API Endpoints

- `POST /api/urls`: Create a shortened URL
- `GET /api/urls/{code}`: Get original URL by code
- `GET /api/urls/{code}/stats`: Get statistics for a shortened URL
- `GET /api/urls/recent`: Get recent shortened URLs 