using Modio;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

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

        public async void DownloadMod(uint modId)
        {
             ModManagerPlugin.Log.LogWarning($"Getting modinfo with modid {modId}");

            var mod = await _modIoClient.Games[_timberbornGameId].Mods[modId].Get();

            ModManagerPlugin.Log.LogWarning($"mod name: {mod.Name}");

            Directory.CreateDirectory($"{Paths.ModManager}\\temp");

            string tempZipLocation = $"{Paths.ModManager}\\temp\\{mod.Name}.zip";
            await _modIoClient.Download(_timberbornGameId,
                                        modId,
                                        new FileInfo(tempZipLocation));

            ModManagerPlugin.Log.LogWarning($"Downloaded zip in {tempZipLocation}");

            ZipFile.ExtractToDirectory(tempZipLocation, Paths.Data);

            ModManagerPlugin.Log.LogWarning($"Extracted to {Paths.Data}");




            var deps = await _modIoClient.Games[_timberbornGameId].Mods[modId].Dependencies.Get();

            ModManagerPlugin.Log.LogWarning($"Found {deps.Count} dependencies");



            File.Delete(tempZipLocation);


            ModManagerPlugin.Log.LogWarning($"Deleted {tempZipLocation}");

            if (!Directory.EnumerateFileSystemEntries($"{Paths.ModManager}\\temp").Any())
            {
                Directory.Delete($"{Paths.ModManager}\\temp");

                ModManagerPlugin.Log.LogWarning($"Deleted temp folder");
            }
        }
    }
}
