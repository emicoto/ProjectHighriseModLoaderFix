using Game.Services;
using HarmonyLib;

namespace ModLoaderFix
{
    [HarmonyPatch(typeof(AchievementsService),"OnSessionChange")]
    public static class AchievementsServiceOnSessionChange
    {
        public static void Postfix(ref AchievementsService __instance)
        {
            ModLoaderFixPlugin.LogInfo("AchievementsService.OnSessionChange");
            var traverse = Traverse.Create(__instance);
            traverse.Field("_disabled").SetValue(0);
        }
        
    }
}