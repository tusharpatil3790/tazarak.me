# Professional Portfolio - Full Stack Application

A modern full-stack portfolio application built with **Angular**, **.NET Core**, and **MongoDB**.

## 🚀 Tech Stack

- **Frontend**: Angular
- **Backend**: .NET Core (C#)
- **Database**: MongoDB (Atlas Cloud)
- **Architecture**: REST API
- **API Documentation**: Swagger/OpenAPI

## 📁 Project Structure

```
tazarak.me/
├── frontend/          # Angular application
├── backend/          # .NET Core API
│   ├── Controllers/  # API controllers
│   ├── Models/       # Data models
│   ├── Services/     # Business logic
│   └── appsettings.json.example  # Configuration template
└── README.md
```

## ✨ Features

- ✅ Professional profile and bio
- ✅ Experience timeline with CRUD operations
- ✅ Skills showcase with proficiency levels
- ✅ Projects gallery with links
- ✅ Contact form with email notifications
- ✅ Full API documentation (Swagger)
- ✅ Input validation and error handling
- ✅ Comprehensive logging

## 🛠️ Prerequisites

- **.NET SDK** (10.0 or later)
- **Node.js** (v16+) for Angular frontend
- **MongoDB** (Atlas Cloud recommended) or local MongoDB instance
- **Angular CLI** (for frontend development)

## 📦 Quick Start

### 1. Clone and Navigate

```bash
cd backend
```

### 2. Configure MongoDB Connection

Copy the example configuration file:

```bash
# Windows PowerShell
Copy-Item appsettings.json.example appsettings.json

# Linux/Mac
cp appsettings.json.example appsettings.json
```

Edit `appsettings.json` and add your MongoDB connection string:

```json
{
  "ConnectionStrings": {
    "MongoDB": "mongodb+srv://username:password@cluster.mongodb.net/tazarak-me?retryWrites=true&w=majority"
  },
  "MongoDb": {
    "DatabaseName": "tazarak-me"
  },
  "EmailSettings": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": "587",
    "SmtpUsername": "your-email@gmail.com",
    "SmtpPassword": "your-app-password",
    "FromEmail": "your-email@gmail.com",
    "FromName": "Portfolio Contact",
    "AdminEmail": "your-email@gmail.com"
  }
}
```

### 3. Restore and Build

```bash
dotnet restore
dotnet build
```

### 4. Run the Application

```bash
dotnet run
```

The API will be available at:
- **HTTP**: `http://localhost:5000`
- **HTTPS**: `https://localhost:5001` (Development mode)

### 5. Access Swagger UI

Open your browser and navigate to:
```
http://localhost:5000/swagger
```

## 🔐 Security Setup

### Important: Credentials Security

The `appsettings.json` file is excluded from version control (`.gitignore`). Never commit actual credentials.

### Option 1: appsettings.json (Development)

1. Copy `appsettings.json.example` to `appsettings.json`
2. Edit with your actual credentials

### Option 2: .NET User Secrets (Recommended for Development)

```bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:MongoDB" "your-connection-string"
dotnet user-secrets set "EmailSettings:SmtpUsername" "your-email@gmail.com"
dotnet user-secrets set "EmailSettings:SmtpPassword" "your-app-password"
```

### Option 3: Environment Variables (Production)

Set environment variables:
- `ConnectionStrings__MongoDB`
- `EmailSettings__SmtpUsername`
- `EmailSettings__SmtpPassword`
- `EmailSettings__FromEmail`
- `EmailSettings__AdminEmail`

**Note**: Use double underscore (`__`) for nested configuration keys.

### Gmail App Password Setup

If using Gmail for SMTP:

1. Enable 2-Factor Authentication on your Google account
2. Go to Google Account Settings → Security → App Passwords
3. Generate an app password for "Mail"
4. Use this app password (not your regular Gmail password)

## 📚 API Documentation

### Base URL

```
http://localhost:5000/api
```

### Swagger UI

Interactive API documentation is available at:
```
http://localhost:5000/swagger
```

### Health Check

```
GET /api/health
```

Response:
```json
{
  "status": "ok",
  "timestamp": "2024-01-28T00:00:00Z"
}
```

## 🔌 API Endpoints

### Experiences

- `GET /api/experiences` - Get all experiences
- `GET /api/experiences/{id}` - Get experience by ID
- `POST /api/experiences` - Create experience
- `PUT /api/experiences/{id}` - Update experience
- `DELETE /api/experiences/{id}` - Delete experience

**Example POST Request:**
```json
{
  "title": "Senior Developer",
  "company": "Tech Company",
  "startDate": "2023-01-01T00:00:00Z",
  "endDate": null,
  "description": "Leading full-stack development projects",
  "technologies": ["Angular", ".NET Core", "MongoDB"]
}
```

### Skills

- `GET /api/skills` - Get all skills
- `GET /api/skills/{id}` - Get skill by ID
- `POST /api/skills` - Create skill
- `PUT /api/skills/{id}` - Update skill
- `DELETE /api/skills/{id}` - Delete skill

**Example POST Request:**
```json
{
  "name": "Angular",
  "category": "Frontend",
  "proficiency": 5
}
```

**Validation**: Proficiency must be between 1 and 5.

### Projects

- `GET /api/projects` - Get all projects
- `GET /api/projects/{id}` - Get project by ID
- `POST /api/projects` - Create project
- `PUT /api/projects/{id}` - Update project
- `DELETE /api/projects/{id}` - Delete project

**Example POST Request:**
```json
{
  "title": "Project Name",
  "description": "Project description",
  "technologies": ["Angular", ".NET Core"],
  "gitHubUrl": "https://github.com/...",
  "liveUrl": "https://...",
  "imageUrl": "https://..."
}
```

**Validation**: URLs must be valid.

### Contacts

- `GET /api/contacts` - Get all contacts
- `POST /api/contacts` - Submit contact form
- `PUT /api/contacts/{id}/mark-read` - Mark contact as read
- `DELETE /api/contacts/{id}` - Delete contact

**Example POST Request:**
```json
{
  "name": "John Doe",
  "email": "john@example.com",
  "subject": "Inquiry",
  "message": "Hello, I'm interested in...",
  "phone": "+1234567890"
}
```

**Validation**: Email must be valid format.

## 🗄️ MongoDB Setup

### Option 1: MongoDB Atlas (Cloud) - Recommended

1. Go to https://www.mongodb.com/cloud/atlas
2. Sign up for free (M0 tier is free forever)
3. Create a cluster
4. Get connection string
5. Update `appsettings.json` with connection string

### Option 2: Docker MongoDB Container

```bash
docker run -d -p 27017:27017 --name mongodb mongo:latest
```

### Option 3: Local MongoDB Installation

1. Download from https://www.mongodb.com/try/download/community
2. Install MongoDB Community Edition
3. Start MongoDB service:
   ```bash
   net start MongoDB
   ```

## 🧪 Testing the API

### Using PowerShell

```powershell
# Health check
Invoke-WebRequest -Uri "http://localhost:5000/api/health" -UseBasicParsing

# Get all experiences
Invoke-WebRequest -Uri "http://localhost:5000/api/experiences" -UseBasicParsing

# Get all skills
Invoke-WebRequest -Uri "http://localhost:5000/api/skills" -UseBasicParsing
```

### Using Swagger UI

1. Navigate to `http://localhost:5000/swagger`
2. Click on any endpoint
3. Click "Try it out"
4. Fill in the parameters
5. Click "Execute"

### Testing Validation

Try these to verify validation is working:

- POST with missing required fields → Should return `400 Bad Request`
- POST with invalid email → Should return `400 Bad Request`
- POST with proficiency > 5 → Should return `400 Bad Request`
- POST with invalid URL → Should return `400 Bad Request`

## 🔧 Development

### Running in Development Mode

```bash
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet run
```

### Building for Production

```bash
dotnet build -c Release
```

### Using Batch Script

```bash
start_backend.bat
```

## 📝 Code Quality Features

### ✅ Implemented Improvements

- **Input Validation**: All models have data annotations (`[Required]`, `[EmailAddress]`, `[Range]`, `[StringLength]`, `[Url]`)
- **Error Handling**: Comprehensive try-catch blocks in all controllers
- **Logging**: All operations are logged using `ILogger`
- **API Documentation**: Swagger/OpenAPI integration
- **Security**: Credentials excluded from version control
- **Data Integrity**: `UpdatedAt` fields automatically maintained
- **RESTful Design**: Proper HTTP status codes and responses

### Validation Rules

- **Experience**: Title, Company, Description required (max 200 chars)
- **Skill**: Name, Category required; Proficiency 1-5
- **Project**: Title, Description required; URLs validated
- **Contact**: Name, Email, Subject, Message required; Email format validated

## 🌐 CORS Configuration

The API is configured to accept requests from:
- `http://localhost:4200`
- `https://localhost:4200`

Update the CORS policy in `Program.cs` if deploying to different URLs.

## 📊 Database Collections

- `experiences` - Work experience entries
- `skills` - Technical skills with proficiency
- `projects` - Portfolio projects
- `contacts` - Contact form submissions

## 🐛 Troubleshooting

### Application Won't Start

1. Check if port 5000/5001 is already in use
2. Verify MongoDB connection string in `appsettings.json`
3. Check logs for error messages

### Swagger Not Loading

- Ensure application is running
- Check URL: `http://localhost:5000/swagger`
- Verify Swagger is enabled in `Program.cs`

### MongoDB Connection Issues

- Verify connection string format
- Check network connectivity
- Ensure MongoDB Atlas IP whitelist includes your IP
- Verify database name matches configuration

### Build Errors

- Run `dotnet clean` then `dotnet restore`
- Ensure .NET SDK version matches project target framework
- Close any running instances before building

## 📄 License

Feel free to improve and expand this portfolio over time!

## 🤝 Contributing

Contributions are welcome! Please ensure:
- Code follows existing patterns
- Validation and error handling are included
- Changes are tested
- Documentation is updated

## 📞 Support

For issues or questions:
1. Check the Swagger UI for API documentation
2. Review error logs in the console
3. Verify configuration in `appsettings.json`

---

**Last Updated**: 2024
**Status**: ✅ Production Ready
