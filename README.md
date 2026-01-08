# NetPostman - Web-Based API Testing Tool

A powerful, self-hosted API testing tool built with .NET Core MVC, inspired by Postman. NetPostman enables developers to build, test, and document APIs directly from a web browser, with full support for collections, environments, request history, and more.

## Overview

NetPostman provides a complete API development environment that runs entirely in your browser while leveraging a robust .NET Core backend for actual HTTP request execution. This architecture eliminates CORS issues commonly faced by browser-based API tools and provides enterprise-grade reliability and security for internal API testing workflows.

The application is designed with a modern, dark-themed interface that maximizes screen real estate for API development tasks. The three-panel layout mirrors popular tools like Postman and VS Code, ensuring a familiar experience for developers transitioning from desktop applications. All requests are executed server-side through a dedicated proxy service, allowing seamless communication with APIs across different domains without browser security restrictions.

## Project Structure

NetPostman/
├── NetPostman.sln                          # Solution file
├── README.md                               # Documentation
└── src/
    ├── NetPostman.Core/                    # Core layer
    │   ├── Entities/                       # Domain entities
    │   │   ├── Workspace.cs               # Workspace for organizing collections
    │   │   ├── Collection.cs              # Collection of requests
    │   │   ├── Request.cs                 # HTTP request configuration
    │   │   ├── Environment.cs             # Environment with variables
    │   │   └── RequestHistory.cs          # History of executed requests
    │   └── Interfaces/                    # Abstractions
    │       ├── IHttpRequestService.cs     # HTTP request execution interface
    │       ├── ICollectionRepository.cs   # Collection operations
    │       ├── IWorkspaceRepository.cs    # Workspace operations
    │       ├── IEnvironmentRepository.cs  # Environment operations
    │       └── IRequestHistoryRepository.cs # History operations
    │
    ├── NetPostman.Infrastructure/          # Infrastructure layer
    │   ├── Data/
    │   │   ├── NetPostmanDbContext.cs     # EF Core database context
    │   │   ├── NetPostmanDbContextFactory.cs # Design-time factory
    │   │   └── DatabaseInitializer.cs     # Database seeding
    │   ├── Repositories/                   # Repository implementations
    │   │   ├── CollectionRepository.cs
    │   │   ├── WorkspaceRepository.cs
    │   │   ├── EnvironmentRepository.cs
    │   │   └── RequestHistoryRepository.cs
    │   └── Services/
    │       └── HttpRequestService.cs      # HTTP request execution
    │
    └── NetPostman.Web/                     # Web layer (MVC)
        ├── Controllers/
        │   ├── HomeController.cs          # Main controller for requests
        │   ├── CollectionController.cs    # Collection management
        │   ├── EnvironmentController.cs   # Environment management
        │   └── HistoryController.cs       # History management
        ├── ViewModels/                     # View models
        ├── Views/                          # Razor views
        │   ├── Home/Index.cshtml          # Main application view
        │   └── Shared/_ErrorLayout.cshtml
        ├── Services/
        │   └── DependencyInjection.cs     # Service registration
        ├── wwwroot/
        │   ├── css/style.css              # Dark theme styling
        │   ├── js/app.js                  # Frontend JavaScript
        │   └── lib/codemirror/            # CodeMirror for syntax highlighting
        ├── appsettings.json               # Configuration
        ├── web.config                     # IIS configuration
        ├── Program.cs                     # Application entry point
        └── Startup.cs                     # Application configuration

## Key Features

### Request Builder

The request builder supports all standard HTTP methods including GET, POST, PUT, PATCH, DELETE, HEAD, and OPTIONS. Each request can be configured with query parameters, custom headers, authentication credentials, and request bodies in multiple formats. The interface provides immediate visual feedback with color-coded method badges, making it easy to distinguish between different request types at a glance.

Query parameters and headers are managed through an intuitive key-value editor that supports enabling and disabling individual entries. This feature is particularly useful when testing different header configurations or debugging API authentication issues. The authentication system supports Bearer token authentication for modern APIs and Basic authentication for legacy systems, with credentials securely handled through the server-side proxy.

Request body support includes form-data for multipart uploads, URL-encoded form data for standard form submissions, and raw body content with syntax highlighting for JSON, XML, HTML, and plain text. The CodeMirror editor provides a rich coding experience with line numbers, syntax highlighting, and automatic indentation, making it comfortable to work with complex request payloads.

### Collection Management

Collections provide a powerful organization system for grouping related API requests together. Each collection can contain multiple requests and nested folders, enabling hierarchical organization of complex API projects. Collections support descriptions for documentation purposes and can be exported for sharing with team members or importing into other API testing tools.

