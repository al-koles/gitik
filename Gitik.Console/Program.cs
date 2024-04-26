using System.Diagnostics;

var currentDir = Directory.GetCurrentDirectory();
Console.WriteLine("Searchinng repos in " + currentDir);

var repos = Directory.GetDirectories("./", "*", SearchOption.TopDirectoryOnly)
    .Where(r => Directory.GetDirectories(r, ".git", SearchOption.TopDirectoryOnly).Any())
    .ToArray();
Console.WriteLine("Detected repos:" + Environment.NewLine + string.Join(Environment.NewLine, repos));

var processes = repos.Select(r => PullWithHandling(r)).ToList();
var responses = new List<PullResponse>();
while (processes.Any())
{
    var finished = await Task.WhenAny(processes);
    processes.Remove(finished);
    var response = await finished;
    responses.Add(response);

    Console.WriteLine("---");
    Console.WriteLine(response.Response);
}

var havingError = responses.Where(r => !r.IsSuccess).Select(r => r.Repo).ToList();
var havingChanges = responses
    .Where(r => !r.Response.Contains("Already up to date.") &&
                !havingError.Contains(r.Repo))
    .Select(r => r.Repo);

Console.WriteLine("---Updated---");
Console.WriteLine(string.Join(Environment.NewLine, havingChanges));

Console.WriteLine("---Responded error---");
Console.WriteLine(string.Join(Environment.NewLine, havingError));


static async Task<PullResponse> PullWithHandling(string repoPath)
{
    try
    {
        return await Pull(repoPath);
    }
    catch (Exception e)
    {
        return new PullResponse(false, repoPath, $"Error pulling '{repoPath}'" + e.Message);
    }
}

static async Task<PullResponse> Pull(string repoPath)
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

    response += Environment.NewLine + pullResponse.Response;

    return new PullResponse(pullResponse.IsSuccess, repoPath, response);
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

internal record PullResponse(bool IsSuccess, string Repo, string Response);
