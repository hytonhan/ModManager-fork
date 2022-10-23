using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using Modio;
using ModManager.ModIoSystem;
using UnityEngine.Networking;

namespace ModManager
{
    [BepInPlugin("com.modmanager", "Mod Manager", "0.1.0")]
    public class ModManagerPlugin : BaseUnityPlugin
    {
        public static ManualLogSource Log;

        private void Awake()
        {
            Log = Logger;
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            Paths.LoadPaths();
            LoadDependencies();

            // TODO: if possible Client and Downloader should be gotten from DI container?
            var test = new Client(Client.ModioApiUrl, new Credentials(ModIoSecret.ApiKey));
            var downloader = new Downloader(test);
            uint timberbornGameId = 2409939;
            uint rotatingSunModId = 3025645;
            downloader.DownloadModFile(timberbornGameId, rotatingSunModId);
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
