using P3bble.PCL;
using P3bble.PCL.Logger;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3bble
{
    public class Logger : BaseLogger
    {
        public override void WriteLine(string message)
        {
            Debug.WriteLine(message);
        }

        public override  void ClearUp()
        {
           // throw new NotImplementedException();
        }
    }
}
