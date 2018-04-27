using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiningApp
{
    public class MinerModel
    {
        public int ID { get; set; }
        public DateTime Created { get; set; }

        public string Name { get; set; }
        public string Path { get; set; }
        public string Arguments { get; set; }
        public string Output { get; set; } = "";
        public StatusEnum Status { get; set; }
        public bool ShowWindow { get; set; } = true;

        public enum StatusEnum
        {
            Stopped,
            Running
        }

        public MinerModel()
        {

        }
    }
}
