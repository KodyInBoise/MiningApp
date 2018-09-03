using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using MiningApp.LoggingUtil;

namespace MiningApp
{
    public class FileHelper
    {
        public static FileHelper Instance { get; set; }

        public FileHelper()
        {
            Instance = this;
        }

        public void CompressDirectory(string source, string dest)
        {
            try
            {
                if (File.Exists(dest)) File.Delete(dest);

                ZipFile.CreateFromDirectory(source, dest, CompressionLevel.Optimal, true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<string> GetAllDirectoryFiles(string dir)
        {
            List<string> files = new List<string>();

            var dirInfo = new DirectoryInfo(dir);
            var lvl = dirInfo.GetAccessControl();
            foreach (var f in Directory.GetFiles(dir))
            {
                try
                {
                    files.Add(f);
                }
                catch (Exception ex) { LogHelper.AddEntry(ex); }
            }
            foreach (var d in Directory.GetDirectories(dir))
            {
                try
                {
                    files.AddRange(GetAllDirectoryFiles(d));
                }
                catch (Exception ex) { LogHelper.AddEntry(ex); }
            }

            return files;
        }
    }
}
