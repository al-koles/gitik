using System.Diagnostics;
using System.Reflection;
using CommandLine;
using Gitik.Console;

var verbs = LoadVerbs();
await Parser.Default
    .ParseArguments(args, verbs)
    .WithParsedAsync(RunAsync);

static Type[] LoadVerbs()
{
    return Assembly.GetExecutingAssembly().GetTypes()
        .Where(t => t.GetCustomAttribute<VerbAttribute>() != null).ToArray();		 
}

async Task RunAsync(object obj)
{
    switch (obj)
    {
        case GitPullCommand c:
            await ExecutePullAsync(c);
            break;
        default:
            throw new ArgumentException($"Command {obj.GetType()} is not active");
    }
}

async Task ExecutePullAsync(GitPullCommand command)
{
    if(command.Debug)
        AttachDebugger();
    
    Console.WriteLine("pull executed");
}

void AttachDebugger()
{
    Console.WriteLine("Attaching debugger");
    Debugger.Launch();
}



// var currentDir = Directory.GetCurrentDirectory();
// Console.WriteLine("Searchinng repos in " + currentDir);
//
// var repos = Directory.GetDirectories("./", "*", SearchOption.TopDirectoryOnly)
//     .Where(r => Directory.GetDirectories(r, ".git", SearchOption.TopDirectoryOnly).Any())
//     .ToArray();
// if (!repos.Any())
// {
//     Console.WriteLine("No repositories found");
//     return;
// }
//
// Console.WriteLine("Detected repos:" + Environment.NewLine + string.Join(Environment.NewLine, repos));
// Console.WriteLine();
// Console.WriteLine("---PULLING STARTED---");
// Console.WriteLine();
//
// var processes = repos.Select(r => PullWithHandling(r)).ToList();
// var responses = new List<PullResponse>();
// while (processes.Any())
// {
//     var finished = await Task.WhenAny(processes);
//     processes.Remove(finished);
//     var response = await finished;
//     responses.Add(response);
//
//     Console.WriteLine(response.Response);
//     Console.WriteLine("---");
//     Console.WriteLine();
// }
//
// Console.WriteLine("---PULLING FINISHED---");
//
// var havingError = responses.Where(r => !r.IsSuccess).Select(r => r.Repo).ToList();
// var havingChanges = responses
//     .Where(r => !r.Response.Contains("Already up to date.") &&
//                 !havingError.Contains(r.Repo))
//     .Select(r => r.Repo)
//     .ToList();
//
// PrintAsList(havingChanges, "Changes applied");
// PrintAsList(havingError, "Responded error");
//
// static void PrintAsList(List<string> items, string name)
// {
//     Console.WriteLine();
//     Console.WriteLine($"---{name}---");
//     Console.WriteLine(items.Any() ? string.Join(Environment.NewLine, items) : "no data");
//     Console.WriteLine("---------------------");
// }
//
// static async Task<PullResponse> PullWithHandling(string repoPath)
// {
//     try
//     {
//         return await Pull(repoPath);
//     }
//     catch (Exception e)
//     {
//         return new PullResponse(false, repoPath, $"Error pulling '{repoPath}'" + e.Message);
//     }
// }
//
// static async Task<PullResponse> Pull(string repoPath)
// {
//     using var process = new Process
//     {
//         StartInfo = new ProcessStartInfo
//         {
//             FileName = "git",
//             Arguments = "branch",
//             WorkingDirectory = repoPath,
//             UseShellExecute = false,
//             CreateNoWindow = true,
//             RedirectStandardOutput = true,
//             RedirectStandardError = true,
//         },
//     };
//
//     var response = $"{repoPath}> git pull";
//
//     var branhResponse = await RunProcessAsync(process, "branch --show-current");
//     if (branhResponse.IsSuccess)
//         response += $" {branhResponse.Response}";
//
//     var pullResponse = await RunProcessAsync(process, "pull");
//
//     response += pullResponse.Response;
//
//     return new PullResponse(pullResponse.IsSuccess, repoPath, response);
// }
//
// static async Task<ProcessResponse> RunProcessAsync(Process process, string args)
// {
//     process.StartInfo.Arguments = args;
//     process.Start();
//
//     var response = await process.StandardOutput.ReadToEndAsync();
//     var errorResponse = await process.StandardError.ReadToEndAsync();
//     var isSuccess = string.IsNullOrEmpty(errorResponse);
//
//     return new ProcessResponse(isSuccess,  response + errorResponse);
// }
//
// internal record ProcessResponse(bool IsSuccess, string Response);
//
// internal record PullResponse(bool IsSuccess, string Repo, string Response);
