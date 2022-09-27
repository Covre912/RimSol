using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
namespace RimSol
{
    internal class CompUseEffect_Ruinseeker : CompUseEffect
    {
        public CompProperties_Ruinseeker Props
        {
            get
            {
                return (CompProperties_Ruinseeker)this.props;
            }
        }

        public override void DoEffect(Pawn usedBy)
		{
			//base.DoEffect(usedBy);
			Quest quest = QuestUtility.GenerateQuestAndMakeAvailable(QuestScriptDefOf.OpportunitySite_AncientComplex, StorytellerUtility.GetProgressScore(usedBy.Map));
            QuestUtility.SendLetterQuestAvailable(quest);
        }
	}
}
