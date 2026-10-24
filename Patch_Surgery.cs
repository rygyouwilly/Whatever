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
            var harmony = new Harmony("com.user.instantsurgerymod");
            harmony.PatchAll();
        }
    }

    // TARGET UPDATE: Patch the fundamental base execution worker directly
    [HarmonyPatch(typeof(RecipeWorker), nameof(RecipeWorker.ApplyOnPawn))]
    public static class Patch_RecipeWorker_ApplyOnPawn
    {
        [HarmonyPrefix]
        public static bool Prefix(RecipeWorker __instance, Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            // Verify if the active instance recipe matches our targets
            if (__instance.recipe != null && (__instance.recipe.defName == "RES_RepairProlapse" || __instance.recipe.defName == "RES_RepairExtremeProlapse"))
            {
                // Isolate the hediff directly from the target rules
                Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(__instance.recipe.removesHediff);
                if (hediff != null)
                {
                    // Strip the target medical condition out instantly
                    pawn.health.RemoveHediff(hediff);
                    
                    if (__instance.recipe.successfullyRemovedHediffMessage != null)
                    {
                        Messages.Message(string.Format(__instance.recipe.successfullyRemovedHediffMessage, billDoer.LabelShort, hediff.Label, pawn.LabelShort), pawn, MessageTypeDefOf.PositiveEvent, true);
                    }
                }
                
                // Block the core game engine from ever stepping into default failure, resource consumption, or sedation states
                return false; 
            }
            return true;
        }
    }
}
