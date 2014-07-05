using Amuse.Demo.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amuse.Demo.Impls
{
    public class B :IB
    {
        public int Add(int x, int y)
        {
            return x + y;
        }
    }
}
