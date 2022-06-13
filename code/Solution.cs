namespace MatCom.Examen;

public class Solution
{
    public static int Solve(int n)
    {
        if (n % 2 == 0)
            return n + 2;
        return n * 2;
    }
}
