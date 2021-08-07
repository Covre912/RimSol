using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimSol
{
    class CryoBullet : Bullet
    {
        protected override void Impact(Thing hitThing)
        {
            if (hitThing is Pawn hitPawn && hitPawn != null && hitPawn.RaceProps.FleshType == FleshTypeDefOf.Normal)
            {
                if (hitPawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Hypothermia) is Hediff hypo && hypo != null)
                {
                    hypo.Severity += 0.1f;
                }
                else
                {
                    hitPawn.health.AddHediff(HediffDefOf.Hypothermia);
                    hitPawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Hypothermia).Severity = 0.2f;
                }
            }
            base.Impact(hitThing);
        }
    }

}