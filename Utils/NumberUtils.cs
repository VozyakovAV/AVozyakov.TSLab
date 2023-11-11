namespace AVozyakov
{
    public static class NumberUtils
    {
        public static int GetDecimalPlaces(this double value)
        {
            return BitConverter.GetBytes(decimal.GetBits((decimal)value)[3])[2];
        }
    }
}