The collection browser in the sidebar displays all collections with expandable folders showing their contents. Requests within collections display their HTTP method and name, with the method color-coded for quick identification. Collections can be created, edited, and deleted through the user interface, with all changes persisted to the SQLite database for long-term storage.

Requests saved to collections retain all configuration including headers, authentication, body content, pre-request scripts, and test scripts. This comprehensive storage ensures that complete request configurations can be restored instantly, supporting iterative API development workflows where requests evolve over time.

### Environment Variables

Environment management enables dynamic configuration of API endpoints and credentials across different deployment stages. Variables are defined as key-value pairs and can be referenced in requests using the double-brace syntax, such as `{{baseUrl}}` or `{{apiKey}}`. This approach eliminates hardcoded values from requests and enables seamless switching between development, staging, and production environments.

The system supports a global environment with variables available across all requests, as well as named environments for stage-specific configurations. The environment selector in the sidebar provides quick access to switch between environments, with a clear indicator showing the currently active environment. Variables defined in the active environment take precedence over global variables with the same name.

Variable substitution occurs server-side before requests are executed, ensuring that sensitive values like API keys are never exposed to the browser. This security model is particularly important for enterprise deployments where API credentials must be protected according to organizational policies.

### Request History

Every request executed through the application is automatically recorded in the request history. History entries capture the HTTP method, full URL, response status code, response time, and execution timestamp. This automatic tracking provides an audit trail of API testing activity and enables quick resumption of previous testing sessions.

The history panel displays recent requests in chronological order, with method badges and URL truncation for compact display. Clicking any history entry instantly loads the request configuration, including method, URL, headers, and body, allowing rapid iteration on previous requests. The history can be cleared entirely or filtered to show only recent entries based on configurable limits.

Response data is cached for history entries, enabling inspection of previous responses without re-executing requests. The cached response includes status, timing, headers, and body content, though large responses may be truncated to manage storage requirements. This caching strategy balances functionality with storage efficiency for long-running testing sessions.

### Response Viewer

The response viewer provides comprehensive information about API responses in a structured, easy-to-read format. Status information includes the HTTP status code with color-coded badges indicating success (green) or error (red) states, along with the status text returned by the server. Performance metrics show response time in milliseconds and response size in human-readable format.

The body panel displays response content with automatic syntax highlighting for JSON responses, making it easy to inspect complex nested structures. JSON responses are automatically formatted with proper indentation for readability. Non-JSON responses display as plain text with line wrapping enabled. A copy button enables quick copying of response content to the clipboard for use in other applications.

Additional tabs display response cookies and headers in organized formats. Cookies show name, value, domain, and other attributes, while headers display all response headers with their values. This comprehensive response information supports detailed API debugging and documentation workflows.

### Test Script Support

NetPostman includes a test scripting environment for validating API responses. Tests are written in JavaScript and executed in the browser after responses are received. The test framework provides a simple API for common assertions including status code verification, response time thresholds, and response body validation.

Tests can check for expected status codes, verify response headers contain specific values, or validate JSON response structures against expected schemas. The test results panel displays pass/fail status for each test with detailed error messages for failures. This testing capability enables automated API validation as part of continuous integration workflows.

Pre-request scripts extend functionality by allowing JavaScript execution before requests are sent. These scripts can set environment variables based on dynamic values, generate timestamps or unique identifiers, or perform calculations needed for request construction. The combination of pre-request scripts and response tests supports complex API testing scenarios.

### IIS Deployment

The application is specifically designed and configured for deployment on Internet Information Services (IIS), Microsoft's web server platform. The deployment package includes a properly configured web.config file with the AspNetCoreModuleV2 handler, enabling seamless integration with IIS infrastructure. The configuration supports both in-process and out-of-process hosting models, with in-process selected by default for optimal performance.

Database persistence uses SQLite, a lightweight, file-based database that requires no additional database server installation. The database file is created automatically on first run, and the application requires only file system write permissions to the application directory. This simplicity makes NetPostman ideal for environments where database server installation is restricted or impractical.

The deployment configuration includes appropriate security settings, request filtering for large uploads, and HTTP compression for improved performance. The application pool can run under the built-in ApplicationPoolIdentity with minimal permissions, adhering to the principle of least privilege for enhanced security.

## Prerequisites

### Development Environment

The following software is required for building and running NetPostman locally:

- .NET 8.0 SDK or later, available from Microsoft's official download page, is essential for compiling and running the application. The SDK includes the runtime, compilers, and development tools needed for .NET development.
- Visual Studio 2022 or later is recommended for the development experience, though any text editor can be used with the .NET CLI. VS Code provides excellent .NET development support through extensions.
- A modern web browser such as Chrome, Firefox, or Edge is required for the web interface. The application is tested with recent browser versions to ensure compatibility with all features.

