using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Logging;
using Modio;
using Modio.Models;
using ModManager.ModIoSystem;
using UnityEngine.Networking;

namespace ModManager
{
    [BepInPlugin("com.modmanager", "Mod Manager", "0.1.0")]
    public class ModManagerPlugin : BaseUnityPlugin
    {
        public static ManualLogSource Log;

        private async Task Awake()
        {
            Log = Logger;
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            Paths.LoadPaths();
            LoadDependencies();

            // TODO: if possible Client and Downloader should be gotten from DI container?
            var test = new Client(Client.ModioApiUrl, new Credentials(ModIoSecret.ApiKey));
            var downloader = new Downloader(test);
            var extractor = new Extractor();

            uint rotatingSunModId = 2409939;
            uint rotatingSunFileId = 3025645;
            uint soiomoistureModId = 2416276;
            uint soilMoistureFileId = 3034829;
            uint fourRiversModId = 2410662;
            uint fourRiversFileId = 3026341;

            //var mod1 = await downloader.DownloadMod(soiomoistureModId, soilMoistureFileId);
            //var mod1Deps = await downloader.DownloadDependencies(soiomoistureModId, soilMoistureFileId);

            var mod = new Mod()
            {
                Name = "SoilMoistureChanger",
                NameId = "soilmoisturechanger",
                Id = 2416276,
                Modfile = new Modio.Models.File()
                {
                    Version = "1.1.1"
                }
            };
            string location = @"D:\Ohjelmat\Steam\steamapps\common\Timberborn\BepInEx\plugins\ModManager\temp\2410139_3025593.zip";
            var tags = new List<Tag>()
            {
                new Tag(){Name = "Mod"}
            };
            //extractor.Extract(mod1.Item1, mod1.Item2, mod1.Item3);
            extractor.Extract(location, mod, tags);
            //foreach (var dep in mod1Deps)
            //{
            //    extractor.Extract(dep.Item1, dep.Item2, dep.Item3);
            //}

            //var map1 = await downloader.DownloadMod(fourRiversModId, fourRiversFileId);
            //extractor.Extract(map1.Item1, map1.Item2, map1.Item3);


            //if (Directory.Exists($"{Paths.ModManager}\\temp"))
            //{
            //    Directory.Delete($"{Paths.ModManager}\\temp");
            //    ModManagerPlugin.Log.LogWarning($"Deleted temp folder");
            //}
        }


        private void LoadDependencies()
        {
            // Assembly.LoadFrom(Path.Combine(Paths.ModManager, "libs", "System.Numerics.Vectors.dll"));
            // Assembly.LoadFrom(Path.Combine(Paths.ModManager, "libs", "System.Buffers.dll"));
            // Assembly.LoadFrom(Path.Combine(Paths.ModManager, "libs", "System.Text.Json.dll"));
            // Assembly.LoadFrom(Path.Combine(Paths.ModManager, "libs", "System.Text.Encodings.Web.dll"));
            // Assembly.LoadFrom(Path.Combine(Paths.ModManager, "libs", "Microsoft.Bcl.AsyncInterfaces.dll"));
            // Assembly.LoadFrom(Path.Combine(Paths.ModManager, "libs", "Modio.dll"));
        }
    }
}
