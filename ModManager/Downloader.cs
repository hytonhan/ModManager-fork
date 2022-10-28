using Modio;
using Modio.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public async Task<(string, Mod, IReadOnlyList<Tag>)> DownloadMod(uint modId, uint fileId)
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
            mod.Modfile = file;
            (string, Mod, IReadOnlyList<Tag>) result = new(tempZipLocation, mod, tags);
            return result;
        }

        public async Task<List<(string, Mod, IReadOnlyList<Tag>)>> DownloadDependencies(uint modId, uint fileId)
        {
            var deps = await _modIoClient.Games[_timberbornGameId].Mods[modId].Dependencies.Get();
            ModManagerPlugin.Log.LogWarning($"Found {deps.Count} dependencies");

            List<(string, Mod, IReadOnlyList<Tag>)> dependencies = new();
            foreach (var dependency in deps)
            {
                dependencies.Add(await DownloadMod(dependency.ModId));
            }

            return dependencies;
        }

        public async Task<(string, Mod, IReadOnlyList<Tag>)> DownloadMod(uint modId)
        {
            var file = await _modIoClient.Games[_timberbornGameId].Mods[modId].Files.Search().First();

            return await DownloadMod(modId, file.Id);
        }
    }
}