### Production Environment

For IIS deployment, the following requirements apply:

- Windows Server 2019 or later, or Windows 10/11 with IIS enabled, provides the required platform. Server editions are recommended for production use with appropriate licensing.
- Internet Information Services (IIS) version 10.0 or later must be installed with the ASP.NET Core Module feature. The module is included with .NET Core Hosting Bundle installation.
- .NET Core 8.0 Hosting Bundle must be installed on the server. This component enables IIS to host .NET Core applications and is available as a free download from Microsoft.

## Installation and Setup

### Local Development Setup

Begin by cloning or downloading the NetPostman source code to your development machine. Navigate to the solution directory and restore all NuGet packages by executing the dotnet restore command. This operation downloads all required dependencies and prepares the solution for building.

After package restoration, build the solution using dotnet build to verify compilation succeeds without errors. The build process produces all project assemblies and validates the codebase structure. Any compilation errors should be resolved before proceeding with execution.

To run the application locally, execute dotnet run from the solution directory. The command starts the development server on a local port, typically http://localhost:5000. The browser opens automatically to the application interface where you can begin creating and executing API requests immediately.

For development with database migrations, ensure the NetPostman.Infrastructure project is set as the startup project when using Visual Studio. The DatabaseInitializer class creates the default workspace and global environment on first run, providing a ready-to-use configuration.

### Database Configuration

NetPostman uses SQLite for data persistence, with the database file created automatically in the application directory. The default connection string in appsettings.json specifies Data Source=netpostman.db, creating the database in the same directory as the application binaries.

For development, the database file is created in the build output directory. For IIS deployments, the database is created in the application root directory. Ensure the application pool identity has write permissions to this directory, or configure an alternative location with appropriate permissions.

To use an alternative database location or SQL Server, modify the ConnectionStrings:DefaultConnection setting in appsettings.json. Entity Framework Core supports multiple database providers including SQL Server, PostgreSQL, and MySQL with appropriate NuGet packages.

## Building for Production

### Publishing the Application

To prepare NetPostman for deployment, publish the application using the Release configuration with the dotnet publish command. The publish operation compiles the application in release mode, optimizes dependencies, and produces deployment-ready files in the specified output directory.

The recommended publish command is dotnet publish -c Release -o ./publish, which creates a self-contained deployment package in the publish directory. This package includes all required .NET runtime components and can be deployed to any server with the appropriate hosting bundle installed.

The publish output includes the application DLLs, configuration files, static assets, and the web.config file required for IIS hosting. No additional installation steps are required on the target server beyond copying these files and configuring IIS.

### IIS Deployment Steps

Deploying NetPostman to IIS involves creating the appropriate site configuration and ensuring proper permissions. Begin by copying the published files to the target server, placing them in a dedicated directory such as C:\inetpub\netpostman or your preferred location.

Create a new Application Pool in IIS Manager with the following settings: .NET CLR version set to "No Managed Code", managed pipeline mode set to "Integrated", and the identity configured as ApplicationPoolIdentity or a specific service account as required by your security policies.

Create a new Website or Virtual Directory in IIS pointing to the NetPostman files directory. Configure the binding with your server's IP address or hostname and the desired port (typically 80 for HTTP or 443 for HTTPS). If using HTTPS, ensure a valid certificate is bound to the site.

Configure the application pool identity to have modify permissions on the NetPostman directory, enabling database file creation and updates. For enhanced security, restrict these permissions to only the database file if preferred, though the application requires writes only during initialization and history recording.

Browse to the configured URL to verify the application starts correctly. The home page should load with the request builder interface ready for use. If errors occur, check the stdout log file configured in web.config for detailed error information.

## Configuration Reference

### Application Settings

The appsettings.json file contains all configurable application settings. The following options control application behavior:

The ConnectionStrings:DefaultConnection setting specifies the database connection. For SQLite, use Data Source=path/to/database.db. For SQL Server, use a connection string such as Server=myServer;Database=NetPostman;Trusted_Connection=True;.

The HttpClient:TimeoutSeconds setting controls the maximum time in seconds that requests wait for responses. The default value of 60 seconds suits most API interactions, but may require adjustment for long-running operations.

The HttpClient:MaxResponseSizeBytes setting limits the response body size accepted by the proxy service. The default of 52428800 bytes (50 MB) prevents memory issues with large responses but may be increased for specific use cases.

### Web.config Settings

The web.config file configures IIS integration. The maxAllowedContentLength attribute controls maximum upload sizes in bytes, currently set to 104857600 (100 MB) to support file uploads and large request bodies.

