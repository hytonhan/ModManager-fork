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
            if (tags.Any(x => x.Name.Equals("Map")))
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
            //DeleteZipFile(mapZipLocation);
        }

        private void ExtractMod(string modZipLocation, Mod modInfo, bool overWrite = true)
        {
            string modFolderName = $"{modInfo.NameId}_{modInfo.Id}_{modInfo.Modfile.Version}";

            //var path = Path.Combine(Paths.Data, modInfo.NameId);
            //ModManagerPlugin.Log.LogMessage($"foo: {path}");

            string dirs = null;
            try
            {
                dirs = Directory.GetDirectories(Paths.Data, $"{modInfo.NameId}_{modInfo.Id}*").SingleOrDefault();
            }
            catch (Exception ex)
            {
                ModManagerPlugin.Log.LogError($"Found multiple folders for for \"{modInfo.Name}\"");
                ModManagerPlugin.Log.LogError($"{ex.Message}");
            }
            if (dirs != null)
            {
                var dirInfo = new DirectoryInfo(dirs);
                if (dirInfo.Name.Equals(modFolderName))
                {
                    ModManagerPlugin.Log.LogMessage($"\tfolder \"{dirInfo.Name}\" already exists, skip.");
                    return;
                }
                ModManagerPlugin.Log.LogMessage($"\t{dirs}");

                ModManagerPlugin.Log.LogMessage($"\tmove to {modFolderName}");
                dirInfo.MoveTo(Path.Combine(Paths.Data, modFolderName));

                //var files = Directory.GetFiles(modFolderName)
                //                      .Where(file => !_foldersToIgnore.Contains(file.Split(Path.DirectorySeparatorChar).Last()));

                DeleteStuff(modFolderName);
            }


            //ZipFile.ExtractToDirectory(modZipLocation, Path.Combine(Paths.Data, modFolderName), overWrite);
            //ModManagerPlugin.Log.LogWarning($"Extracted to {Paths.Data}");

            //DeleteZipFile(modZipLocation);
        }

        private void DeleteStuff(string modFolderName)
        {
            //ModManagerPlugin.Log.LogMessage($"\tDeleteStuff()");
            //ModManagerPlugin.Log.LogMessage($"\t{Path.Combine(Paths.Data, modFolderName)}");
            var modDirInfo = new DirectoryInfo(Path.Combine(Paths.Data, modFolderName));
            ModManagerPlugin.Log.LogMessage($"\t{modDirInfo}");
            //foreach (FileInfo file in modDirInfo.GetFiles())
            //{
            //    ModManagerPlugin.Log.LogMessage($"\t\tdelete file {file}");
            //    file.Delete();
            //}
            var modSubFolders = modDirInfo.GetDirectories("*", SearchOption.AllDirectories)
                                          .Where(file => !_foldersToIgnore.Contains(file.FullName.Split(Path.DirectorySeparatorChar).Last()));
            foreach (DirectoryInfo subDirectory in modSubFolders.Reverse())
            {
                ModManagerPlugin.Log.LogMessage($"\t\tfolder: {subDirectory}");
            }
            foreach (DirectoryInfo subDirectory in modSubFolders.Reverse())
            {
                DeleteFilesFromFolder(subDirectory);
                TryDeleteFolder(subDirectory);
            }

            DeleteFilesFromFolder(modDirInfo);
            TryDeleteFolder(modDirInfo);

            ModManagerPlugin.Log.LogWarning($"Deleted everything expect for {_foldersToIgnore.Aggregate((a, b) => $"{a}, {b}")}");
        }

        private void DeleteFilesFromFolder(DirectoryInfo dir)
        {
            foreach (FileInfo file in dir.GetFiles())
            {
                ModManagerPlugin.Log.LogMessage($"\t\tdelete file {file}");
                file.Delete();
            }
        }

        private void TryDeleteFolder(DirectoryInfo dir)
        {
            try
            {
                if (dir.EnumerateDirectories().Any() == false && dir.EnumerateFiles().Any() == false)
                {
                    ModManagerPlugin.Log.LogMessage($"\t\tdelete folder {dir}");
                    dir.Delete();
                }
            }
            catch (IOException ex)
            {
                ModManagerPlugin.Log.LogMessage($"\t\tIO exc: {ex.Message}");
            }
            catch (Exception ex)
            {
                ModManagerPlugin.Log.LogMessage($"\t\texc: {ex.Message}");
            }
        }

        private void DeleteZipFile(string mapZipLocation)
        {
            System.IO.File.Delete(mapZipLocation);
            ModManagerPlugin.Log.LogWarning($"Deleted {mapZipLocation}");
        }
    }
}
