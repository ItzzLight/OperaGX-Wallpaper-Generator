class Program
{
    static void Main(string[] args)
    {
        int n = 0;
        // If the first argument is an integer, treat it as version and shift indices
        if (args.Length > 0 && int.TryParse(args[0], out _))
            n = -1;

        // If n is -1, set to a blank string, else, check for Name.
        string name = n == -1 ? "" : (args.Length > n ? args[n] : "");
        string version = args.Length > n + 1 ? args[n + 1] : "1";

        if (args.Length > n + 2 && bool.TryParse(args[n + 2], out bool _))
            n = -2; // Skip project name if it's a boolean
        string projectName = n == -2 ? "" : (args.Length > n + 2 ? args[n + 2] : "");

        bool quickConvert = args.Length > n + 3 && bool.TryParse(args[n + 3], out bool parsed) ? parsed : false;
        bool firstFrame = args.Length > n + 4 && bool.TryParse(args[n + 4], out parsed) ? parsed : false;

        // If name is blank, use video file name from input folder
        if (string.IsNullOrWhiteSpace(name))
        {
            var inputDir = "./input";
            var videoFile = Directory.GetFiles(inputDir, "*.webm").FirstOrDefault()
                ?? Directory.GetFiles(inputDir, "*.mp4").FirstOrDefault();
            name = videoFile != null ? Path.GetFileNameWithoutExtension(videoFile) : "Live Custom Wallpaper";
        }

        // If projectName is blank, use name or fallback
        if (string.IsNullOrWhiteSpace(projectName))
        {
            projectName = !string.IsNullOrWhiteSpace(name) ? name : "LiveWallpaper";
        }

        bool success = PersonaPackBuilder.BuildPack(name, version, projectName, quickConvert, firstFrame);

        Console.WriteLine(success ? "Pack created successfully!" : "Pack creation failed.");
    }
}
/// dotnet run "Live Custom Wallpaper" 1 "LiveWallpaper" false false