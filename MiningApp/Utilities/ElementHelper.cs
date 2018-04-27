using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiningApp
{
    public class ElementHelper
    {
        public static string TrimPath(string path, int length = -1)
        {
            var count = path?.Length;
            var pathLength = length > 0 ? length : 30;

            if (count > pathLength)
            {
                var trimmedPath = string.Empty;
                var dirs = path.Split('\\').ToList();

                for (var x = 0; count > pathLength; dirs.RemoveAt(x))
                {
                    trimmedPath = string.Empty;
                    dirs.ForEach(d => trimmedPath += $"\\{d}");

                    count = trimmedPath.Length;
                }

                return $"...{trimmedPath}";
            }
            else
            {
                return path;
            }
        }
    }
}
