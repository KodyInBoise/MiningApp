using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;

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
    }
}
