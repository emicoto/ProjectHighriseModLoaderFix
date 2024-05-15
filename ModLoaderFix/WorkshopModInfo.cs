using System.Collections.Generic;
using System.IO;
using System.Linq;
using Game.Services;
using Game.UI.Popups;
using Steamworks;
using UnityEngine;

namespace ModLoaderFix
{
    public class WorkshopModInfo
    {
        public static List<ModDefinition> NewMods = new List<ModDefinition>();
        public WorkshopModInfo(string rootPath)
        {
            RootPath = rootPath;
            IsWorkshopMod = ulong.TryParse(Path.GetFileName(RootPath), out var result);
            Id = IsWorkshopMod ? (PublishedFileId_t) result : PublishedFileId_t.Invalid;
            ModDefinition = new ModDefinition
            {
                modid = ModID.MakeWorkshop(Id.m_PublishedFileId.ToString()),
                name = "",
                description = "",
                workshopid = Id.m_PublishedFileId.ToString()
            };
            
       
        }

        public ModDefinition ModDefinition;
        public ModDefinition ParseModInfo(bool isNew = false)
        {
            if (ModInfo != null)
            {
                return ModDefinition;
            }
            if (isNew)
            {
                NewMods.Add(ModDefinition);
            }
            var modInfoPath = Path.Combine(RootPath, "modinfo.json");
            if (File.Exists(modInfoPath))
            {
                ModInfo = JsonUtility.FromJson<ModInfo>(File.ReadAllText(modInfoPath));
                ModDefinition.name = ModInfo.Name;
                ModDefinition.description = ModInfo.Description;
                return ModDefinition;
            }
            CallResult<SteamUGCRequestUGCDetailsResult_t>.APIDispatchDelegate func = (CallResult<SteamUGCRequestUGCDetailsResult_t>.APIDispatchDelegate) ((result, failed) =>
            {
                if (failed || result.m_details.m_eResult != EResult.k_EResultOK)
                    return;
                ModInfo modDefinition = new ModInfo();
                modDefinition.Name = result.m_details.m_rgchTitle;
                modDefinition.Description = result.m_details.m_rgchDescription;
                ModInfo = modDefinition;
                ModDefinition.name = ModInfo.Name;
                ModDefinition.description = ModInfo.Description;
                var json = modDefinition.ToJson();
                ModLoaderFixPlugin.LogInfo($"json: {json}");
                ModLoaderFixPlugin.LogInfo($"Writing modinfo.json to {modInfoPath}");
                File.WriteAllText(modInfoPath, json);
               
              

            });
            
                    CallResult<SteamUGCRequestUGCDetailsResult_t>.Create(func).Set(SteamUGC.RequestUGCDetails(Id, uint.MaxValue), func);
            return ModDefinition;
        }
        public static void ShowNewMods()
        {
            if (NewMods.Count == 0)
            {
                return;
            }
            var modText = GetModList(NewMods);
            var resultText = modText.Length > 200 ? modText.Substring(0, 200) + "..." : modText;
            
            OkPopup.Show($"Total Mods: {NewMods.Count} New Mods: {NewMods.Count} List:\n{resultText}",null);
        }
        public static string GetModList(List<ModDefinition> moddefs)
        {
            var modList = moddefs.Select(mod => $"{mod.name}({mod.modid.id})").ToArray();
            
            return string.Join(",",modList );
        }
        public ModInfo ModInfo { get; set; } = null;
        public bool IsWorkshopMod;
        public string RootPath { get; set; }
        public PublishedFileId_t Id { get; set; }
        
    }
}