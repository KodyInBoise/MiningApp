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

        public static List<FileInfo> GetAllDirectoryFiles(string path)
        {
            var allFiles = new List<FileInfo>();

            try
            {
                var rootDir = new DirectoryInfo(path);

                var files = rootDir.GetFiles().ToList();
                files.ForEach(x => allFiles.Add(x));

                var dirs = rootDir.GetDirectories();
                foreach (var subDir in dirs)
                {
                    try { allFiles.AddRange(GetAllDirectoryFiles(subDir.FullName)); }
                    catch (UnauthorizedAccessException ex) { ExceptionUtil.Handle(ex, ExceptionType.Blacklist, path: subDir.FullName); }
                    catch (Exception ex) { ExceptionUtil.Handle(ex); }
                }
            }
            catch (Exception ex) { ExceptionUtil.Handle(ex); }

            return allFiles;
        }
    }
}
