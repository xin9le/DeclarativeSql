using System;



namespace DeclarativeSql.Helpers
{
    /// <summary>
    /// Provides helper functions for numerics.
    /// </summary>
    internal static class NumericsHelpers
    {
        /// <summary>
        /// Gets the digit of specified value.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Digit</returns>
        /// <remarks>http://smdn.jp/programming/netfx/tips/get_number_of_digits/</remarks>
        public static int GetDigit(int value)
        {
            //--- 0 は特別扱い
            if (value == 0)
                return 1;

            //--- 比較的小さい値の場合は 10 で割る方が高速
            if (value <= 10000)
            {
                var digit = 1;
                while (value >= 10)
                {
                    value /= 10;
                    digit++;
                }
                return digit;
            }

            //--- ある程度数字が大きいときは対数から算出
            return unchecked((int)Math.Log10(value)) + 1;
        }
    }
}