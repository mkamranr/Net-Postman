# NetPostman - Web-Based API Testing Tool

A powerful, self-hosted API testing tool built with .NET Core MVC, inspired by Postman. NetPostman enables developers to build, test, and document APIs directly from a web browser, with full support for collections, environments, request history, and more.

## Overview

NetPostman provides a complete API development environment that runs entirely in your browser while leveraging a robust .NET Core backend for actual HTTP request execution. This architecture eliminates CORS issues commonly faced by browser-based API tools and provides enterprise-grade reliability and security for internal API testing workflows.

The application is designed with a modern, dark-themed interface that maximizes screen real estate for API development tasks. The three-panel layout mirrors popular tools like Postman and VS Code, ensuring a familiar experience for developers transitioning from desktop applications. All requests are executed server-side through a dedicated proxy service, allowing seamless communication with APIs across different domains without browser security restrictions.

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

## Building and Running the Application

Building and running the NetPostman application involves several steps from obtaining the source code to launching the development server. This section provides detailed instructions for setting up the development environment, building the application, and running it locally for testing and development purposes.

### Obtaining the Source Code

The source code can be obtained by cloning the Git repository or downloading a ZIP archive from the repository hosting service. To clone the repository, ensure Git is installed on your machine and execute the following command in your preferred terminal application:

```bash
git clone https://github.com/yourusername/NetPostman.git
```

Replace the repository URL with the actual URL of the NetPostman repository. After cloning, navigate to the NetPostman directory to access the solution files. Alternatively, if you have received the code as a ZIP archive, extract it to a convenient location and open the extracted folder.

### Restoring Dependencies

Before building the application, all NuGet package dependencies must be restored. The solution uses several third-party packages including Entity Framework Core, Newtonsoft.Json, and SQLite runtime libraries. The dotnet restore command reads the project files, identifies all dependencies, and downloads them from the NuGet gallery.

Execute the following command from the solution directory to restore dependencies:

```bash
dotnet restore
```

This command reads all .csproj files in the solution, collects their dependencies, and downloads the packages to the local NuGet cache. The restore process typically completes in under a minute depending on your internet connection speed. If you encounter any errors during restoration, verify your internet connection and ensure the NuGet feed is accessible.

You can also restore dependencies implicitly by running the build command, which automatically restores packages before compilation. However, running restore explicitly is useful for verifying that all dependencies are available before attempting a build.

### Building the Application

The build process compiles all projects in the solution and produces executable assemblies. The build command performs several operations including restoring packages if needed, compiling C# code, copying content files, and validating the project structure. Any compilation errors or warnings are displayed during this process.

Execute the following command to build the solution:

```bash
dotnet build
```

The build output indicates whether compilation succeeded and displays any errors or warnings. A successful build produces DLL files in the bin/Debug/net8.0 directories of each project. The Web project produces the NetPostman.Web.dll assembly that serves as the application entry point.

To build for release configuration with optimizations, use the following command:

```bash
dotnet build -c Release
```

Release builds enable compiler optimizations that improve runtime performance and reduce assembly size. The Release build output is placed in bin/Release/net8.0 directories. For development purposes, the Debug configuration is typically preferred as it enables debugging features and provides more detailed error information.

### Database Initialization

The application uses SQLite as its database provider, and the database file is created automatically on first run. The DatabaseInitializer class, invoked during application startup, ensures the database schema is created and seeds the default workspace and global environment required for the application to function properly.

No manual database setup is required. The first time the application runs, it creates the netpostman.db file in the application directory and populates it with the initial schema. The database includes tables for workspaces, collections, requests, environments, and request history, along with appropriate indexes and foreign key relationships.

For development scenarios where you need to inspect or modify the database directly, you can use any SQLite-compatible tool such as the sqlite3 command-line utility, DB Browser for SQLite, or Visual Studio's Server Explorer. The database file is located in the output directory alongside the application DLLs.

### Running Locally

The .NET CLI provides a convenient development server for running the application locally. The development server is optimized for iterative development, featuring automatic recompilation when source files change and detailed error pages for troubleshooting.

Execute the following command to run the application:

```bash
dotnet run
```

This command compiles and starts the application, listening on http://localhost:5000 for HTTP requests and https://localhost:5001 for HTTPS requests. The terminal displays the server URL and other startup information. Open your web browser and navigate to the displayed URL to access the application interface.

