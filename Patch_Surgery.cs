using System.Collections.Generic;
using Verse;
using RimWorld;

namespace InstantSurgeryMod
{
    // Inherits from Recipe_RemoveHediff to ensure the surgery populates properly in the UI bills menu
    public class Recipe_SurgeryNoSedation : Recipe_RemoveHediff
    {
        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            if (pawn == null || recipe == null || recipe.removesHediff == null) return;

            // 1. Instantly target and extract the base medical condition (the prolapse)
            Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(recipe.removesHediff);
            if (hediff != null)
            {
                pawn.health.RemoveHediff(hediff);

                if (!recipe.successfullyRemovedHediffMessage.NullOrEmpty())
                {
                    Messages.Message(string.Format(recipe.successfullyRemovedHediffMessage, billDoer.LabelShort, hediff.Label, pawn.LabelShort), pawn, MessageTypeDefOf.PositiveEvent, true);
                }
            }

            // 2. INJECTION LINE: Scan the pawn's active health tracker for any hardcoded anesthesia hediffs
            // HediffDefOf.Anesthetic references the base game's default sedation tracking definition
            Hediff anestheticHediff = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Anesthetic);
            if (anestheticHediff != null)
            {
                // Forcefully clear out the sedation data node instantly
                pawn.health.RemoveHediff(anestheticHediff);
            }

            // CRITICAL ENGINE INTERCEPTION:
            // Omitting base.ApplyOnPawn(...) stops the game engine from executing its default 
            // failure algorithms, item use hooks, or subsequent anesthesia application routines.
        }
    }
}
