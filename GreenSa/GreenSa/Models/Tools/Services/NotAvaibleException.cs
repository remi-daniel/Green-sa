using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenSa.Models.Tools.Services
{
    class NotAvaibleException : Exception
    {
        public NotAvaibleException():base()
        {

        }

        public NotAvaibleException(string message) : base(message)
        {

        }
    }
}
