using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PUFAS
{
    public class cFile
    {
        private string name;
        private bool local;
        private string path;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        public bool Local
        {
            get { return local; }
            set { local = value; }
        }
    }
}
