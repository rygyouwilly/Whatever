using System.Collections.Generic;
using Verse;
using RimWorld;
using HarmonyLib;

namespace InstantSurgeryMod
{
    [StaticConstructorOnStartup]
    public static class ModMain
    {
        static ModMain()
        {
            // Initializes your custom patch sequence
            var harmony = new Harmony("com.user.instantsurgerymod");
            harmony.PatchAll();
        }
    }

    [HarmonyPatch(typeof(Recipe_RemoveHediff), nameof(Recipe_RemoveHediff.ApplyOnPawn))]
    public static class Patch_Recipe_RemoveHediff
    {
        [HarmonyPrefix]
        public static bool Prefix(Recipe_RemoveHediff __instance, Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            if (__instance.def.defName == "RES_RepairProlapse" || __instance.def.defName == "RES_RepairExtremeProlapse")
            {
                Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(__instance.recipe.removesHediff);
                if (hediff != null)
                {
                    pawn.health.RemoveHediff(hediff);
                    if (__instance.recipe.successfullyRemovedHediffMessage != null)
                    {
                        Messages.Message(string.Format(__instance.recipe.successfullyRemovedHediffMessage, billDoer.LabelShort, hediff.Label, pawn.LabelShort), pawn, MessageTypeDefOf.PositiveEvent, true);
                    }
                }
                // Skip the Waylith framework's native surgery routines entirely (no cost, no fail, no sedation)
                return false; 
            }
            return true;
        }
    }
}
