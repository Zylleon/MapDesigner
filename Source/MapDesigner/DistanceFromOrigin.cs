using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse.Noise;

namespace MapDesigner
{
    public class DistanceFromOrigin : ModuleBase
    {
        public DistanceFromOrigin() : base(0)
        {
        }

        public override double GetValue(double x, double y, double z)
        {
            return Math.Sqrt(x * x + z * z);
        }
    }
}
