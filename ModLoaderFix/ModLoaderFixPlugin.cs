using System;
using System.Collections.Generic;
using System.IO;
using BepInEx;
using Game.Services;
using Game.Services.Filesystem;
using Game;
using HarmonyLib;
using Steamworks;

namespace ModLoaderFix
{
    [BepInPlugin("com.bepinex.plugins.modloaderfix", "ModLoaderFix", "0.0.1")]
    public class ModLoaderFixPlugin : BaseUnityPlugin
    {
        public static ModLoaderFixPlugin Instance { get; private set; }
        private Harmony _harmony;
        public static string WorkshopPath { get; private set; }
        public static void LogInfo(string message)
        {
            Instance.Logger.LogInfo(message);
        }
        public static void LogWarning(string message)
        {
            Instance.Logger.LogWarning(message);
        }
        public static void LogError(string message)
        {
            Instance.Logger.LogError(message);
        }
        public static Dictionary<PublishedFileId_t,WorkshopModInfo> WorkshopModInfos { get; private set; }
        public static bool SteamClientStarted;
        private void Start()
        {
            _harmony = new Harmony("com.bepinex.plugins.modloaderfix");
            _harmony.PatchAll();
            Instance = this;
            WorkshopPath = System.IO.Path.Combine(Paths.GameRootPath ,"../../workshop/content/423580");
            LogInfo("ModLoaderFix loaded");
            WorkshopModInfos= GetWorkshopDirectories();
        }
        public static Dictionary<PublishedFileId_t,WorkshopModInfo> GetWorkshopDirectories()
        {
           var directories = Directory.GetDirectories(WorkshopPath);
           var workshopModInfos = new Dictionary<PublishedFileId_t,WorkshopModInfo>();
           foreach (var directory in directories)
           {
               var directoryName = Path.GetFileName(directory);
               
               var info = new WorkshopModInfo(directory);
               if (!info.IsWorkshopMod || info.Id == PublishedFileId_t.Invalid)
               {
                     LogWarning($"Invalid workshop directory {directoryName}");
                   continue;
               }
               LogInfo($"Found workshop directory {directoryName}");
               workshopModInfos.Add(info.Id,info);
           }
           return workshopModInfos;
        }


    }
}