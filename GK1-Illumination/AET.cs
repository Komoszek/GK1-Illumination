using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK1_Illumination
{
    public class AETPointer
    {
        public double yMax;
        public double x;
        public double mInv;
        public int Id1;
        public int Id2;
 
        public AETPointer(int id1, int id2, double x1, double y1, double x2, double y2)
        {
            Id1 = id1;
            Id2 = id2;
            yMax = y2;
            mInv = (x1 - x2) / (y1 - y2);
            x = x1;

        }
    }

    public static class AETListExtensions
    {
        public static void removeAETPointerWithIds(this List<AETPointer> aet, int id1, int id2)
        {
            for (int i = 0; i < aet.Count; i++)
            {
                if (aet[i].Id1 == id1 && aet[i].Id2 == id2)
                {
                    aet.RemoveAt(i);
                    return;
                }
            }
        }
    }
}
