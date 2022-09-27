using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace RimSol
{
    internal class CompTargetable_AllNearbyPlantLife : CompTargetable
    {
        protected override bool PlayerChoosesTarget => false;

        protected override TargetingParameters GetTargetingParameters()
        {
            return new TargetingParameters
            {
                canTargetPlants = true,
                validator = delegate (TargetInfo x)
                {
                    Plant plant;
                    if ((plant = x.Thing as Plant) != null)
                    {
                        return base.BaseTargetValidator(x.Thing) && plant.HarvestableNow;
                    }
                    else
                    {
                        return false;
                    }
                }
            };
        }

        public override IEnumerable<Thing> GetTargets(Thing targetChosenByPlayer = null)
        {
            if (this.parent.MapHeld == null)
            {
                yield break;
            }
            TargetingParameters tp = this.GetTargetingParameters();
            foreach (Plant plant in this.parent.MapHeld.listerThings.ThingsInGroup(ThingRequestGroup.Plant))
            {
                if (tp.CanTarget(plant, null))
                {
                    yield return plant;
                }
            }
            yield break;
        }
    }
}
