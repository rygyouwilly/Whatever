using System.Collections.Generic;
using Verse;
using RimWorld;

namespace InstantSurgeryMod
{
    // Inheriting from Recipe_RemoveHediff ensures the surgery displays in the UI bills menu
    public class Recipe_SurgeryNoSedation : Recipe_RemoveHediff
    {
        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            if (pawn == null || recipe == null || recipe.removesHediff == null) return;

            // Instantly extract the target condition from the pawn
            Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(recipe.removesHediff);
            if (hediff != null)
            {
                pawn.health.RemoveHediff(hediff);

                if (!recipe.successfullyRemovedHediffMessage.NullOrEmpty())
                {
                    Messages.Message(string.Format(recipe.successfullyRemovedHediffMessage, billDoer.LabelShort, hediff.Label, pawn.LabelShort), pawn, MessageTypeDefOf.PositiveEvent, true);
                }
            }

            // WE DO NOT CALL base.ApplyOnPawn here. 
            // This completely blocks the parent "SurgeryFlesh" from applying anesthesia/sedation.
        }

        // Forcefully intercept and disable any chance of medical surgery failure
        public override bool CheckSurgeryFail(Pawn surgeon, Pawn patient, List<Thing> ingredients, BodyPartRecord part, Bill bill)
        {
            return false; // 0% failure chance guaranteed
        }
    }
}
