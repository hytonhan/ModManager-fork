using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using ModManager.ModIoSystem;

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

            //var test = ModIo.Instance.Client.Games[3659].Get();
            //test.ConfigureAwait(true).GetAwaiter().OnCompleted(() =>
            //{
            //    Log.LogError(test.IsCompleted);
            //    Log.LogError(test.IsFaulted);
            //    Log.LogError(test.Result.Name);
            //});

            Test();

        }

        private async void Test()
        {
            uint gameId = 2;
            uint modId = 6;
            await ModIo.Instance.Client.Download(gameId, modId, new FileInfo("Test.zip"));


        }

        private void LoadDependencies()
        {
            Assembly.LoadFrom(Path.Combine(Paths.ModManager, "libs", "Modio.dll"));
        }
    }
}
