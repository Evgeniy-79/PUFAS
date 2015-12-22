using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PUFAS
{
    public class pFile
    {
        private string name;
        private string size;
        private DateTime date;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Size
        {
            get { return size; }
            set { size = value; }
        }

        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }
    }
}
