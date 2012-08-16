using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Random_Song_Generator
{
    class SongsPerAlbumSeeds
    {
        //left skewed
        private static int[] one = new int[20] {1,1,1,1,1,1,1,1,1,2,2,2,2,2,3,3,3,4,4,5};
        private static int[] two = new int[20] { 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 4, 4, 5, 5, 6 };
        private static int[] three = new int[20] { 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 4, 4, 4, 5, 5, 6, 7 };
        private static int[] four = new int[20] {1,1,2,2,2,3,3,3,4,4,4,4,4,5,5,5,6,6,6,7};
        private static int[] five = new int[20] { 1,1,2,2,3,3,4,4,4,5,5,5,5,5,6,6,6,7,7,8};
        private static int[] six = new int[20] {1,2,2,3,3,4,4,5,5,6,6,6,6,6,7,7,8,8,9,10 };
        private static int[] seven = new int[20] { 1,2,3,3,4,4,5,5,6,6,7,7,7,7,8,8,9,9,10,11 };
        private static int[] eight = new int[20] { 1,2,3,4,5,5,6,6,7,7,8,8,8,8,9,9,10,10,11,12 };
        private static int[] nine = new int[20] { 1,2,3,4,5,6,7,7,8,8,9,9,9,9,10,10,11,11,12,13 };
        private static int[] ten = new int[20] { 1,2,3,4,5,6,7,8,9,9,10,10,10,10,11,11,12,12,13,14 };

        public static int[] getSeeds(int avg)
        {
            switch (avg)
            {
                case 1:
                    return one;
                case 2:
                    return two;
                case 3:
                    return three;
                case 4:
                    return four;
                case 5:
                    return five;
                case 6:
                    return six;
                case 7:
                    return seven;
                case 8:
                    return eight;
                case 9:
                    return nine;
                case 10:
                    return ten;
                default:
                    return one;
            }
        }
    }
}