The development server supports hot reload, automatically detecting changes to source files and recompiling without restarting the application. Changes to C# code trigger a compilation and application restart, while changes to static files such as CSS and JavaScript are reflected immediately without compilation. This workflow enables rapid iteration during development.

To specify a particular URL for the development server, use the --urls flag:

```bash
dotnet run --urls "http://localhost:8080"
```

You can also configure the default URLs in the appsettings.json file or through environment variables. The ASPNETCORE_URLS environment variable overrides the default ports when set.

### Running with Visual Studio

Visual Studio provides an integrated development experience with debugging support, project management, and productivity features. Open the NetPostman.sln solution file to load all projects into the Visual Studio workspace.

To run the application from Visual Studio, select the NetPostman.Web project as the startup project in the Solution Explorer. Press F5 or click the Run button in the toolbar to start the application with debugging. Visual Studio launches the development server and opens your default browser to the application URL.

The debugger enables setting breakpoints, inspecting variables, and stepping through code during execution. This debugging capability is invaluable for troubleshooting issues and understanding application behavior. Debug builds include full symbol information and disable optimizations to support the debugging experience.

Visual Studio also provides the Package Manager Console for executing Entity Framework Core commands such as migrations and database updates. To open the Package Manager Console, navigate to Tools > NuGet Package Manager > Package Manager Console. From here, you can run commands like Add-Migration to create database migrations or Update-Database to apply pending migrations.

### Development Workflow

A typical development workflow involves editing source code, observing changes in the browser, and iterating until the desired functionality is achieved. The following practices enhance productivity when working with the NetPostman codebase:

When adding new features, start by defining the required interfaces in the Core layer. This approach ensures the core business logic remains independent of implementation details. Implement the interfaces in the Infrastructure layer, then create or update controllers and views in the Web layer as needed.

For database changes, modify the entity classes in the Core layer and update the DbContext configuration in the Infrastructure layer. Create Entity Framework Core migrations to capture schema changes in a version-controlled format. Apply migrations during development and include them in deployment scripts.

Use the existing code patterns as templates when adding new functionality. The repository pattern, dependency injection, and controller conventions used throughout the codebase provide consistency and maintainability. Following established patterns makes the codebase easier to navigate and reduces the learning curve for new contributors.

## Configuration Reference

### Application Settings

The appsettings.json file contains all configurable application settings. The following options control application behavior:

The ConnectionStrings:DefaultConnection setting specifies the database connection. For SQLite, use Data Source=path/to/database.db. For SQL Server, use a connection string such as Server=myServer;Database=NetPostman;Trusted_Connection=True;.

The HttpClient:TimeoutSeconds setting controls the maximum time in seconds that requests wait for responses. The default value of 60 seconds suits most API interactions, but may require adjustment for long-running operations.

The HttpClient:MaxResponseSizeBytes setting limits the response body size accepted by the proxy service. The default of 52428800 bytes (50 MB) prevents memory issues with large responses but may be increased for specific use cases.

### Web.config Settings

The web.config file configures IIS integration. The maxAllowedContentLength attribute controls maximum upload sizes in bytes, currently set to 104857600 (100 MB) to support file uploads and large request bodies.

The aspNetCore element configures the hosting model, currently set to inprocess for optimal performance. The stdoutLogFile path can be adjusted for custom logging locations.

## IIS Deployment Steps

Deploying NetPostman to Internet Information Services (IIS) involves preparing the application package, configuring the web server, and ensuring proper permissions for operation. This section provides comprehensive instructions for deploying NetPostman to a Windows Server environment.

### Server Prerequisites

Before deploying NetPostman, ensure the target server meets all prerequisites and has the necessary components installed. These prerequisites are essential for the application to function correctly and perform optimally.

The .NET Core 8.0 Hosting Bundle must be installed on the server. This component includes the ASP.NET Core Module for IIS, which enables IIS to host .NET Core applications. Download the hosting bundle from the official Microsoft download page and run the installer on the server. After installation, restart the server or restart IIS using the iisreset command to ensure the module is properly registered.

Internet Information Services must be installed with the required role services. Use the Server Manager dashboard to add the Web Server (IIS) role, ensuring that the following role services are included: Web Server > Common HTTP Features, Web Server > Application Development, Web Server > Security, and Management Tools > IIS Management Console. The ASP.NET Core Module is installed automatically with the hosting bundle.

