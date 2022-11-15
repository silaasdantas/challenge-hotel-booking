# :checkered_flag: Challenge Hotel Booking - API

### :point_right: Description
This project is part of a backend development challenge.  This is an API for a hotel that has only one room to book.  

API requirements are: 
- Every booking starts the next day.  
- Only allowed to book the room for 3 days.  
- It is not allowed to book the room more than 30 days in advance

### :point_right: Stack
- **C#**
- **Asp.Net Core 6**
- **Swagger**
- **Entity Framework In Memory** 
- **LINQ** 
- **xUnit** 
- **Moq** 
- **Moq** 
### :point_right: Processo de Desenvolvimento

I tried to follow best practices using Clean Code and SOLID concepts and API Design I tried not to do something so complex to solve a simple problem, but at the same time I organized the architecture in a clean way, containing the layers of:
- API
- Core
- Infra

### :point_right: Run project

Install .Net Core 6 SDK
link: https://dotnet.microsoft.com/en-us/download/dotnet/6.0

Use the .NET Core CLI to check the installation, type the commands below:
```
$ dotnet --version
```
Access the directory in the root folder of the project where the sln is located and run:
```
$ dotnet restore

$ dotnet build

$ cd src\Hotel.Booking.Api

$ dotnet run
```
> **_Nota:_** To access the documentation paste in the browser: https://localhost:7209/swagger/index.html

### :point_right: Testes
- To run the test project, with the CMD still open, go back to the project's root folder where the sln file is
```
$ dotnet test
```
