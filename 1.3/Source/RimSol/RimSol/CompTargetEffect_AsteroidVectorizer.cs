using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimSol
{
    internal class CompTargetEffect_AsteroidVectorizer : CompTargetEffect
    {
        public CompProperties_Ruinseeker Props
        {
            get
            {
                return (CompProperties_Ruinseeker)this.props;
            }
        }
        public override void DoEffectOn(Pawn user, Thing target)
        {
            IncidentParms incidentParms = new IncidentParms();
            incidentParms.target = user.Map;
            incidentParms.spawnCenter = target.Position;
            incidentParms.points = StorytellerUtility.GetProgressScore(user.Map);
            incidentParms.forced = true;
            DefDatabase<IncidentDef>.GetNamed("RS_SpaceDebrisImpactWithTarget").Worker.TryExecute(incidentParms);
        }
    }
}
