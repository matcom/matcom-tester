using System.Text.Json;
using MatCom.Tester;
using Weboo.Examen;

Directory.CreateDirectory(".output");
var tester = new Tester();
tester.GenerateResponses(0, (100, -1000, 1000), Path.Combine(".output", "cache.json"), 100);
var result = tester.Test(Path.Combine(".output", "cache.json"), Solucion.SubarrayDeSumaMaxima)!;
File.Delete(Path.Combine(".output", "result.json"));
File.WriteAllText(Path.Combine(".output", "result.json"), JsonSerializer.Serialize(result));
