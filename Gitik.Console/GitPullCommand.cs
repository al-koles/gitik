using CommandLine;

namespace Gitik.Console;

[Verb("pull", HelpText = "Execute git pull for repositories in current directory")]
public class GitPullCommand
{
    [Option("debug", Required = false, Default = false, HelpText = "Attach debugger to the process")]
    public bool Debug { get; set; }
    
    [Option('m', "max", Required = false, Default = 10, HelpText = "Max pull processes")]
    public int Max { get; set; }
}
