using System.Collections.Generic;
using System.Linq;
using Game.Services;
using Game.Services.Filesystem;
using Game.UI.Popups;
using HarmonyLib;
using Steamworks;

namespace ModLoaderFix
{
    [HarmonyPatch(typeof(SteamModsProvider), "StartLoadingModDefinitions")]
    public static class SteamModsProviderStartLoadingModDefinitions
    {
        public static Dictionary<string, ModDefinition> modDictionary;
        public static Dictionary<string, string> rootpaths;
        public static List<ModDefinition> moddefs;

        public static bool Prefix(SteamModsProvider __instance)
        {
            ModLoaderFixPlugin.LogInfo("SteamModsProvider.StartLoadingModDefinitions");
            var modService = Game.Game.serv.mods;
             modDictionary = Traverse.Create(modService).Field<Dictionary<string, ModDefinition>>("_mods").Value;
             var traverse = Traverse.Create(__instance);
             rootpaths = traverse.Field<Dictionary<string, string>>("rootpaths").Value;
             moddefs = traverse.Field<List<ModDefinition>>("moddefs").Value;
             var player = ModLoaderFixPlugin.ServiceContext.saveload.player;
            var prefs = player.prefs;
            var values = new List<WorkshopModInfo>(ModLoaderFixPlugin.WorkshopModInfos.Values);
            var count = ModLoaderFixPlugin.WorkshopModInfos.Count;
            for (var i = 0; i < count; i++)
            {
                var info = values[i];
                var id = info.ModDefinition.modid.id;
                var isNew = !prefs.enabledmods.ContainsKey(id);
                var modDefinition =  info.ParseModInfo(isNew);
                moddefs.Add(modDefinition);
                if (isNew)
                {
                prefs.enabledmods[id] = true;
                ModLoaderFixPlugin.LogInfo($"[NewModActive] mod: {id}  name: {modDefinition.name}");
                }
                if (modDictionary.ContainsKey(id))
                {
                    ModLoaderFixPlugin.LogInfo($"Mod {id} already loaded");
                }
                modDictionary[id] = modDefinition;
                rootpaths[id] = info.RootPath;
            }
            player.SavePrefs();
            
            return false;
        }
       
    }
}