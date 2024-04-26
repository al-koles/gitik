using System.Diagnostics;

var repos = Directory.GetDirectories("./", "*", SearchOption.TopDirectoryOnly);
var currentDir = Directory.GetCurrentDirectory();
Console.WriteLine("Searchinng repos in " + currentDir);

foreach (var repoPath in repos)
{
    var response = await Pull(repoPath);

    Console.WriteLine("---");
    Console.WriteLine(response);
}

static async Task<string> Pull(string repoPath)
{
    var process = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = "git",
            Arguments = "branch",
            WorkingDirectory = repoPath,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        },
    };

    var response = $"{repoPath}> git pull";

    var branhResponse = await RunProcessAsync(process, "branch --show-current");
    if (branhResponse.IsSuccess)
        response += $" {branhResponse.Response}";

    var pullResponse = await RunProcessAsync(process, "pull");

    return response + Environment.NewLine + pullResponse.Response;
}

static async Task<ProcessResponse> RunProcessAsync(Process process, string args)
{
    process.StartInfo.Arguments = args;
    process.Start();

    var response = await process.StandardOutput.ReadToEndAsync();
    if (!string.IsNullOrEmpty(response))
        return new ProcessResponse(true, response);

    var errorResponse = await process.StandardError.ReadToEndAsync();
    return new ProcessResponse(false, errorResponse);
}

internal record ProcessResponse(bool IsSuccess, string Response);
