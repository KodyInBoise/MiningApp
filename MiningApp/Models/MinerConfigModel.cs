using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiningApp
{
    public class MinerConfigModel
    {
        public int ID { get; set; }
        public DateTime Created { get; set; }

        public string Name { get; set; }
        public string Path { get; set; }
        public string Arguments { get; set; }
        public string Output { get; set; } = "";
        public StatusEnum Status { get; set; }
        public bool ShowWindow { get; set; } = true;

        public string ProcessName => GetProcessName();
        private Process _process { get; set; }

        public enum StatusEnum
        {
            Stopped,
            Running
        }

        public MinerConfigModel()
        {

        }

        public override string ToString()
        {
            return Name;
        }

        public string GetFileName()
        {
            var pathParts = Path.Split('\\').ToList();

            return pathParts[pathParts.Count - 1];
        }

        public void SetStatus(StatusEnum status)
        {
            Status = status;
        }

        public Process GetProcess()
        {
            var procs = Process.GetProcessesByName(ProcessName).ToList();

            if (procs.Any())
            {
                _process = procs[0];
                SetStatus(StatusEnum.Running);
            }
            else
            {
                _process = null;
                SetStatus(StatusEnum.Stopped);
            }

            return _process;
        }

        private string GetProcessName()
        {
            var pathParts = Path.Split('\\').ToList();
            var fileName = pathParts[pathParts.Count - 1].Split('.')[0];

            return fileName;
        }
    }
}
