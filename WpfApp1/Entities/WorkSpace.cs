using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Entities
{
    public class WorkSpace
    {
        public Guid Id
        { get; set; }
        public int EthernetPortNumber
        { get; set; }
        public int TelephonePortNumber
        { get; set; }   
        public Guid PCInventoryNumber
        { get; set; }
        public Guid MonitorInventoryNumber
        { get; set; }

        public WorkSpace(int ethernetPortNumber, int telephonePortNumber)
        {
            Id = Guid.NewGuid();
            EthernetPortNumber = ethernetPortNumber;
            TelephonePortNumber = telephonePortNumber;
        }


        public WorkSpace(WorkSpace tmp)
        {
            Id = tmp.Id;
            EthernetPortNumber = tmp.EthernetPortNumber;
            TelephonePortNumber = tmp.TelephonePortNumber;
            PCInventoryNumber = tmp.PCInventoryNumber;
            MonitorInventoryNumber = tmp.MonitorInventoryNumber;
        }

    }
}
