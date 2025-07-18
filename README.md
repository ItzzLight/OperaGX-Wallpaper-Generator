# OperaGX Wallpaper Generator

A C# tool for creating OperaGX Persona Packs from video and image files. It automates video conversion, thumbnail extraction, and packaging for easy distribution.

## Features
- Converts MP4 videos to WebM format using FFmpeg
- Extracts thumbnails or first frames from videos
- Copies or renames images for pack inclusion
- Generates persona.ini configuration files
- Packages all assets into a zip file for OperaGX

## Requirements
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download)
- [FFmpeg](https://ffmpeg.org/download.html) (update the path in `VideoConverter.cs` if needed)

## Usage
1. Place your video (`.mp4` or `.webm`) and optional image (`.jpg`, `.jpeg`, `.png`) in the `input/` folder.
2. Run the project:
   ```powershell
   dotnet run "WallpaperName" 1 "ProjectFolder" false false
   ```
   - `WallpaperName`: Name for the persona pack (optional)
   - `1`: Version (default: 1)
   - `ProjectFolder`: Output folder name (optional)
   - `false`: Quick convert (use fast settings, optional)
   - `false`: Use first frame as thumbnail (optional)

3. The generated pack will be in the `output/` folder as a `.zip` file.

## Example
```powershell
dotnet run "Light Theme Pack 1" 1 "ThemePack1" false true
```

## How It Works
- If no image is provided, extracts a thumbnail or first frame from the video.
- Converts video to WebM if needed.
- Creates a persona.ini file with metadata.
- Zips all files for easy import into OperaGX.

## Credits
- Author: Itzz Light
- FFmpeg for video processing

## License
MIT License
# OperaGX-Wallpaper-Generator
