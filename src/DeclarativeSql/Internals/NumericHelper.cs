using System;

namespace DeclarativeSql.Internals;



/// <summary>
/// Provides helper methods for numeric.
/// </summary>
internal static class NumericHelper
{
    /// <summary>
    /// Gets the number of digits of the specified value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <remarks>http://smdn.jp/programming/netfx/tips/get_number_of_digits/</remarks>
    public static byte GetDigit(int value)
    {
        //--- 0 is special
        if (value == 0)
            return 1;

        //--- If smaller value, dividing by 10 is faster
        if (value <= 10000)
        {
            byte digit = 1;
            while (value >= 10)
            {
                value /= 10;
                digit++;
            }
            return digit;
        }

        //--- When the number is large, calculates from the logarithm
        return (byte)(unchecked((byte)Math.Log10(value)) + 1);
    }
}
