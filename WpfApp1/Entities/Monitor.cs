using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Entities
{
    class Monitor
    {
        public Guid InventoryNumber
        { get; set; }
        public int Size
        { get; set; }
        public string Matrix
        { get; set; }


        public Monitor(int size,string matrix)
        {
            InventoryNumber = Guid.NewGuid();
            Size = size;
            Matrix = matrix;
        }

    }
}
