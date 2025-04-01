using System.Diagnostics;

public static class CommandRunner
{
    public static string Run(string command, string arguments = "")
    {
        var process = new Process();
        process.StartInfo.FileName = command;
        process.StartInfo.Arguments = arguments;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;

        process.Start();

        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();

        process.WaitForExit();

        if (!string.IsNullOrEmpty(error))
        {
            // optionally log or throw
            return $"[ERROR]: {error}";
        }

        return output.Trim();
    }
}
