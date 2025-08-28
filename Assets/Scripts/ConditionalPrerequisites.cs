using UnityEngine;

public static class ConditionalPrerequisites
{
    public static bool GreaterThan(int value1, int value2, int value3, int value4, int value5, int expected)
    {
        return value1 + value2 + value3 + value4 + value5 >= expected;
    }

    public static bool LowerThan(int value1, int value2, int value3, int value4, int value5, int expected)
    {
        return value1 - value2 + value3 - value4 + value5 < expected;
    }

    public static bool EqualTo(int value1, int value2, int value3, int value4, int value5, int expected)
    {
        return value1 + value2 + value3 + value4 + value5 == expected;
    }
}
