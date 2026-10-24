using System.Collections.Generic;
using Verse;
using RimWorld;

namespace InstantSurgeryMod
{
    // Inherits safely from Recipe_RemoveHediff so the bills menu registers it correctly
    public class Recipe_SurgeryNoSedation : Recipe_RemoveHediff
    {
        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            if (pawn == null || recipe == null || recipe.removesHediff == null) return;

            // 1. Instantly pinpoint the exact medical condition on the patient pawn
            Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(recipe.removesHediff);
            if (hediff != null)
            {
                // 2. Safely extract it out of the pawn's active health tracker
                pawn.health.RemoveHediff(hediff);

                // 3. Throw the positive green notification log up on the UI
                if (!recipe.successfullyRemovedHediffMessage.NullOrEmpty())
                {
                    Messages.Message(string.Format(recipe.successfullyRemovedHediffMessage, billDoer.LabelShort, hediff.Label, pawn.LabelShort), pawn, MessageTypeDefOf.PositiveEvent, true);
                }
            }

            // CRITICAL ENGINE INTERCEPTION: 
            // By completely leaving out "base.ApplyOnPawn(...)", we stop the execution trail right here.
            // Because the code never returns to the native engine, the game never triggers the devirtualized 
            // failure algorithms OR the hardcoded anesthesia routine. It forces a 100% clean success state.
        }
    }
}
