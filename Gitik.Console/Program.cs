using System.Diagnostics;

var repos = Directory.GetDirectories("./", "*", SearchOption.TopDirectoryOnly);

foreach (var repoPath in repos)
{
    // Console.WriteLine("Pulling" + repoPath);
    Console.WriteLine(await Pull(repoPath));
    Console.WriteLine("---");
}

static async Task<string> Pull(string repoPath)
{
    var command = $"C/ cd {repoPath} && git pull";
    
    var process = new Process();
    var startInfo = new ProcessStartInfo
    {
        RedirectStandardOutput = true,
        // UseShellExecute = false,
        // CreateNoWindow = true,
        FileName = "git",
        Arguments = "pull",
        WorkingDirectory = repoPath,
    };
    process.StartInfo = startInfo;
    process.Start();

    var gitPullResponse = await process.StandardOutput.ReadToEndAsync();
    
    return $"{repoPath} git pull{Environment.NewLine}{gitPullResponse}";
}