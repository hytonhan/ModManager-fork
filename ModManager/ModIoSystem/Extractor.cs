using Modio.Models;
using System;
using System.IO;
using System.Collections.Generic;
using System.IO.Compression;
using System.Text;
using Timberborn.MapSystem;
using System.Linq;

namespace ModManager.ModIoSystem
{
    public class Extractor
    {
        private List<string> _foldersToIgnore = new() { "configs" };

        public void Extract(string mapZipLocation, Mod modInfo, IReadOnlyCollection<Tag> tags, bool overWrite = true)
        {
            if(tags.Any(x => x.Name.Equals("Map")))
            {
                ExtractMap(mapZipLocation, modInfo, overWrite);
            }
            else
            {
                ExtractMod(mapZipLocation, modInfo, overWrite);
            }
        }

        private void ExtractMap(string mapZipLocation, Mod modInfo, bool overWrite = true)
        {
            ZipFile.ExtractToDirectory(mapZipLocation, MapRepository.CustomMapsDirectory, overWrite);
            ModManagerPlugin.Log.LogWarning($"saved map \"{modInfo.Name}\" in: {MapRepository.CustomMapsDirectory}");
            DeleteZipFile(mapZipLocation);
        }

        private void ExtractMod(string modZipLocation, Mod modInfo, bool overWrite = true)
        {
            string modFolderName = $"{modInfo.NameId}_{modInfo.Id}_{modInfo.Modfile.Version}";

            //var path = Path.Combine(Paths.Data, modInfo.NameId);
            //ModManagerPlugin.Log.LogMessage($"foo: {path}");

            var dirs = Directory.GetDirectories(Paths.Data, $"{modInfo.NameId}_{modInfo.Id}*");
            ModManagerPlugin.Log.LogMessage($"dirs count: {dirs.Length}");
            foreach(var dir in dirs)
            {
                var dirInfo = new DirectoryInfo(dir);
                ModManagerPlugin.Log.LogMessage($"\t{dir}");

                //var files = Directory.GetFiles(dir)
                //                      .Where(file => !_foldersToIgnore.Contains(file.Split(Path.DirectorySeparatorChar).Last()));
                //foreach(var file in files)
                //{
                //    //File.Delete
                //}

                //foreach (FileInfo file in dirInfo.GetFiles())
                //{
                //    file.Delete();
                //}
                //foreach (DirectoryInfo subDirectory in dirInfo.GetDirectories().Where(file => !_foldersToIgnore.Contains(file.FullName.Split(Path.DirectorySeparatorChar).Last())))
                //{
                //    subDirectory.Delete(true);
                //}

                ModManagerPlugin.Log.LogMessage($"\tmove to {modFolderName}");
                dirInfo.MoveTo(Path.Combine(Paths.Data, modFolderName));
            }

            ZipFile.ExtractToDirectory(modZipLocation, Path.Combine(Paths.Data, modFolderName), overWrite);
            ModManagerPlugin.Log.LogWarning($"Extracted to {Paths.Data}");
            DeleteZipFile(modZipLocation);
        }

        private void DeleteStuff()
        {

        }

        private void DeleteZipFile(string mapZipLocation)
        {
            System.IO.File.Delete(mapZipLocation);
            ModManagerPlugin.Log.LogWarning($"Deleted {mapZipLocation}");
        }
    }
}
