using System;

namespace WebApi.Support
{
    public static class MathUtils
    {
        public static int Random(int min, int max)
        {
            Random rnd = new Random();
            int res = rnd.Next(min, max + 1);
            return res;
        }
    }
}