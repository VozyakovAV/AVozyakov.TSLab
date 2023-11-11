namespace AVozyakov
{
    public static class NumberUtils
    {
        public static int GetDecimalPlaces(this double value)
        {
            return BitConverter.GetBytes(decimal.GetBits((decimal)value)[3])[2];
        }

        public static double Round(this double value, int decimals)
        {
            return Math.Round(value, decimals);
        }

        public static double? Round(this double? value, int decimals)
        {
            if (value == null)
                return null;
            return value == null ? null : Math.Round(value.Value, decimals);
        }
    }
}
