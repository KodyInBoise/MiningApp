using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Management;

namespace MiningApp
{
    public class ProcessHelper
    {
        //private OldWindowController _controller => OldWindowController.Instance;

        private List<ConfigModel> _allMiners = new List<ConfigModel>();
        private List<Process> _minerProcesses = new List<Process>();

        public ProcessHelper()
        {
            UpdateMiners();
            GetRunningMiners();
        }

        private async void UpdateMiners()
        {
            //_allMiners = await _controller.GetMiners();
        }

        private void GetRunningMiners()
        {
            /*
            foreach (var miner in _allMiners)
            {
                var proc = miner.GetProcess();

                if (proc != null)
                {
                    _minerProcesses.Add(proc);
                }
            }
            */
            //Testing
            _minerProcesses.ForEach(x => Console.WriteLine(x.ProcessName));
        }

        public void StartMiner(ConfigModel miner)
        {
            
        }

        public static List<Process> GetChildProcesses(Process proc)
        {
            var childProcesses = new List<Process>();
            

            return childProcesses;
        }
    }
}
