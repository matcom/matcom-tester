using System.Diagnostics;
using System.Text.Json;
using MatCom.Tester;

Directory.CreateDirectory(".output");
File.Delete(Path.Combine(".output", "result.md"));
File.WriteAllLines(Path.Combine(".output", "result.md"), new[]
{
    "# Results for MatCom Programming Contest #1",
    "",
    "| Student | Result | Ok | Wrong | Exception | TimeOut |",
    "|:-------:|:------:|:--:|:-----:|:---------:|:-------:|",
});
foreach (var solution in Directory.GetFiles("solutions", "*.cs"))
{
    var oldFiles = Directory
        .EnumerateFiles("code", "*.*", SearchOption.AllDirectories)
        .Where(f => Path.GetFileName(f) != "code.csproj");
    foreach (var oldFile in oldFiles) File.Delete(oldFile);
    File.Copy(solution, Path.Combine("code", "Solution.cs"));
    var info = new ProcessStartInfo("cmd.exe", "/C dotnet run --project tester");
    var process = Process.Start(info);
    process?.WaitForExit();
    if (process?.ExitCode != 0)
    {
        Console.WriteLine($"Error running {solution}");
        continue;
    }
    var name = Path.GetFileNameWithoutExtension(solution);
    var result = JsonSerializer.Deserialize<TestResult>(File.ReadAllText(Path.Combine(".output", "result.json")));
    if (result == null)
    {
        Console.WriteLine($"Error running {solution}");
        continue;
    }
    Console.WriteLine($"{name} - {result.Ok}/{result.Total}");
    File.AppendAllLines(Path.Combine(".output", "result.md"), new[]
    {
        $"| {name} | {(result.Ok == result.Total ? "Success" : "Failure")} | {result.Ok} | {result.Wrong} | {result.Exception} | {result.TimeOut} |",
    });
}
