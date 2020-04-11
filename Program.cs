using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using MySavesAtPortia.Serialization;

namespace MySavesAtPortia
{
    internal static class Program
    {
        private static void RemoveDLCRequirementsFromSave(string path)
        {
            var archive = new Archive();
            archive.Load(path);

            var summary = archive.Fetch<Summary>("Summary");
            if (summary.DLCRequire.Count == 0)
            {
                Console.WriteLine("This save does not require any DLCs");
                return;
            }
            summary.DLCRequire.Clear();

            archive.Store("Summary", summary);

            var summary2 = archive.Fetch<Summary>("Summary");
            Debug.Assert(summary2.DLCRequire.Count == 0);
            archive.Save(path);
        }

        internal static void Main(string[] args)
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var saveDir = Path.Join(appData, "AppData/LocalLow/Pathea Games/My Time at Portia");

            var saveFiles = Directory.GetFiles(saveDir);
            foreach (var savePath in saveFiles.Where(t => !t.EndsWith(".txt")))
            {
                Console.WriteLine("Patching {0}", Path.GetFileName(savePath));
                RemoveDLCRequirementsFromSave(savePath);
            }
        }
    }
}