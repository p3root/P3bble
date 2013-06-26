using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3bble.Core.Messages
{
    public class LogsMessage : P3bbleMessage
    {
        public LogsMessage()
            : base(P3bbleEndpoint.Logs)
        {

        }
    }
}
