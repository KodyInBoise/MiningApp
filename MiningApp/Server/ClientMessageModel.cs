using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiningApp
{
    public class ClientAction
    {
        private ClientAction(string value) { Value = value; }

        public string Value { get; set; }
        
        public static ClientAction GetAction(string value)
        {
            return new ClientAction(value);
        }

        public override string ToString()
        {
            return Value;
        }

        public static ClientAction StopSession { get { return new ClientAction("StopSession"); } }
        public static ClientAction PauseSession { get { return new ClientAction("PauseSession"); } }
        public static ClientAction StartSession { get { return new ClientAction("StartSession"); } }
        public static ClientAction RestartMiner { get { return new ClientAction("RestartMiner"); } }
        public static ClientAction CloseApp { get { return new ClientAction("CloseApp"); } }
    }

    public class ClientMessageModel
    {
        public string MessageID { get; set; }

        public string ClientID { get; set; }

        public DateTime Timestamp { get; set; }

        public string Message { get; set; }

        public ClientAction Action { get; set; }

        public ClientMessageModel()
        {
            MessageID = ElementHelper.GetNewGuid(8);
        }

        public override string ToString()
        {
            return $"{Action} - {Message}";
        }
    }
}
