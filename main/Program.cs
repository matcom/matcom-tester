using System.Diagnostics;
using System.Text.Json;
using MatCom.Tester;

Directory.CreateDirectory(".output");
File.Delete(Path.Combine(".output", "result.md"));
File.WriteAllLines(Path.Combine(".output", "result.md"), new[]
{
    "# Results of MatCom Programming Contest #1",
    "",
    "| Student name | Result | 👌 | 👎 | 💥 | ⏰ |",
    "| ------------ | ------ | -- | -- | --- | -- |",
});
foreach (var solution in Directory.GetFiles("solutions", "*.cs"))
{
    var oldFiles = Directory
        .EnumerateFiles("code", "*.*", SearchOption.AllDirectories)
        .Where(f => Path.GetFileName(f) != "code.csproj");
    foreach (var oldFile in oldFiles) File.Delete(oldFile);
    File.Copy(solution, Path.Combine("code", "Solution.cs"));
    var name = Path.GetFileNameWithoutExtension(solution);
    var info = new ProcessStartInfo("cmd.exe", "/C dotnet run --project tester");
    var process = Process.Start(info);
    process?.WaitForExit();
    if (process?.ExitCode != 0)
    {
        File.AppendAllLines(Path.Combine(".output", "result.md"), new[]
        {
            $"| {name} | ❌ | - | - | - | - |",
        });
        continue;
    }
    var result = JsonSerializer.Deserialize<TestResult>(File.ReadAllText(Path.Combine(".output", "result.json")));
    if (result == null)
    {
        File.AppendAllLines(Path.Combine(".output", "result.md"), new[]
        {
            $"| {name} | ❌ | - | - | - | - |",
        });
        continue;
    }
    Console.WriteLine($"{name} - {result.Ok}/{result.Total}");
    File.AppendAllLines(Path.Combine(".output", "result.md"), new[]
    {
        $"| {name} | {(result.Ok == result.Total ? "✅" : "❌")} | {result.Ok} | {result.Wrong} | {result.Exception} | {result.TimeOut} |",
    });
}
