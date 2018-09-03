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
            List<FileInfo> files = new List<FileInfo>();

            try
            {
                var rootDir = new DirectoryInfo(path);

                foreach (var file in rootDir.GetFiles())
                {
                    try
                    {
                        files.Add(file);
                    }
                    catch (Exception ex) { ExceptionUtil.Handle(ex, ExceptionType.Blacklist, path: file.FullName); }
                }

                foreach (var subDir in rootDir.GetDirectories())
                {
                    try
                    {
                        files.AddRange(GetAllDirectoryFiles(subDir.FullName));
                    }
                    catch (Exception ex) { ExceptionUtil.Handle(ex, ExceptionType.Blacklist, path: subDir.FullName); }
                }
            }
            catch (Exception ex)
            {
                ExceptionUtil.Handle(ex, ExceptionType.Blacklist, path: path);
            }

            return files;
        }
    }
}
