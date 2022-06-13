namespace MatCom.Tester;

using System;
using System.Diagnostics;
using System.Text.Json;

public enum ResultType { Ok, Wrong, Exception, TimeOut }

public class TestResult
{
    public int Ok { get; set; }
    public int Wrong { get; set; }
    public int Exception { get; set; }
    public int TimeOut { get; set; }
    public int Total { get; set; }

    public void Update(ResultType result)
    {
        switch (result)
        {
            case ResultType.Ok:
                Ok++;
                break;
            case ResultType.Wrong:
                Wrong++;
                break;
            case ResultType.Exception:
                Exception++;
                break;
            case ResultType.TimeOut:
                TimeOut++;
                break;
        }
    }
}

public class CaseCache<TIn, TOut>
{
    public CaseCache(int seed, long time, TIn input, TOut output)
    {
        Seed = seed;
        Time = time;
        Input = input;
        Output = output;
    }

    public int Seed { get; set; }
    public long Time { get; set; }
    public TIn Input { get; set; }
    public TOut Output { get; set; }
}

public abstract class TesterBase<TArg, TIn, TOut>
{
    public abstract TIn InputGenerator(int seed, TArg arg);

    public abstract TOut OutputGenerator(TIn input);

    public abstract bool OutputChecker(TOut output, TOut otherOutput);

    public void GenerateResponses(int baseSeed, TArg param, string cacheFilename, int numTests = 100)
    {
        if (File.Exists(cacheFilename))
        {
            return;
        }
        var responses = new List<CaseCache<TIn, TOut>>();
        for (var s = 0; s < numTests; ++s)
        {
            var seed = baseSeed + s;
            var input = InputGenerator(seed, param);
            (var output, var time) = RunTask(OutputGenerator, input, default(TOut));
            if (output is null)
            {
                Console.WriteLine($"Timeout generating output for seed {seed} and input {input}");
                continue;
            }
            var caseCache = new CaseCache<TIn, TOut>(seed, time, input, output);
            responses.Add(caseCache);
        }
        File.WriteAllText(cacheFilename, JsonSerializer.Serialize(responses.ToArray()));
    }

    public TestResult? Test(string cacheFilename, Func<TIn, TOut> func, int maxTimeOuts = 30)
    {
        var testResult = new TestResult();

        var cache = JsonSerializer.Deserialize<List<CaseCache<TIn, TOut>>>(File.ReadAllText(cacheFilename));
        if (cache is null)
        {
            Console.WriteLine($"Failed to deserialize cache file {cacheFilename}");
            return null;
        }

        for (int i = 0; i < cache.Count; ++i)
        {
            var caseCache = cache[i];
            (var result, var time) = RunTask((input) =>
            {
                try
                {
                    return OutputChecker(func(input), caseCache.Output) ? ResultType.Ok : ResultType.Wrong;
                }
                catch (Exception)
                {
                    return ResultType.Exception;
                }
            }, caseCache.Input, ResultType.TimeOut);

            testResult.Update(result);

            Console.ForegroundColor = result == ResultType.Ok ? ConsoleColor.Green : ConsoleColor.Red;
            Console.Write($"Case {i + 1}: {result} ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"{time}s");
            Console.ResetColor();

            if (testResult.TimeOut > maxTimeOuts)
                break;
        }

        testResult.Total = cache.Count;

        var percent = testResult.Ok * 100f / testResult.Total;
        Console.WriteLine($"Results: {testResult.Ok} de {testResult.Total} {percent}%");
        Console.ForegroundColor = testResult.Ok == testResult.Total ? ConsoleColor.Green : ConsoleColor.Red;
        Console.ResetColor();
        Console.WriteLine(testResult.Ok == testResult.Total ? "Success" : "Failure");
        return testResult;
    }

    private static (T?, long) RunTask<T>(Func<TIn, T> func, TIn input, T? onTimeOut, int seconds = 10)
    {
        var result = onTimeOut;
        var task = Task.Run(() => func(input));
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        if (task.Wait(new TimeSpan(seconds: seconds, hours: 0, minutes: 0)))
        {
            result = task.Result;
        }
        task.Dispose();
        return (result, stopwatch.ElapsedMilliseconds);
    }
}
