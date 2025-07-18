using System.IO.Compression;

class PersonaPackBuilder
{
    private readonly string projectName;
    private readonly string inputDir = "./input";
    private readonly string outputDir = "./output";
    private readonly string author = "Itzz Light";
    private string videoFile = "";
    private string imageFile = "";
    private string imageExt = "";
    private string iniFile = "";
    private string zipFile = "";

    public PersonaPackBuilder(string projectName)
    {
        this.projectName = projectName;
    }

    public PersonaPackBuilder CreateProjectDirectory()
    {
        Directory.CreateDirectory(projectName);
        return this;
    }

    public PersonaPackBuilder CopyOrConvertVideo(bool quickConvert = false)
    {
        var webm = Directory.GetFiles(inputDir, "*.webm").FirstOrDefault();
        if (webm != null)
        {
            videoFile = Path.Combine(projectName, "video.webm");
            File.Copy(webm, videoFile, true);
        }
        else
        {
            var mp4 = Directory.GetFiles(inputDir, "*.mp4").FirstOrDefault();
            if (mp4 != null)
            {
                videoFile = Path.Combine(projectName, "video.webm");
                bool converted = VideoConverter.ConvertMp4ToWebm(mp4, videoFile, quickConvert);
                if (!converted)
                    throw new Exception("FFmpeg conversion failed.");
            }
            else
            {
                throw new FileNotFoundException("No .webm or .mp4 video found in input folder.");
            }
        }
        return this;
    }

    public PersonaPackBuilder CopyAndRenameImage(bool firstFrame = false)
    {
        var img = Directory.GetFiles(inputDir, "*.*")
            .FirstOrDefault(f => new[] { ".jpg", ".jpeg", ".png" }.Contains(Path.GetExtension(f).ToLower()));
        if (img == null)
        {
            // No image found, extract first frame from video
            var videoInput = Directory.GetFiles(inputDir, "*.webm").FirstOrDefault()
                ?? Directory.GetFiles(inputDir, "*.mp4").FirstOrDefault();
            if (videoInput == null)
                throw new FileNotFoundException("No video found to extract first frame.");

            var firstFramePath = Path.Combine(projectName, "thumbnail.png");
            if (!(firstFrame ? VideoConverter.ExtractFirstFrame(videoInput, firstFramePath) :
                    VideoConverter.ExtractThumbnail(videoInput, firstFramePath)))
                throw new Exception("Failed to extract first frame from video.");

            imageFile = firstFramePath;
            imageExt = "png";
        }
        else
        {
            imageExt = Path.GetExtension(img).TrimStart('.');
            imageFile = Path.Combine(projectName, $"thumbnail.{imageExt}");
            File.Copy(img, imageFile, true);
        }
        return this;
    }

    public PersonaPackBuilder CreateIniFile(string name, string version)
    {
        iniFile = Path.Combine(projectName, "persona.ini");
        var iniContent = $@"
[Info]
name = {name}
author = {author}
url = 
version = {version}

[Start Page]
background = video.webm
position = center center
title text color = #FFFFFF
title text shadow = #000000
first frame image = thumbnail.{imageExt}
";
        File.WriteAllText(iniFile, iniContent.Trim());
        return this;
    }

    public PersonaPackBuilder ZipFiles()
    {
        Directory.CreateDirectory(outputDir);
        zipFile = Path.Combine(outputDir, $"{projectName}.zip");
        if (File.Exists(zipFile))
        {
            File.Delete(zipFile);
        }
        using (var zip = ZipFile.Open(zipFile, ZipArchiveMode.Create))
        {
            zip.CreateEntryFromFile(videoFile, Path.GetFileName(videoFile));
            zip.CreateEntryFromFile(imageFile, Path.GetFileName(imageFile));
            zip.CreateEntryFromFile(iniFile, Path.GetFileName(iniFile));
        }
        return this;
    }

    public PersonaPackBuilder Cleanup()
    {
        Directory.Delete(projectName, true);
        return this;
    }

    public static bool BuildPack(string name, string version, string projectName = "LiveWallpaper", bool quickConvert = false, bool firstFrame = false)
    {
        try
        {
            new PersonaPackBuilder(projectName)
                .CreateProjectDirectory()
                .CopyOrConvertVideo(quickConvert)
                .CopyAndRenameImage(firstFrame)
                .CreateIniFile(name, version)
                .ZipFiles()
                .Cleanup();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return false;
        }
    }
}
