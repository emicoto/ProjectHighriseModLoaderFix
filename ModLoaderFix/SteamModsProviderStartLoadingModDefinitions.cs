using System.Collections.Generic;
using Game.Services;
using Game.Services.Filesystem;
using HarmonyLib;

namespace ModLoaderFix
{
    [HarmonyPatch(typeof(SteamModsProvider), "StartLoadingModDefinitions")]
    public static class SteamModsProviderStartLoadingModDefinitions
    {
        public static bool Prefix(SteamModsProvider __instance)
        {
            ModLoaderFixPlugin.LogInfo("SteamModsProvider.StartLoadingModDefinitions");
            var modService = Game.Game.serv.mods;
            var modDictionary = Traverse.Create(modService).Field<Dictionary<string, ModDefinition>>("_mods").Value;
            var traverse = Traverse.Create(__instance);
            var rootpaths = traverse.Field<Dictionary<string, string>>("rootpaths").Value;
            var moddefs = traverse.Field<List<ModDefinition>>("moddefs").Value;
            
            var values = new List<WorkshopModInfo>(ModLoaderFixPlugin.WorkshopModInfos.Values);
            for (var i = 0; i < ModLoaderFixPlugin.WorkshopModInfos.Count; i++)
            {
                var info = values[i];
                var modDefinition =  info.ParseModInfo();
                var id = modDefinition.modid.id;
                moddefs.Add(modDefinition);
                if (modDictionary.ContainsKey(id))
                {
                    ModLoaderFixPlugin.LogInfo($"Mod {id} already loaded");
                }
                modDictionary[id] = modDefinition;
                rootpaths[id] = info.RootPath;
            }
            
            return false;
        }
    }
}