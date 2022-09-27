using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimSol
{
    // Token: 0x02000C3D RID: 3133
    public class IncidentWorker_SpaceDebrisImpactWithTarget : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            return false;
        }
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            IntVec3 intVec = parms.spawnCenter;
            List<Thing> list = new List<Thing>();
            ThingDef fallingThing;
            if (Rand.Bool)
            {
                fallingThing = ThingDefOf.MeteoriteIncoming;
                list = ThingSetMakerDefOf.Meteorite.root.Generate();
            }
            else
            {
                fallingThing = ThingDefOf.ShipChunkIncoming;
                for (int i = 0; i < Rand.RangeInclusive(1, 5); i++)
                {
                    list.Add(ThingMaker.MakeThing(ThingDefOf.ShipChunk));
                }
            }
            SkyfallerMaker.SpawnSkyfaller(fallingThing, list, intVec, map);
            LetterDef letterDef = LetterDefOf.PositiveEvent;
            string text = string.Format(this.def.letterText, list[0].def.label).CapitalizeFirst();
            base.SendStandardLetter(this.def.letterLabel + ": " + list[0].def.LabelCap, text, letterDef, parms, new TargetInfo(intVec, map, false), Array.Empty<NamedArgument>());
            return true;
        }
    }
}
