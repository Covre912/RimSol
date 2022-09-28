using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimSol
{
    internal class CompSolusDrill : ThingComp
	{
		public CompProperties_SolusDrill Props
		{
			get
			{
				return (CompProperties_SolusDrill)this.props;
			}
		}
		public float ProgressToNextPortionPercent
		{
			get
			{
				return this.portionProgress / 10000f;
			}
		}

		int numOfRadialCells;

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			this.powerComp = this.parent.TryGetComp<CompPowerTrader>();
			numOfRadialCells = GenRadial.NumCellsInRadius(Props.radius);
		}

		public override void PostExposeData()
		{
			Scribe_Values.Look<float>(ref this.portionProgress, "portionProgress", 0f, false);
			Scribe_Values.Look<float>(ref this.portionYieldPct, "portionYieldPct", 0f, false);
			Scribe_Values.Look<int>(ref this.lastUsedTick, "lastUsedTick", 0, false);
		}

		public override void CompTickRare()
		{
            if (!CanDrillNow()) { return; }
			this.portionProgress += Props.MiningProgressPerTickRare;
			this.portionYieldPct += Props.MiningProgressPerTickRare / 10000f;// * driller.GetStatValue(StatDefOf.MiningYield, true) / 10000f;
			this.lastUsedTick = Find.TickManager.TicksGame;
			if (this.portionProgress > 10000f)
			{
				this.TryProducePortion(this.portionYieldPct);
				this.portionProgress = 0f;
				this.portionYieldPct = 0f;
			}
		}

		public override void PostDeSpawn(Map map)
		{
			this.portionProgress = 0f;
			this.portionYieldPct = 0f;
			this.lastUsedTick = -99999;
		}

		private void TryProducePortion(float yieldPct, Pawn driller = null)
		{
			ThingDef thingDef;
			int num;
			IntVec3 intVec;
			bool nextResource = this.GetNextResource(out thingDef, out num, out intVec);
			if (thingDef == null)
			{
				return;
			}
			int num2 = Mathf.Min(num, thingDef.deepCountPerPortion);
			if (nextResource)
			{
				this.parent.Map.deepResourceGrid.SetAt(intVec, thingDef, num - num2);
			}
			int num3 = Mathf.Max(1, GenMath.RoundRandom((float)num2 * yieldPct));
			Thing thing = ThingMaker.MakeThing(thingDef, null);
			thing.stackCount = num3;
			GenPlace.TryPlaceThing(thing, this.parent.InteractionCell, this.parent.Map, ThingPlaceMode.Near, null, (IntVec3 p) => p != this.parent.Position && p != this.parent.InteractionCell, default(Rot4));
			if (driller != null)
			{
				Find.HistoryEventsManager.RecordEvent(new HistoryEvent(HistoryEventDefOf.Mined, driller.Named(HistoryEventArgsNames.Doer)), true);
			}
			if (nextResource && !this.ValuableResourcesPresent())
			{
				if (DeepDrillUtility.GetBaseResource(this.parent.Map, this.parent.Position) == null)
				{
					Messages.Message("DeepDrillExhaustedNoFallback".Translate(), this.parent, MessageTypeDefOf.TaskCompletion, true);
					return;
				}
				Messages.Message("DeepDrillExhausted".Translate(Find.ActiveLanguageWorker.Pluralize(DeepDrillUtility.GetBaseResource(this.parent.Map, this.parent.Position).label, -1)), this.parent, MessageTypeDefOf.TaskCompletion, true);
				for (int i = 0; i < 21; i++)
				{
					IntVec3 intVec2 = intVec + GenRadial.RadialPattern[i];
					if (intVec2.InBounds(this.parent.Map))
					{
						ThingWithComps firstThingWithComp = intVec2.GetFirstThingWithComp<CompSolusDrill>(this.parent.Map);
						if (firstThingWithComp != null && !firstThingWithComp.GetComp<CompDeepDrill>().ValuableResourcesPresent())
						{
							firstThingWithComp.SetForbidden(true, true);
						}
					}
				}
			}
		}

		private bool GetNextResource(out ThingDef resDef, out int countPresent, out IntVec3 cell)
		{
			return GetNextResource(this.parent.Position, this.parent.Map, out resDef, out countPresent, out cell);
		}

		public bool CanDrillNow()
		{
			return (this.powerComp == null || this.powerComp.PowerOn) && (DeepDrillUtility.GetBaseResource(this.parent.Map, this.parent.Position) != null || this.ValuableResourcesPresent());
		}

		public bool ValuableResourcesPresent()
		{
			ThingDef thingDef;
			int num;
			IntVec3 intVec;
			return this.GetNextResource(out thingDef, out num, out intVec);
		}

		public bool UsedLastTick()
		{
			return this.lastUsedTick >= Find.TickManager.TicksGame - 1;
		}

		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			foreach (Gizmo gizmo in base.CompGetGizmosExtra())
			{
				yield return gizmo;
			}
			if (Prefs.DevMode)
			{
				yield return new Command_Action
				{
					defaultLabel = "Debug: Produce portion (100% yield)",
					action = delegate ()
					{
						this.TryProducePortion(1f, null);
					}
				};
			}
			yield break;
		}

		public override string CompInspectStringExtra()
		{
			if (!this.parent.Spawned)
			{
				return null;
			}
			ThingDef thingDef;
			int num;
			IntVec3 intVec;
			this.GetNextResource(out thingDef, out num, out intVec);
			if (thingDef == null)
			{
				return "DeepDrillNoResources".Translate();
			}
			return "ResourceBelow".Translate() + ": " + thingDef.LabelCap + "\n" + "ProgressToNextPortion".Translate() + ": " + this.ProgressToNextPortionPercent.ToStringPercent("F0");
		}

		public bool GetNextResource(IntVec3 p, Map map, out ThingDef resDef, out int countPresent, out IntVec3 cell)
		{
			for (int i = 0; i < numOfRadialCells; i++)
			{
				IntVec3 intVec = p + GenRadial.RadialPattern[i];
				if (intVec.InBounds(map))
				{
					ThingDef thingDef = map.deepResourceGrid.ThingDefAt(intVec);
					if (thingDef != null)
					{
						resDef = thingDef;
						countPresent = map.deepResourceGrid.CountAt(intVec);
						cell = intVec;
						return true;
					}
				}
			}

			resDef = DeepDrillUtility.GetBaseResource(map, p);
			countPresent = int.MaxValue;
			cell = p;
			return false;
		}

		// Token: 0x04003C52 RID: 15442
		private CompPowerTrader powerComp;

		// Token: 0x04003C53 RID: 15443
		private float portionProgress;

		// Token: 0x04003C54 RID: 15444
		private float portionYieldPct;

		// Token: 0x04003C55 RID: 15445
		private int lastUsedTick = -99999;

		// Token: 0x04003C56 RID: 15446
		private const float WorkPerPortionBase = 10000f;
	}
}
