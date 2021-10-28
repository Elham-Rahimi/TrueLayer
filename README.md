# TrueLayer

# Description
 This application has two end points
 - it get name of a `Pokemon` and returm some information including `name` , `habitat`, `isLegendary` , `description`.
 - it get name of a `Pokemon` and return information above with some fun translation instead of default description.
 
 This is a .NET Core 5 API using xUnit.
 
# How to setup 
  Please install .Net 5.0 from this link https://dotnet.microsoft.com/download/dotnet/5.0 
  
  Then clone this repository `git clone https://github.com/Elham-Rahimi/TrueLayer.git `.
  
  After that go to the Projects solution by `cd Pokedex`
  
## Run application
   To run the application from the project repository go to the `cd Pokedex/Pokedex`
   
   Enter command `dotnet run` now the project should be visible at localhost:5000. 
   
   You can test the project by `swagger` in `http://localhost:5000/index.html`
   
## Run test
   To run the test for application from the project repository go to the `cd Pokedex/Pokedex.Test`
   
   Enter command `dotnet test`
   
# Improvments
- [] Add test for the middleware.
- [] Add more details for middleware exceptions messages. 
- [] add integration test for second endpoint
 
