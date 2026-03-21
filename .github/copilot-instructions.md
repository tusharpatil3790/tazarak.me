# Professional Portfolio Project - Copilot Instructions

This is a complete full-stack portfolio application with Angular frontend, .NET Core backend, and MongoDB Atlas database. The project is fully implemented, tested, and deployed.

## Project Context
- **Tech Stack**: Angular (static HTML/JS) + .NET Core 10.0 + MongoDB Atlas
- **Purpose**: Showcase professional career, skills, projects, and experiences
- **Structure**: Separate frontend and backend folders, with comprehensive test suite
- **Status**: Production-ready, committed to GitHub develop branch

## Architecture Overview
- **Backend**: RESTful API with controllers for Experiences, Skills, Projects, Contacts
- **Frontend**: Static HTML with JavaScript for dynamic data fetching and display
- **Database**: MongoDB Atlas with connection string in appsettings.json
- **Testing**: xUnit with Moq and FluentAssertions (57 tests, 95%+ coverage)
- **Deployment**: Configured for HTTPS, CORS enabled for localhost development

## Key Directories
- `frontend/` - Static HTML portfolio website with API integration
- `backend/` - .NET Core API with controllers, services, models
- `tests/backend/PortfolioAPI.Tests/` - Comprehensive unit test suite
- `.github/` - Project configuration and CI/CD setup
- `backend/bin/` - Built executables (ignore in version control)

## Development Guidelines
- Frontend updates: Modify `frontend/index.html` for UI changes, update API calls as needed
- Backend updates: Add/modify controllers in `backend/Controllers/`, services in `backend/Services/`
- Database: Use MongoDB Atlas, ensure IP whitelisting for access
- Testing: Run tests with `dotnet test` in test project directory
- API Documentation: Available at `/swagger` when running backend

## Running the Application
1. Backend: `cd backend && dotnet run` (runs on https://localhost:5001)
2. Frontend: `cd frontend && node serve-frontend.js` (runs on http://localhost:4200)
3. Database: Ensure MongoDB Atlas connection and IP whitelisting
4. Populate data: Run `backend/populate-data.ps1` for sample data

## API Endpoints
- GET/POST/PUT/DELETE `/api/experiences`
- GET/POST/PUT/DELETE `/api/skills`
- GET/POST/PUT/DELETE `/api/projects`
- POST `/api/contacts` (with email notification)

## Testing
- Unit tests: 57 tests covering all controllers and services
- Coverage: 95%+ line and branch coverage
- Run: `dotnet test` in `tests/backend/PortfolioAPI.Tests/`

## Deployment
- GitHub: Pushed to `develop` branch at https://github.com/tusharpatil3790/tazarak.me.git
- Production: Configure environment variables for MongoDB and email settings
- Static files: Frontend served via backend or separate hosting

## Maintenance
- Update resume data: Use PowerShell scripts in `backend/` for data management
- Code cleanup: Run cleanup scripts for build artifacts
- Version control: Commit changes to develop branch, merge to main for releases
