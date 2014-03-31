using P3bble.PCL.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3bble.PCL
{
    public class ServiceLocator
    {
        private static BaseLogger _logger;
        public static BaseLogger Logger
        {
            get
            {
                if (_logger == null)
                    _logger = new BaseLogger();
                return _logger;

            }
            set 
            {
                _logger = value;
            }
        }
    }
}
