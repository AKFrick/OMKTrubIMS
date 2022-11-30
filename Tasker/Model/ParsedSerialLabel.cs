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
            if (StartSerialNumber != null)
            {
               try
                { 
                    string[] splitted = StartSerialNumber.Split('/');
                    StartSerial = int.Parse(splitted[0]);
                    EndLabel = $"/{splitted[1]}";
                }
                catch (Exception e)
                {
                    StartSerial = 0;
                    EndLabel = "";
                    OutputLog.That($"Неверный формат серийного номер: {StartSerialNumber}");
                }
            }
            else
            {
                StartSerial = 0;
                EndLabel = "";
            }
        }
        public int StartSerial { get; private set; }
        public string EndLabel { get; private set; }
    }
}
