namespace LibVDF
{
    public static class StringExtension
    {
        public static int CountOf(this string str, char expectedChar)
        {
            var count = 0;

            foreach (var chr in str)
            {
                if (chr == expectedChar)
                {
                    count++;
                }
            }

            return count;
        }
    }
}