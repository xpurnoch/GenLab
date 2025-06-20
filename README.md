# PV178-Project

## Project Overview

Biology Lab Manager called GenLab is a WPF desktop application built with C# and .NET, designed for managing biological sample data in laboratory environments.  
It supports sample importing/exporting, user authentication, data visualization, and sequence analysis including translation, matching, and classification.

### Features

- Import and export of sample data (JSON and Excel)
- Sequence analysis (RNA transcription, protein translation)
- Organism prediction from DNA sequences
- Sequence similarity matching with Levenshtein distance
- Data visualization using charts and visual markers


## Project Structure

- `Assets/` – Contains static resources like images (e.g., dna.png)  
- `ExportedFiles/` – Stores exported data files (e.g., JSON, XLSX)  
- `Helpers/` – UI and utility helper classes (e.g., visibility converters)  
- `Messages/` – Message classes used for inter-component communication  
- `Migrations/` – Entity Framework Core database migration files  
- `Models/` – Application data models such as Sample, Lab, User, etc.  
- `Services/` – Business logic and data-related services (e.g., AuthService, DataTransferService)  
- `ViewModels/` – MVVM ViewModel classes handling presentation logic  
- `Views/` – XAML views for WPF user interface

- `.gitignore` – Git configuration for excluding files/folders  
- `App.xaml` and `App.xaml.cs` – Application entry point and resources  
- `MainWindow.xaml` – Main window definition  
- `BioLabManager.csproj` – C# project file  
- `BioLabManager.sln` – Visual Studio solution file  
- `biolab.db` – SQLite database file  
- `README.md` – This documentation file

**Input/Output Paths:**

- Exported files are saved in `./ExportedFiles/`  
- No manual path setup required – all handled internally by the application logic


## Usage Notes

- Export and import operations (JSON, Excel) are handled asynchronously to ensure smooth UI experience.
- Exported data files are saved in `ExportedFiles/` folder in either `.json` or `.xlsx` format.
- Database access is implemented using Entity Framework Core with async queries for performance.
- All disposable resources (file streams, database contexts) are managed using `using` statements to prevent memory leaks.
- The application follows the MVVM pattern using CommunityToolkit.Mvvm for clean separation of concerns.



## Project Info

- **Course**: PV178 
- **Student**: xpurnoch 
- **Assignment**: Project 
- **Branch**: submit  