Windows Server should be kept up to date with the latest security patches. Ensure that .NET Core runtime updates are applied promptly to address any security vulnerabilities. The hosting bundle can be updated independently of the .NET SDK, so monitor release announcements for both components.

### Publishing the Application

The application must be published to produce deployment-ready files. The publish process compiles the application in Release mode, copies all required files to a designated output directory, and produces a self-contained package that can be deployed to any server with the hosting bundle installed.

Execute the following command from the solution directory to publish the application:

```bash
dotnet publish -c Release -o ./publish
```

This command creates a Release build and copies all files to the ./publish directory. The publish output includes the application DLLs, configuration files, the web.config file, and all static assets including CSS, JavaScript, and third-party libraries. The total size of the publish output is approximately 20-30 megabytes depending on the included assets.

The publish directory structure is organized as follows:

```
publish/
├── appsettings.json              # Application configuration
├── appsettings.Development.json  # Development settings (if present)
├── web.config                    # IIS configuration
├── NetPostman.Core.dll           # Core layer assembly
├── NetPostman.Infrastructure.dll # Infrastructure layer assembly
├── NetPostman.Web.dll            # Web layer assembly
├── Newtonsoft.Json.dll           # JSON serialization library
├── Microsoft.EntityFramework.Core.dll # EF Core library
├── SQLitePCLRaw.*.dll            # SQLite native libraries
├── css/
│   └── style.css                 # Main stylesheet
├── js/
│   └── app.js                    # Main JavaScript file
└── lib/
    └── codemirror/               # CodeMirror editor library
```

The publish directory is self-contained and includes all dependencies. No additional installations or configurations are required on the target server beyond copying these files.

### File Transfer

Copy the entire contents of the publish directory to the target server. Create a dedicated directory for the application, such as C:\inetpub\netpostman or D:\WebApps\NetPostman. The application directory path should not contain spaces or special characters that might cause issues with command-line tools or scripts.

Use one of the following methods to transfer the files:

Remote Desktop copy-paste functionality allows dragging and dropping files between your local machine and the server. This method is convenient for small deployments but may be slow for large files or slow network connections.

Windows File Sharing (SMB) enables copying files over the network using the server's administrative shares. Map a network drive to \\servername\c$\inetpub\netpostman or use the Copy-Item command in PowerShell with UNC paths. This method supports large files and provides progress indicators.

FTP or SFTP services can be used if configured on the server. Upload the files using an FTP client such as FileZilla or the ftp command-line tool. This method works well when direct server access is not available.

Robocopy provides robust file copying with retry logic and logging. Use the following command to copy files with detailed progress:

```cmd
robocopy .\publish \\servername\c$\inetpub\netpostman /E /ZB /R:3 /W:10 /V /TEE
```

After copying, verify that all files were transferred successfully. Check the file count and total size against the source directory to ensure nothing was missed during the transfer.

### Application Pool Configuration

Create a dedicated Application Pool for NetPostman in IIS Manager. Right-click on Application Pools in the Connections panel and select Add Application Pool. Configure the following settings in the Add Application Pool dialog:

The Name field should be set to NetPostman or a descriptive name that identifies the application. The .NET CLR version must be set to "No Managed Code" because ASP.NET Core applications do not use the .NET Framework CLR. The Managed pipeline mode should be set to "Integrated", which is the default and recommended mode for ASP.NET Core applications.

The identity under which the application pool runs determines the permissions available to the application. The default ApplicationPoolIdentity is suitable for most deployments. This built-in identity has limited permissions and follows the principle of least privilege. If your application requires access to specific resources, create a custom identity with only the necessary permissions.

After creating the application pool, configure additional settings by right-clicking on the pool and selecting Advanced Settings. The following settings are recommended:

The Load User Profile setting should be set to True to enable the application to access user-specific resources. The Recycling settings can be left at their defaults, though you may want to configure regular recycling during off-peak hours. The CPU settings can remain at their defaults unless you have specific processor affinity requirements.

### Website Configuration

Create a new Website or Virtual Directory in IIS to host the NetPostman application. Right-click on Sites in the Connections panel and select Add Website. Configure the following settings in the Add Website dialog:

