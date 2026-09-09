---
description: Clean and rebuild .NET project to release locked files
---

**Purpose**: Resolve the "Unable to copy file ... because it is being used by another process" error in ASP.NET Core projects.

### Steps
1. **Close the running application**
   - If the project is running from Visual Studio, stop debugging (Shift+F5).
   - If you launched the app via `dotnet run` or an executable, close the console window.

2. **Terminate lingering process (if any)**
   // turbo
   ```powershell
   # Replace 13072 with the actual PID if different
   taskkill /PID 13072 /F
   ```
   - Open Task Manager → Details tab → locate `WebApplication1.exe` or the PID shown in the error and end the task.

3. **Delete temporary build folders**
   // turbo
   ```powershell
   Remove-Item -Recurse -Force "c:\Users\fuaf\source\opject\WebApplication1\bin"
   Remove-Item -Recurse -Force "c:\Users\fuaf\source\opject\WebApplication1\obj"
   ```
   - This ensures no locked files remain.

4. **Clean the project using .NET CLI**
   // turbo
   ```powershell
   dotnet clean "c:\Users\fuaf\source\opject\WebApplication1\WebApplication1.csproj"
   ```

5. **Rebuild the project**
   // turbo
   ```powershell
   dotnet build "c:\Users\fuaf\source\opject\WebApplication1\WebApplication1.csproj" -c Debug
   ```

6. **Run the project**
   ```powershell
   dotnet run --project "c:\Users\fuaf\source\opject\WebApplication1\WebApplication1.csproj"
   ```

### Optional: Prevent future locking
- Disable **Enable Edit and Continue** in Visual Studio (Tools → Options → Debugging → General).
- Ensure no other tool (e.g., antivirus) is scanning the `bin` folder while building.
- Consider adding a post‑build clean step in the `.csproj` if the issue recurs.

**Note**: All commands marked with `// turbo` are safe to run automatically. Use them with caution if you have unsaved work.
