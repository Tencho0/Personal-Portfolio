# Portfolio Website

A professional portfolio website built with ASP.NET Core MVC, Entity Framework Core, and SQL Server.

## Features

### Public Pages
- **About Me (/)** - Profile, bio, metrics, tech stack, featured testimonials, and blog preview
- **Portfolio (/portfolio)** - Grid of projects with details, technologies, and links
- **Blog (/blog)** - Blog posts written in Markdown with category support
- **Reviews (/reviews)** - Client testimonials with star ratings
- **Certificates (/certificates)** - Two-tab layout for certificates and awards
- **Contact (/contact)** - Contact form and social links

### Admin Panel
- Secure login with ASP.NET Core Identity
- Dashboard with statistics and recent messages
- CRUD operations for all content types
- Drag-and-drop sorting for projects, reviews, certificates, and awards
- Markdown editor with live preview for blog posts
- Image upload functionality
- Site settings management (profile, metrics, tech stack, social links)

## Technology Stack

- **Framework**: ASP.NET Core MVC (.NET 9)
- **ORM**: Entity Framework Core
- **Database**: SQL Server (LocalDB by default)
- **Authentication**: ASP.NET Core Identity
- **UI**: Bootstrap 5, Bootstrap Icons
- **Markdown**: Markdig library

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- SQL Server (LocalDB included with Visual Studio, or SQL Server Express)

## Getting Started

### 1. Clone the repository

```bash
git clone <repository-url>
cd Personal-Portfolio
```

### 2. Update the connection string (optional)

The default connection string in `appsettings.json` uses LocalDB:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=PortfolioDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  }
}
```

Update this if you're using a different SQL Server instance.

### 3. Update admin credentials (optional)

The default admin credentials in `appsettings.json`:

```json
{
  "Admin": {
    "Email": "admin@portfolio.com",
    "Password": "Admin123!"
  }
}
```

Change these before deploying to production.

### 4. Apply database migrations

```bash
dotnet ef database update
```

If you don't have the EF Core tools installed:

```bash
dotnet tool install --global dotnet-ef
```

### 5. Run the application

```bash
dotnet run
```

The application will start at:
- HTTP: http://localhost:5000
- HTTPS: https://localhost:5001

### 6. Access the admin panel

Navigate to `/admin/login` and sign in with the credentials from `appsettings.json`:
- Email: `admin@portfolio.com`
- Password: `Admin123!`

## Project Structure

```
Portfolio/
├── Areas/
│   └── Admin/
│       ├── Controllers/     # Admin controllers
│       └── Views/           # Admin views
├── Controllers/             # Public controllers
├── Data/
│   ├── ApplicationDbContext.cs
│   └── DbSeeder.cs          # Database seeding
├── Helpers/
│   └── MarkdownHelper.cs    # Markdown to HTML conversion
├── Models/
│   ├── ViewModels/          # View models
│   └── *.cs                 # Entity models
├── Views/                   # Public views
├── wwwroot/
│   ├── images/              # Static images
│   └── uploads/             # Uploaded files
├── appsettings.json         # Configuration
└── Program.cs               # Application entry point
```

## Entity Models

- **Project** - Portfolio projects
- **BlogPost** - Blog articles (Markdown content)
- **Review** - Client testimonials
- **Certificate** - Professional certifications
- **Award** - Awards and recognitions
- **ContactMessage** - Contact form submissions
- **SiteSetting** - Site configuration
- **Metric** - Homepage metrics
- **TechStackItem** - Technology logos
- **SocialLink** - Social media links

## Admin Features

### Drag-and-Drop Sorting
Projects, reviews, certificates, and awards support drag-and-drop reordering in the admin panel.

### Markdown Editor
Blog posts are written in Markdown with a live preview feature. HTML is sanitized to prevent XSS attacks.

### Image Uploads
Images are stored in `wwwroot/uploads/` with subdirectories for different content types:
- `/uploads/projects/`
- `/uploads/blog/`
- `/uploads/reviews/`
- `/uploads/certificates/`
- `/uploads/awards/`
- `/uploads/profile/`
- `/uploads/techstack/`

### Soft Delete
Content entities support soft deletion via the `IsDeleted` flag. Deleted items are excluded from public queries.

## Security

- Admin routes protected with `[Authorize]` attribute
- Anti-forgery tokens on all forms
- HTML sanitization in Markdown rendering
- Password requirements enforced via Identity
- Session-based authentication with configurable cookie settings

## Customization

### Changing the Theme
Edit the CSS variables in `Views/Shared/_Layout.cshtml`:

```css
:root {
    --primary-color: #2563eb;
    --secondary-color: #1e40af;
    --dark-bg: #0f172a;
    --card-bg: #1e293b;
}
```

### Adding New Content Types
1. Create a model in `Models/`
2. Add a DbSet to `ApplicationDbContext`
3. Create a migration: `dotnet ef migrations add AddNewEntity`
4. Create controllers and views

## License

This project is for personal portfolio use.