The Site name field should be set to NetPostman or a descriptive name. The Application pool field should reference the NetPostman application pool created in the previous step. The Physical path field should point to the directory where you copied the published files.

The Binding section configures how the website responds to requests. The Type should be set to http for standard HTTP traffic or https if you have an SSL certificate. The IP address can be set to All Unassigned to listen on all server addresses, or a specific IP address if needed. The Port field defaults to 80 for HTTP or 443 for HTTPS. The Host name field can be left empty or set to a specific domain name if hosting multiple sites.

If using HTTPS, you must have an SSL certificate bound to the website. Obtain a certificate from a trusted certificate authority or use a self-signed certificate for testing. Bind the certificate by selecting the website, clicking Bindings in the Actions panel, and adding or editing the HTTPS binding with the certificate.

Click OK to create the website. The website is now configured and ready to serve the NetPostman application.

### Directory Permissions

The application pool identity requires modify permissions on the application directory to create and update the SQLite database file. Grant these permissions to ensure the application can initialize and persist data correctly.

Right-click on the application directory in Windows Explorer and select Properties. Navigate to the Security tab and click Edit to change permissions. Click Add to add a new security principal. In the Enter object names to select field, enter the application pool name in the format IIS AppPool\NetPostman and click Check Names to validate the identity.

After adding the application pool identity, grant the following permissions: Read & Execute to allow reading and executing files, List folder contents to enable directory browsing, Read to access file contents, and Write to create and modify files. The Write permission is essential for the database file creation and updates.

For enhanced security, you can grant permissions only to the specific database file instead of the entire directory. However, this requires setting more granular permissions and may need to be updated if the database file location changes. The simpler approach of granting directory-level permissions is recommended for most deployments.

### Verifying the Deployment

After configuring the application pool and website, verify that the application is functioning correctly. Open a web browser and navigate to the website URL. The NetPostman home page should load, displaying the request builder interface with the sidebar, request panel, and response panel.

If the application does not load, check the following troubleshooting items:

Verify that the application pool is started by opening IIS Manager and checking the status of the NetPostman application pool. Start the pool if it is stopped. Check the application pool identity has the necessary permissions on the application directory. Review the Windows Event Viewer Application logs for error messages from the ASP.NET Core Module. Check the stdout log file configured in web.config for detailed application output.

Common issues include missing hosting bundle installation, incorrect directory permissions, port conflicts with other websites, and configuration errors in appsettings.json. The error messages in the event logs typically indicate the specific cause of the problem.

### Post-Deployment Configuration

After successful deployment, consider the following configuration items for production operation:

Configure logging verbosity in appsettings.json. The default logging level of Information is suitable for development but may be too verbose for production. Consider changing the LogLevel:Default to Warning or Error to reduce log file growth.

Set appropriate limits for request and response sizes. The default maximum response size of 50 MB suits most use cases, but adjust if your APIs return larger payloads. The maxAllowedContentLength setting in web.config controls maximum upload sizes.

Configure backup procedures for the SQLite database. Since the database is a single file, it can be backed up using standard file backup solutions. Schedule regular backups during off-peak hours and test restoration procedures to ensure data recoverability.

Consider enabling HTTP compression for improved performance. The web.config file includes compression configuration that can be enabled by uncommenting the httpCompression element. Compression reduces bandwidth usage for JSON and text responses.

Monitor application performance and error rates using your organization's standard monitoring tools. The application exposes performance counters through the .NET runtime, and the stdout log file provides application-level logging for troubleshooting.

### Troubleshooting Common Issues

The following issues commonly occur during IIS deployment and their solutions:

If the website returns 502.5 Process Failure, the application failed to start. Check that the .NET Core Hosting Bundle is installed and that the server has been restarted after installation. Verify the application pool identity has directory permissions.

If the website returns 500.19 Internal Server Error with configuration read errors, the web.config file may be malformed or the application pool is configured incorrectly. Verify the web.config XML is valid and the application pool uses "No Managed Code".

If the database is not created, check that the application pool identity has write permissions to the application directory. The database file should appear in the application directory after the first request.

If requests to external APIs fail with timeout errors, the server may not have outbound network access or the proxy configuration may be incorrect. Configure proxy settings in appsettings.json if your network uses a proxy server.

