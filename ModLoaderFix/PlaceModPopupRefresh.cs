using Game.UI.Popups;
using Game.UI.Session.Mods;
using HarmonyLib;

namespace ModLoaderFix
{
    [HarmonyPatch(typeof(ModsPopup),"InitializeGameObject")]
    public static class PlaceModPopupRefresh
    {
        public static void Postfix()
        {
            WorkshopModInfo.ShowNewMods();
        }
    }
}