using System;
using System.Collections.Generic;
using System.IO;
using BepInEx;
using BepInEx.Configuration;
using Game.Services;
using Game.Services.Filesystem;
using Game;
using HarmonyLib;
using Steamworks;
using UnityEngine;

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
        public static ConfigEntry<KeyCode> ReloadModsKey;
        private void Start()
        {
            _harmony = new Harmony("com.bepinex.plugins.modloaderfix");
            _harmony.PatchAll();
            Instance = this;
            WorkshopPath = System.IO.Path.Combine(Paths.GameRootPath ,"../../workshop/content/423580");
            LogInfo("ModLoaderFix loaded");
            WorkshopModInfos= GetWorkshopDirectories();
       //     FileWatcherWorkshopDirectory();
            ReloadModsKey = Config.Bind("General", "ReloadModsKey", KeyCode.F8);
        }

        private void Update()
        {
            if (!IsMainMenu)
            {
                return;
            }

            if (Input.GetKeyDown(ReloadModsKey.Value))
            {
                LogInfo("Reloading mods");
                ServiceContext.mods.Release();
                ServiceContext.mods.Initialize();
                LogInfo("Mods reloaded");
            }
        }

        public static ServiceContext ServiceContext => Game.Game.serv;
        FileSystemWatcher watcher;
        public static bool IsMainMenu => ServiceContext.scenarios.sessionType == ScenarioService.Type.Unknown;
 //       public void FileWatcherWorkshopDirectory()
//       {
//           if (watcher != null)
//           {
//               watcher.EnableRaisingEvents = false;
//               watcher.Dispose();
//           }
//           watcher = new FileSystemWatcher(WorkshopPath);
//           watcher.NotifyFilter = NotifyFilters.DirectoryName ;
//           watcher.Created += (sender, args) =>
//           {
//               if (!IsWorkshopModDirectory(args.FullPath))
//               {
//                   return;
//               }
//               LogInfo($"Directory created {args.Name}");
//               var info = new WorkshopModInfo(args.FullPath);
//               if (info.IsWorkshopMod)
//               {
//                   WorkshopModInfos[info.Id] = info;
//                   if (!IsMainMenu)
//                   {
//                       return;
//                   }
//                   var modDefinition=info.ParseModInfo();
//                   ServiceContext.saveload.player.prefs.enabledmods[info.Id.m_PublishedFileId.ToString()] = true;
//                   SteamModsProviderStartLoadingModDefinitions.rootpaths[info.Id.m_PublishedFileId.ToString()] = info.RootPath;
//                   SteamModsProviderStartLoadingModDefinitions.moddefs.Add(modDefinition);
//                   SteamModsProviderStartLoadingModDefinitions.modDictionary[modDefinition.modid.id] = modDefinition;
//                   
//               }
//           };
//           watcher.Deleted += (sender, args) =>
//           {
//               if (!IsWorkshopModDirectory(args.FullPath))
//               {
//                   return;
//               }
//               LogInfo($"Directory deleted {args.Name}");
//               var info = new WorkshopModInfo(args.FullPath);
//               if (info.IsWorkshopMod)
//               {
//                   WorkshopModInfos.Remove(info.Id);
//                   if (!IsMainMenu)
//                   {
//                       return;
//                   }
//              
//                   ServiceContext.saveload.player.prefs.enabledmods.Remove(info.Id.m_PublishedFileId.ToString());
//                   SteamModsProviderStartLoadingModDefinitions.rootpaths.Remove(info.Id.m_PublishedFileId.ToString());
//                   SteamModsProviderStartLoadingModDefinitions.moddefs.RemoveAll(modDefinition => modDefinition.modid.id == info.Id.m_PublishedFileId.ToString());
//               }
//           };
//           watcher.EnableRaisingEvents = true;
 //       }
        public bool IsWorkshopModDirectory(string directory)
        {
            var directoryName = Path.GetFileName(directory);
            return Directory.Exists(Path.Combine(WorkshopPath, directoryName));
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
          //     LogInfo($"Found workshop directory {directoryName}");
               workshopModInfos.Add(info.Id,info);
           }
           return workshopModInfos;
        }


    }
}