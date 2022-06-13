using System.Text.Json;
using MatCom.Examen;
using MatCom.Tester;

Directory.CreateDirectory(".output");
var tester = new Tester();
tester.GenerateResponses(0, 10, Path.Combine(".output", "cache.json"));
var result = tester.Test(Path.Combine(".output", "cache.json"), Solution.Solve)!;
File.Delete(Path.Combine(".output", "result.json"));
File.WriteAllText(Path.Combine(".output", "result.json"), JsonSerializer.Serialize(result));
