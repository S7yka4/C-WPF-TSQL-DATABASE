using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Entities
{
    class PC
    {
        public Guid InventoryNumber
        { get; set; }
        public string Type
        { get; set; }
        public double FrequencyOfCore
        { get; set; }
        public bool SSD
        { get; set; }

        public PC(string type,double frequency,bool ssd)
        {
            InventoryNumber = Guid.NewGuid();
            Type = type;
            FrequencyOfCore = frequency;
            SSD = ssd;
        }

        
    }
}