If the application works initially but fails after recycling, the database may be locked by another process or the application pool identity lost permissions. Ensure no other applications access the same database file and verify permissions are correctly configured.

## Project Structure

The NetPostman solution is organized following Clean Architecture principles to ensure maintainability, testability, and clear separation of concerns. The project structure promotes a modular design where each layer has a distinct responsibility and depends only on abstractions defined in the layer below. This architectural approach makes the application flexible enough to accommodate future requirements while maintaining code quality and reducing technical debt.

### Directory Structure

The solution consists of three main projects arranged in a parent-child relationship, with the Web project depending on both Core and Infrastructure projects. Each project has a well-defined purpose and contains only the types appropriate to its layer. This organization supports independent testing of business logic and enables infrastructure components to be swapped without affecting the core application logic.

The complete directory structure for the NetPostman project is as follows:

```
NetPostman/
├── NetPostman.sln                          # Visual Studio solution file
├── README.md                               # Project documentation
├── global.json                             # .NET SDK version specification
└── src/
    ├── NetPostman.Core/                    # Core business layer
    │   ├── NetPostman.Core.csproj          # Core project file
    │   ├── Entities/                       # Domain entities
    │   │   ├── Workspace.cs               # Workspace entity definition
    │   │   ├── Collection.cs              # Collection entity definition
    │   │   ├── Request.cs                 # Request entity definition
    │   │   ├── Environment.cs             # Environment entity definition
    │   │   └── RequestHistory.cs          # RequestHistory entity definition
    │   └── Interfaces/                    # Service abstractions
    │       ├── IHttpRequestService.cs     # HTTP request execution interface
    │       ├── ICollectionRepository.cs   # Collection repository interface
    │       ├── IWorkspaceRepository.cs    # Workspace repository interface
    │       ├── IEnvironmentRepository.cs  # Environment repository interface
    │       └── IRequestHistoryRepository.cs # History repository interface
    │
    ├── NetPostman.Infrastructure/          # Infrastructure layer
    │   ├── NetPostman.Infrastructure.csproj # Infrastructure project file
    │   ├── Data/                          # Data access components
    │   │   ├── NetPostmanDbContext.cs     # Entity Framework Core context
    │   │   ├── NetPostmanDbContextFactory.cs # Design-time factory for migrations
    │   │   └── DatabaseInitializer.cs     # Database seeding and initialization
    │   ├── Repositories/                   # Repository implementations
    │   │   ├── CollectionRepository.cs    # Collection data access implementation
    │   │   ├── WorkspaceRepository.cs     # Workspace data access implementation
    │   │   ├── EnvironmentRepository.cs   # Environment data access implementation
    │   │   └── RequestHistoryRepository.cs # History data access implementation
    │   └── Services/                       # Business services
    │       └── HttpRequestService.cs      # HTTP request execution service
    │
    └── NetPostman.Web/                    # Web presentation layer
        ├── NetPostman.Web.csproj          # Web project file
        ├── Program.cs                     # Application entry point
        ├── Startup.cs                     # Application configuration and DI setup
        ├── appsettings.json               # Application configuration
        ├── appsettings.Development.json   # Development-specific settings
        ├── web.config                     # IIS configuration file
        ├── Controllers/                   # MVC Controllers
        │   ├── HomeController.cs          # Main controller for request execution
        │   ├── CollectionController.cs    # Collection CRUD operations
        │   ├── EnvironmentController.cs   # Environment management
        │   └── HistoryController.cs       # History management
        ├── ViewModels/                     # View models and DTOs
        │   ├── RequestViewModels.cs       # Request-related view models
        │   ├── CollectionViewModels.cs    # Collection-related view models
        │   └── ImportExportViewModel.cs   # Import/export view models
        ├── Views/                          # Razor Views
        │   ├── Home/
        │   │   ├── Index.cshtml           # Main application view
        │   │   └── Error.cshtml           # Error page view
        │   └── Shared/
        │       └── _ErrorLayout.cshtml   # Error layout view
        ├── Services/                       # Web-specific services
        │   └── DependencyInjection.cs     # Service registration extensions
        └── wwwroot/                        # Static web assets
            ├── css/
            │   └── style.css              # Main stylesheet with dark theme
            ├── js/
            │   └── app.js                 # Main application JavaScript
            └── lib/
                └── codemirror/            # CodeMirror editor library
                    ├── codemirror.js      # Core editor functionality
                    ├── dracula.css        # Dark theme for editor
                    ├── javascript.js      # JavaScript mode
                    ├── json.js            # JSON mode
                    └── xml.js             # XML mode
```

