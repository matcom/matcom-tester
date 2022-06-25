namespace MatCom.Tester;

public class Tester : TesterBase<(int, int), int[], int[], IEnumerable<int[]>>
{
    public override int[] InputGenerator(int seed, (int, int) arg)
    {
        var random = new Random(seed);
        var length = random.Next(arg.Item1, arg.Item2);
        return Enumerable.Range(0, length).Select(i => random.Next(-1000, 1000)).ToArray();
    }

    public override bool OutputChecker(int[] input, int[] output, IEnumerable<int[]> expectedOutput)
    {
        foreach (var expected in expectedOutput)
        {
            if (expected.SequenceEqual(output))
            {
                return true;
            }
        }
        return false;
    }

    public override IEnumerable<int[]> OutputGenerator(int[] input)
    {
        var result = new List<(long, int[])>();
        for (var k = 0; k < input.Length; k++)
        {
            var array = Rotate(input, k);
            var count = Count(array);
            result.Add((count, array));
        }
        var min = result.MaxBy(x => x.Item1).Item1;
        return result.Where(x => x.Item1 == min).Select(x => x.Item2);
    }

    private int[] Rotate(int[] array, int k)
    {
        var result = new int[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            result[(i + k) % array.Length] = array[i];
        }
        return result;
    }

    private long Count(int[] array)
    {
        var result = long.MinValue;
        for (var i = 0; i < array.Length; i++)
        {
            var sum = 0L;
            for (var j = i; j < array.Length; j++)
            {
                sum += array[j];
                result = Math.Max(result, sum);
            }
        }
        return result;
    }
}
