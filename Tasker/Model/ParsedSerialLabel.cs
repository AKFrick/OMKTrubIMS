using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Tasker.Model
{
    public class ParsedSerialLabel
    {
        public ParsedSerialLabel(string StartSerialNumber)
        {
            Regex regex = new Regex(@"[^0-9]*/[^0-9]*");
            string[] splitted = StartSerialNumber.Split('/');
            StartSerial = int.Parse(splitted[0]);
            EndLabel = splitted[1];
        }
        public int StartSerial { get; private set; }
        public string EndLabel { get; private set; }
    }
}
