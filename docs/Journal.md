# Journal

Documenting daily work on the project.

## December 16 2024

- Further diagrammed what data the frontend will require for Flashcard Back.

- Read through and understood the JMdict XML DTD.

- UML'ed a conversion of JMdict DTD to EF Core domain objects.

- Created feature requirements for sprint 1, which will last until December 23 2024.

- Initialized api-server project structure and Presentation layer.

- Initialized Application layer and Mediatr. I am using Mediatr Pipeline to catch uncaught exceptions and return a Response.ServerError.

- Initialized Infrastructure layer and database.

- Changed my Mediatr handler Response type to focus on StatusCodes. Controllers now map the Response status code to the respective IActionResult.

- Used XUnit to unit test Presentation layer controllers, Application layer Mediatr handlers, Application layer Mediatr Pipeline.

- Using FluentAssertions for deep object comparisons.

## December 17 2024

- Created the ImportJMdict handler, which parses the XML (using System.Xml) into C# objects and then uses the repository to RangeAdd them. No optimizations needed as 200,000 entries are parsed in about 2 seconds. I am keeping each XML Entity's namespace instead of it's string value, as I will implement custom string values on the client side.

- Created the UploadJMdict endpoint and integration test using WebApplicationFactory so this process can be automatically tested.