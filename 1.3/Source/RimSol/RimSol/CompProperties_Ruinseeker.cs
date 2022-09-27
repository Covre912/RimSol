using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
namespace RimSol
{
    public class CompProperties_Ruinseeker : CompProperties
    {
        public CompProperties_Ruinseeker()
        {
            this.compClass = typeof(CompUseEffect_Ruinseeker);
        }
    }
}
