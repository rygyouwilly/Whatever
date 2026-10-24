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

    [HarmonyPatch(typeof(Recipe_RemoveHediff), nameof(Recipe_RemoveHediff.ApplyOnPawn))]
    public static class Patch_Recipe_RemoveHediff
    {
        [HarmonyPrefix]
        public static bool Prefix(Recipe_RemoveHediff __instance, Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            // FIXED: Changed __instance.def.defName to __instance.recipe.defName
            if (__instance.recipe.defName == "RES_RepairProlapse" || __instance.recipe.defName == "RES_RepairExtremeProlapse")
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
                // Completely skips the game's default surgery completion logic (eliminating failure checks and sedation)
                return false; 
            }
            return true;
        }
    }
}
