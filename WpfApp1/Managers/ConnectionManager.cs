using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Managers
{
    class ConnectionManager
    {
        public static string ConnectString = "Integrated Security = SSPI;" +
         "Initial Catalog=Staff;" +
             "Data Source=(localdb)\\MSSQLLocalDB";
    }
}
