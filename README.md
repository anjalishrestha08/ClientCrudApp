# ClientCrudApp

## Overview
ClientCrudApp is a full-featured **ASP.NET Core 8 MVC application** that demonstrates modern web application development practices.  
It includes CRUD operations, repository pattern, automated testing, CI/CD integration, code quality analysis, logging, and Docker containerization.


## Features
- **CRUD Operations:** Create, Read, Update, Delete functionality for Client entities.  
- **Repository Pattern:** Clean separation of concerns for data access.  
- **Unit Testing:** Comprehensive tests for controllers and repositories using xUnit and Moq.  
- **Continuous Integration:** Automated builds, tests, and code coverage via GitHub Actions.  
- **Code Quality:** SonarCloud integration for static code analysis and quality gate checks.  
- **Logging:** Structured logging in controllers and repositories.  
- **Docker Support:** Ready-to-use Dockerfile for containerization and deployment.


## Project Structure
ClientCrudApp/
│
├─ ClientCrudApp/            # Main application
│   ├─ Controllers/
│   ├─ Models/
│   ├─ Views/
│   ├─ Repositories/
│   ├─ wwwroot/
│   └─ ClientCrudApp.csproj
│
├─ ClientCrudApp.Tests/      # Unit tests
├─ CoverageReport/           # Test coverage reports
├─ .github/workflows/        # GitHub Actions CI workflow
├─ Dockerfile                # Docker container definition
└─ README.md                 # Project documentation


## Prerequisites
- .NET 8 SDK installed
- Docker Desktop installed (for containerization)
- Git for version control


## Getting Started

### 1. Clone the repository
git clone https://github.com/anjalishrestha08/ClientCrudApp.git
cd ClientCrudApp

### 2. Build and run locally
dotnet restore
dotnet build
dotnet run

The application will start on `http://localhost:5000`.

### 3. Run Unit Tests
dotnet test --collect:"Xplat Code Coverage"

### 4. Docker Containerization
Build Docker image:
docker build -t clientcrudapp .

Run Docker container:
docker run -d -p 5000:5000 clientcrudapp

Open in browser: `http://localhost:5000`


## Continuous Integration (CI)
- **GitHub Actions workflow:** `.github/workflows/ci.yml`  
- Runs automatically on `push` or `pull request` to `master` or `Feature` branch.  
- Steps included:
  - Restore dependencies
  - Build project
  - Run tests
  - Generate code coverage report
  - Analyze code quality using SonarCloud


## SonarCloud Integration
- Monitors code quality
- Follows **Sonar Way** quality profile
- Tracks new issues, duplicated lines, reliability, and security hotspots


## Logging
- Logging implemented in Controllers and Repositories
- Unit tests include `Mock<ILogger>` to verify logs
