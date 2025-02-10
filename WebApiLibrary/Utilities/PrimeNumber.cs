namespace WebApiLibrary.Utilities
{
    public static class PrimeNumber
    {
        public static bool IsPrimeNumber(long value)
        {
            for (int i = 2; i < value; i++)
            {
                if (value % i == 0)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
