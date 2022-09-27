using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimSol
{
    internal class CompTargetEffect_Harvest : CompTargetEffect
	{
        public override void DoEffectOn(Pawn user, Thing target)
		{
            foreach (var radialThing in GenRadial.RadialDistinctThingsAround(target.Position, target.Map, 6, true))
            {
				Plant plant = radialThing as Plant;
				if(plant == null) { continue; }
				int num = plant.YieldNow();
				if (num > 0)
				{
					Thing thing = ThingMaker.MakeThing(plant.def.plant.harvestedThingDef, null);
					thing.stackCount = num;

					GenPlace.TryPlaceThing(thing, radialThing.Position, user.Map, ThingPlaceMode.Near, null, null, default(Rot4));
					user.records.Increment(RecordDefOf.PlantsHarvested);
				}
				plant.PlantCollected(user);
			}	
			
		}
	}
}
