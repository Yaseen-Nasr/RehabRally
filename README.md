#RehabRally

RehabRally is a physiotherapy clinic management application built using .NET Core.
 The application consists of a dashboard that allows doctors to manage categories and exercises and assign them to patients.
Doctor Can Create patient Account And manage Resete Password or edit profile info,
 The patient can then log in to a mobile app that interacts with APIs built for the application to manage their assigned exercises and see their daily progress.

#Architecture

RehabRally follows the clean architecture principles, which separates the presentation layer from the domain layer and database access layer. The solution is structured into the following projects:

`RehabRally.Web`: This is the presentation layer built using ASP.NET Core MVC. It includes controllers, views, and other presentation-related components.

`RehabRally.Core`: This is the domain layer that contains all the business logic and domain models. It includes entities, interfaces, and services.

`RehabRally.Ef`: This is the database access layer that contains the Entity Framework Core context and database migration classes.

By following clean architecture principles, RehabRally provides benefits such as better testability, maintainability, and scalability. The separation of concerns and dependencies between layers allows for easier changes and updates without affecting other parts of the application.

#Features

Dashboard for doctors to manage categories and exercises

Mobile app for patients to manage assigned exercises and view their progress

Three types of notifications for doctors to remind, encourage, or provide precautions to patients

User profile page for doctors to view patient progress and exercise history

Client and server-side validation

Secure APIs using JWT
 the Unit of Work and Repository patterns
#Used
C#,
,SQL Server
,EF Core 
,.NET Core Api
,MVC Core
,JQuery
,Ajax
and other clinet side Liberaries.

