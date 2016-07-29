using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;

namespace RFGR
{
    class Program
    {
        //[STAThread]
        static void Main(string[] args)
        {
            Game program = new Game(OpenTK.Graphics.GraphicsMode.Default);
            program.LoadResources();
            program.Run(60);
        }
    }
}
