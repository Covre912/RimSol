using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace RimSol
{
    internal class CompProperties_TargetEffect_Harvest : CompProperties
    {
        public CompProperties_TargetEffect_Harvest()
        {
            this.compClass = typeof(CompTargetEffect_Harvest);
        }
    }
}
