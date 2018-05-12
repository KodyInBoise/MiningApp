using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace MiningApp
{
    public class ProcessHelper
    {
        private WindowController _controller => WindowController.Instance;

        private List<MiningRuleModel> _allMiners = new List<MiningRuleModel>();
        private List<Process> _minerProcesses = new List<Process>();

        public ProcessHelper()
        {
            UpdateMiners();
            GetRunningMiners();
        }

        private async void UpdateMiners()
        {
            _allMiners = await _controller.GetMiners();
        }

        private void GetRunningMiners()
        {
            foreach (var miner in _allMiners)
            {
                var proc = miner.GetProcess();

                if (proc != null)
                {
                    _minerProcesses.Add(proc);
                }
            }

            //Testing
            _minerProcesses.ForEach(x => Console.WriteLine(x.ProcessName));
        }

        public void StartMiner(MiningRuleModel miner)
        {
            
        }
    }
}
