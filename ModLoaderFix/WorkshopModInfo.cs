using System.IO;
using Game.Services;
using Steamworks;
using UnityEngine;

namespace ModLoaderFix
{
    public class WorkshopModInfo
    {
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

        private ModDefinition ModDefinition;
        public ModDefinition ParseModInfo()
        {
            if (ModInfo != null)
            {
                return ModDefinition;
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

        public ModInfo ModInfo { get; set; } = null;
        public bool IsWorkshopMod;
        public string RootPath { get; set; }
        public PublishedFileId_t Id { get; set; }
        
    }
}