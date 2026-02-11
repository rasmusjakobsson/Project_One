# Project-One

## Description
Project-One is a web application designed to provide search functionality using various search engines. It includes implementations for search engines like AltaVista and ClassicSongSearch.

Ensure these variables are set before running the application.

## How to Run

1. **Restore Dependencies**
   Run the following command to restore the required NuGet packages:
   ```bash
   dotnet restore
   ```

2. **Build the Project**
   Build the solution to ensure all dependencies are correctly configured:
   ```bash
   dotnet build
   ```

3. **Run the Application**
   Start the application using the following command:
   ```bash
   dotnet run 
   ```
   The application will be accessible at `http://localhost:<port>`.

4. **Run Tests**
   To ensure everything is working as expected, run the tests:
   ```bash
   dotnet test
   ```

## Environment Variables

The application requires the following environment variables:

- `ALTA_VISTA_API_KEY`: API key for accessing the AltaVista search engine.
- `CLASSIC_SONG_API_KEY`: API key for accessing the ClassicSongSearch engine.

### Setting Environment Variables on macOS

1. Open your terminal.
2. Add the environment variables to your shell configuration file (e.g., `~/.zshrc` or `~/.bash_profile`):
   ```bash
   export ALTA_VISA_API_KEY="your-alta-vista-api-key"
   export CLASSIC_SONG_API_KEY="your-classic-song-api-key"
   ```
3. Save the file and reload your shell configuration:
   ```bash
   source ~/.zshrc
   ```
