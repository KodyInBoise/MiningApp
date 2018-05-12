using LiteDB;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiningApp
{
    public enum MinerStatus
    {
        Inactive,
        Stopped,
        Running
    }

    public class MiningRuleModel
    {
        public int ID { get; set; }

        public DateTime CreatedTimestamp { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }

        public string Arguments { get; set; }

        public string Output { get; set; } = "";

        public MinerStatus Status { get; set; }

        public bool ShowWindow { get; set; } = true;
        
        [BsonIgnore]
        public CryptoModel Crypto { get; set; }

        [BsonIgnore]
        public string ProcessName => GetProcessName();

        [BsonIgnore]
        public string CryptoName => Crypto?.Name;



        private Process _process { get; set; }

        public MiningRuleModel()
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

        public void SetStatus(MinerStatus status)
        {
            Status = status;
        }

        public Process GetProcess()
        {
            var procs = Process.GetProcessesByName(ProcessName).ToList();

            if (procs.Any())
            {
                _process = procs[0];
                SetStatus(MinerStatus.Running);
            }
            else
            {
                _process = null;
                SetStatus(MinerStatus.Stopped);
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
