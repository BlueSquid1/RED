using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;

namespace RED
{
    class Program
    {
        //[STAThread]
        static void Main(string[] args)
        {
            Game program = new Game();
            //program.LoadResources();
            program.Run(60.0);
        }
    }
}
