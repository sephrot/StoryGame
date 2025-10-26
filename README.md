# StoryGame

A simple ASP.NET Core story game engine where users can create interactive stories with multiple scenes and choices with branching logic.

## Current Status

⚠️ Work in progress. Some functionalities are missing or incomplete.

- Users can CRUD stories, scenes, and choices.
- Final and first scenes can be created, but validation is basic.
- Missing good story workflow creation
- Bugs like "Multiple First Scenes", "More than 3 choices" and choices leading back to same scenes still need fixing

## Technical Requirements

- .NET Core 8.0 MVC for backend
- Node.js v22.16.0 (tested)

### How to Start the Application

1. Download and unzip the project folder.
2. Open the folder in vscode
3. Build the project via the terminal: dotnet build
4. Run the project: dotnet run

## Acknowledgments

- Some ideas for implementing story flow and validation were inspired by ChatGPT.
- CSS inspiration and layout guidance also used online resources and ChatGPT.
