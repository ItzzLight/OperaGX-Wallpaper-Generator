using System.Diagnostics;

public static class VideoConverter
{
    private static readonly bool inEnv = true;
    public static readonly string FFmpegPath = inEnv ? "ffmpeg" :
        @"C:\Users\raysh\Files\Downloads\Zip\FFmpeg\ffmpeg-7.1.1-essentials_build\bin\ffmpeg.exe";

    public static bool ConvertMp4ToWebm(string mp4Path, string webmPath, bool quickConvert = false)
    {
        var ffmpegExe = FFmpegPath;
        string args = quickConvert
            ? $"-y -i \"{mp4Path}\" -c:v libvpx -b:v 1M -preset ultrafast -c:a libvorbis \"{webmPath}\""
            : $"-y -i \"{mp4Path}\" -c:v libvpx-vp9 -b:v 2M -c:a libopus \"{webmPath}\"";

        double duration = GetVideoDuration(mp4Path);

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = ffmpegExe,
                Arguments = args,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        int lastPercent = -1;
        while (!process.StandardError.EndOfStream)
        {
            var line = process.StandardError.ReadLine();
            if (line != null && line.Contains("time="))
            {
                var timeMatch = System.Text.RegularExpressions.Regex.Match(line, @"time=(\d+):(\d+):(\d+)\.(\d+)");
                if (timeMatch.Success)
                {
                    int h = int.Parse(timeMatch.Groups[1].Value);
                    int m = int.Parse(timeMatch.Groups[2].Value);
                    int s = int.Parse(timeMatch.Groups[3].Value);
                    int ms = int.Parse(timeMatch.Groups[4].Value);
                    double current = h * 3600 + m * 60 + s + ms / 100.0;
                    int percent = duration > 0 ? (int)(current / duration * 100) : 0;
                    if (percent != lastPercent)
                    {
                        lastPercent = percent;
                        Console.Write($"\rConverting: [{new string('#', percent / 2)}{new string('-', 50 - percent / 2)}] {percent}%");
                    }
                }
            }
        }
        process.WaitForExit();
        Console.WriteLine(); // New line after progress bar

        return process.ExitCode == 0 && File.Exists(webmPath) && new FileInfo(webmPath).Length > 0;
    }

    public static bool ExtractFirstFrame(string videoPath, string outputImagePath)
    {
        var ffmpegExe = FFmpegPath;
        var args = $"-y -i \"{videoPath}\" -vf \"select=eq(n\\,0)\" -q:v 3 \"{outputImagePath}\"";
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = ffmpegExe,
                Arguments = args,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        process.Start();
        process.StandardError.ReadToEnd();
        process.WaitForExit();
        return process.ExitCode == 0 && File.Exists(outputImagePath);
    }

    public static bool ExtractThumbnail(string videoPath, string outputImagePath)
    {
        var ffmpegExe = FFmpegPath;
        // This filter selects a "thumbnail" frame (usually near the middle)
        var args = $"-y -i \"{videoPath}\" -vf \"thumbnail\" -frames:v 1 \"{outputImagePath}\"";
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = ffmpegExe,
                Arguments = args,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        process.Start();
        process.StandardError.ReadToEnd();
        process.WaitForExit();
        return process.ExitCode == 0 && File.Exists(outputImagePath);
    }


    public static double GetVideoDuration(string videoPath)
    {
        var ffmpegExe = FFmpegPath;
        var args = $"-i \"{videoPath}\"";
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = ffmpegExe,
                Arguments = args,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        process.Start();
        string output = process.StandardError.ReadToEnd();
        process.WaitForExit();

        var match = System.Text.RegularExpressions.Regex.Match(output, @"Duration: (\d+):(\d+):(\d+)\.(\d+)");
        if (match.Success)
        {
            int h = int.Parse(match.Groups[1].Value);
            int m = int.Parse(match.Groups[2].Value);
            int s = int.Parse(match.Groups[3].Value);
            int ms = int.Parse(match.Groups[4].Value);
            return h * 3600 + m * 60 + s + ms / 100.0;
        }
        return 0;
    }

}

// Quality : ffmpeg -y -i "input.mp4" -c:v libvpx-vp9 -b:v 2M -c:a libopus "output.webm"
// Speed : ffmpeg -y -i "input.mp4" -c:v libvpx -b:v 1M -preset ultrafast -c:a libvorbis "output.webm"