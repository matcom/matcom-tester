using System;

namespace MatCom.Tester;

public class Tester : TesterBase<int, int, int>
{
    public override int InputGenerator(int seed, int arg)
    {
        var random = new Random(seed);
        return random.Next(0, arg);
    }

    public override bool OutputChecker(int output, int otherOutput)
    {
        return output == otherOutput;
    }

    public override int OutputGenerator(int input)
    {
        return 2 * input;
    }
}