The aspNetCore element configures the hosting model, currently set to inprocess for optimal performance. The stdoutLogFile path can be adjusted for custom logging locations.

## Project Architecture

### Solution Structure

The application follows Clean Architecture principles with three primary layers:

The Core layer (NetPostman.Core) contains domain entities representing the business model, including Workspace, Collection, Request, Environment, and RequestHistory. This layer also defines repository and service interfaces that abstract data access and HTTP operations. Dependencies flow inward, ensuring the core remains independent of infrastructure concerns.

The Infrastructure layer (NetPostman.Infrastructure) implements the interfaces defined in the core layer. The Entity Framework Core DbContext manages database operations, while repository implementations provide data access functionality. The HttpRequestService handles actual HTTP communication with target APIs.

The Web layer (NetPostman.Web) contains the MVC application including controllers, views, and static assets. This layer depends on both core and infrastructure, composing the application's user interface and HTTP endpoints.

### Key Components

The HomeController serves as the main application controller, handling the initial page load and request execution. The ExecuteRequest action receives request configurations from the browser, executes requests through the HttpRequestService, and returns results for display.

The CollectionController manages CRUD operations for collections and requests. Actions support creating collections, saving requests to collections, loading saved requests, and deleting requests or collections. All operations persist to the SQLite database.

The EnvironmentController handles environment management including creating, updating, and deleting environments. Variables are stored as JSON in the database and resolved during request execution.

The HistoryController provides access to request history through the GetHistory, GetHistoryEntry, and ClearHistory actions. History entries are automatically created when requests are executed.

### Data Model

The workspace entity serves as the top-level organizational container, with collections and environments as children. Each workspace contains multiple collections organized in a flat list, with collections supporting parent references for folder hierarchies.

Requests belong to collections and store complete configuration including method, URL, headers, body, and scripts. Headers and other structured data are serialized to JSON for storage, enabling flexible schema evolution.

Request history entries reference the parent workspace and optionally the original request. History captures response information for later review, supporting debugging and documentation workflows.

## API Endpoints

### Home Controller

The POST /Home/ExecuteRequest endpoint executes HTTP requests with the provided configuration. Request bodies include method, URL, headers, body content, body type, and optional request identifiers. Responses return status code, status text, timing, headers, and body content.

### Collection Controller

The GET /Collection/GetCollections endpoint returns all collections for the default workspace with their requests. The POST /Collection/Create action creates new collections with the provided name and description. The POST /Collection/SaveRequest action creates or updates requests within collections.

The GET /Collection/GetRequest action loads a saved request by ID for editing or execution. The POST /Collection/Delete action removes collections or requests by ID.

### Environment Controller

The GET /Environment/GetEnvironments endpoint returns all environments including the global environment. The POST /Environment/Create action creates new environments with name and variable definitions. The POST /Environment/Update action modifies existing environment variables.

### History Controller

The GET /History/GetHistory endpoint returns recent history entries with configurable limits. The GET /History/GetHistoryEntry action returns detailed information for a specific history entry. The POST /History/ClearHistory action removes all history entries for the workspace.

## Technologies Used

The backend is built on ASP.NET Core 8.0, providing the runtime, framework, and libraries for the web application. Entity Framework Core 8.0 handles object-relational mapping and database operations. SQLite serves as the lightweight, embedded database requiring no separate server process.

The frontend uses HTML5, CSS3, and modern JavaScript for a responsive, single-page application experience. CodeMirror 5 provides the code editing capability with syntax highlighting for multiple languages. The dark theme is custom-designed for comfortable extended use.

IIS with the ASP.NET Core Module hosts the application in production environments. The hosting model runs .NET Core in-process with IIS for optimal performance and simplified deployment.

## Security Considerations

All HTTP requests execute server-side, preventing CORS issues and keeping sensitive credentials away from browser access. Authentication credentials entered in the UI are transmitted only to the server and never stored in browser storage or cookies.

The application should be deployed over HTTPS in production environments to protect request data in transit. Configure TLS certificates through IIS bindings for the deployed website.

Environment variables containing sensitive values like API keys should be protected by restricting database file access. The SQLite database stores variables in plain text, so file system permissions are critical for security.

## License

This project is licensed under the MIT License, a permissive open-source license that allows free use, modification, and distribution for any purpose, including commercial use.

## Contributing

Contributions are welcome through pull requests for bug fixes, feature additions, or documentation improvements. Please ensure code changes follow existing patterns and include appropriate tests where applicable.

## Support

For issues or questions, review the existing documentation or open a GitHub issue with detailed information about the problem and steps to reproduce.

## Future Enhancements

Potential future features include workspace collaboration for team use, Postman collection import/export in version 2.1 format, GraphQL request support, automated test collections, and response comparison capabilities.
