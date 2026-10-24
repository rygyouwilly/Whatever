using System.Collections.Generic;
using Verse;
using RimWorld;

namespace InstantSurgeryMod
{
    // Inherits directly from RimWorld's core surgery architecture
    public class Recipe_SurgeryNoSedation : Recipe_Surgery
    {
        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            if (pawn == null || recipe == null || recipe.removesHediff == null) return;

            // Target and isolate the exact condition on the pawn
            Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(recipe.removesHediff);
            if (hediff != null)
            {
                // Remove the condition instantly
                pawn.health.RemoveHediff(hediff);

                // Send the standard success message directly to the event log
                if (!recipe.successfullyRemovedHediffMessage.NullOrEmpty())
                {
                    Messages.Message(string.Format(recipe.successfullyRemovedHediffMessage, billDoer.LabelShort, hediff.Label, pawn.LabelShort), pawn, MessageTypeDefOf.PositiveEvent, true);
                }
            }
            
            // NOTE: We deliberately do not call base.ApplyOnPawn() here.
            // This completely intercept the process and drops all failure risk and anesthesia routines.
        }
    }
}
