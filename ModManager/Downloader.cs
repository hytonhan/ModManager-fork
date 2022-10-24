using Modio;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Timberborn.Core;
using Timberborn.MapSystem;

namespace ModManager
{
    public class Downloader
    {
        private Client _modIoClient;
        private uint _timberbornGameId = 3659;

        public Downloader(Client modIoClient)
        {
            _modIoClient = modIoClient;
        }

        // TODO: If mod has dlls then updating that mod fails becasue the dll is already loaded by the game.
        //       But only for BepInEx plugins?
        public async void DownloadModFile(uint modId, uint fileId)
        {
            ModManagerPlugin.Log.LogWarning($"Getting modinfo with modid {modId}");
            var mod = await _modIoClient.Games[_timberbornGameId].Mods[modId].Get();
            ModManagerPlugin.Log.LogWarning($"Trying to download {mod.Name}");

            Directory.CreateDirectory($"{Paths.ModManager}\\temp");
            string tempZipLocation = $"{Paths.ModManager}\\temp\\{modId}_{fileId}.zip";
            await _modIoClient.Download(_timberbornGameId,
                                        modId,
                                        fileId,
                                        new FileInfo(tempZipLocation));
            //ModManagerPlugin.Log.LogWarning($"Downloaded zip in {tempZipLocation}");

            var tags = await _modIoClient.Games[_timberbornGameId].Mods[modId].Tags.Get();
            var file = await _modIoClient.Games[_timberbornGameId].Mods[modId].Files[fileId].Get();

            if (tags.Any(x => x.Name == "Map"))
            {
                ZipFile.ExtractToDirectory(tempZipLocation, MapRepository.CustomMapsDirectory, true);
                ModManagerPlugin.Log.LogWarning($"saved map \"{mod.Name}\" in: {MapRepository.CustomMapsDirectory}");
            }
            else
            {
                string modFolderName = $"{mod.NameId}_{file.Version}";
                ZipFile.ExtractToDirectory(tempZipLocation, Path.Combine(Paths.Data, modFolderName), true);
                ModManagerPlugin.Log.LogWarning($"Extracted to {Paths.Data}");
            }

            File.Delete(tempZipLocation);
            //ModManagerPlugin.Log.LogWarning($"Deleted {tempZipLocation}");


            var deps = await _modIoClient.Games[_timberbornGameId].Mods[modId].Dependencies.Get();
            ModManagerPlugin.Log.LogWarning($"Found {deps.Count} dependencies");
            foreach(var dependency in deps)
            {
                DownloadMod(dependency.ModId);
            }
            
            if (!Directory.EnumerateFileSystemEntries($"{Paths.ModManager}\\temp").Any())
            {
                Directory.Delete($"{Paths.ModManager}\\temp");
                ModManagerPlugin.Log.LogWarning($"Deleted temp folder");
            }
            //ModManagerPlugin.Log.LogWarning($"Downloaded {mod.Name}");
        }

        public async void DownloadMod(uint modId)
        {
            var file = await _modIoClient.Games[_timberbornGameId].Mods[modId].Files.Search().First();

            DownloadModFile(modId, file.Id);
        }
    }
}
