namespace MatCom.Tester;

public class Tester : TesterBase<(int, int, int), int[], int[], int[]>
{
    public override int[] InputGenerator(int seed, (int, int, int) arg)
    {
        var random = new Random(seed);
        var array = new int[random.Next(1, arg.Item1)];
        for (int i = 0; i < array.Length; i++)
            array[i] = random.Next(arg.Item2, arg.Item3);
        return array;
    }

    public override bool OutputChecker(int[] input, int[] output, int[] expectedOutput)
    {
        var sum = output.Sum();
        var expectedSum = expectedOutput.Sum();
        if (sum != expectedSum)
            return false;
        return IsSubArray(output, input);
    }

    public override int[] OutputGenerator(int[] input)
    {
        var maxSum = long.MinValue;
        var maxStart = 0;
        var maxEnd = 0;
        for (int i = 0; i < input.Length; i++)
        {
            var sum = 0L;
            for (int j = i; j < input.Length; j++)
            {
                sum += input[j];
                if (sum > maxSum)
                {
                    maxSum = sum;
                    maxStart = i;
                    maxEnd = j;
                }
            }
        }
        var output = new int[maxEnd - maxStart + 1];
        for (int i = maxStart; i <= maxEnd; i++)
            output[i - maxStart] = input[i];
        return output;
    }

    private static bool IsSubArray(int[] a, int[] b)
    {
        var j = 0;
        var i = 0;
        while (i < a.Length && j < b.Length)
        {
            if (a[i++] == b[j++])
            {
                if (i == a.Length) return true;
            }
            else
            {
                j += 1 - i;
                i = 0;
            }
        }
        return false;
    }

}