### Core Layer Details

The Core layer contains the fundamental building blocks of the application that represent the business domain. This layer has no dependencies on any external frameworks or libraries, making it portable and easy to test. All types in the Core layer are pure C# classes and interfaces that express business concepts without any infrastructure concerns.

The Entities subdirectory contains the domain models that represent the core business concepts. The Workspace entity serves as the top-level container for all other entities, representing a logical grouping of collections and environments. Collections organize requests into meaningful groups, supporting both flat lists and hierarchical folder structures. The Request entity captures all configuration for an HTTP request including the method, URL, headers, body, and associated scripts. Environment entities store key-value variables that can be substituted in requests, while RequestHistory tracks executed requests for auditing and quick access.

The Interfaces subdirectory defines contracts that the infrastructure layer must implement. These abstractions decouple the core business logic from implementation details, enabling different database providers, HTTP clients, or other infrastructure components to be used without modifying the core. The IHttpRequestService interface defines how HTTP requests are executed, while repository interfaces define standard operations for persisting and retrieving domain entities.

### Infrastructure Layer Details

The Infrastructure layer contains implementations of the interfaces defined in the Core layer. This layer depends on external frameworks and libraries, particularly Entity Framework Core for data persistence and System.Net.Http for HTTP communication. All dependencies flow inward from this layer to the Core layer, ensuring the core remains framework-agnostic.

The Data subdirectory contains the Entity Framework Core configuration. The NetPostmanDbContext class inherits from DbContext and defines DbSet properties for each entity type. The OnModelCreating method configures entity relationships, primary keys, indexes, and cascade behaviors. The DatabaseInitializer class ensures the database schema is created on first run and seeds the default workspace and global environment.

The Repositories subdirectory contains concrete implementations of the repository interfaces. Each repository class encapsulates all database operations for a specific entity type, providing a clean API for the Web layer to interact with persisted data. Repositories use LINQ queries to retrieve data efficiently and leverage Entity Framework Core's change tracking for updates.

The Services subdirectory contains the HttpRequestService implementation that executes HTTP requests on behalf of the browser client. This service receives request configurations from the Web layer, constructs HttpRequestMessage objects, sends them using HttpClient, and returns formatted response data. The service handles all aspects of HTTP communication including headers, body content, authentication, and error handling.

### Web Layer Details

The Web layer is the presentation layer that handles HTTP requests, renders views, and serves static assets. This layer depends on both the Core and Infrastructure layers, composing them to create the complete application. The Web layer contains controllers, views, view models, static assets, and configuration files necessary for running the application.

The Controllers subdirectory contains MVC controllers that handle incoming HTTP requests. Each controller is responsible for a specific domain area and defines action methods that handle particular operations. Controllers receive input from route parameters, query strings, and request bodies, interact with services and repositories, and return appropriate responses including views, JSON data, or redirects.

The ViewModels subdirectory contains classes that represent data passed between controllers and views. View models are optimized for their specific view requirements, often combining data from multiple entities or transforming entity data for display. This separation keeps views simple while enabling complex business logic in controllers.

The Views subdirectory contains Razor views that render HTML responses. The Index.cshtml view in the Home folder is the main application interface, containing the complete single-page application structure. Views use Razor syntax to embed C# code within HTML markup, enabling dynamic content rendering based on model data.

The wwwroot directory contains static web assets served directly to browsers. The CSS subdirectory contains the main stylesheet with the dark theme styling. The JavaScript subdirectory contains the main application JavaScript file that implements all client-side functionality including request building, response display, and user interactions. The lib subdirectory contains the CodeMirror library for syntax highlighting in code editors.

### Configuration Files

The solution includes several configuration files that control application behavior. The appsettings.json file contains application settings including connection strings, HTTP client configuration, and logging settings. The appsettings.Development.json file contains development-specific overrides such as more verbose logging. The web.config file configures IIS integration including the ASP.NET Core module, request filtering, and HTTP compression.

## Project Architecture

### Solution Architecture

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
