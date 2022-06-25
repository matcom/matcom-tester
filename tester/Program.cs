using System.Text.Json;
using MatCom.Tester;
using Weboo.Examen;

Directory.CreateDirectory(".output");
var tester = new Tester();
tester.GenerateResponses(0, (1, 100), Path.Combine(".output", "cache.json"));
var result = tester.Test(Path.Combine(".output", "cache.json"), Solucion.MejorRotacion)!;
File.Delete(Path.Combine(".output", "result.json"));
File.WriteAllText(Path.Combine(".output", "result.json"), JsonSerializer.Serialize(result));
