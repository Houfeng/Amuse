using Amuse.Demo.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amuse.Demo.Impls
{
    public class A : IA
    {
        public static A Create()
        {
            return new A();
        }
        public IB B { get; set; }
        public int Value1 { get; set; }
        public int Value2 { get; set; }
        public A(int val1, int val2)
        {
            this.Value1 = val1;
            this.Value2 = val2;
        }
        public A() { }
        public int ToResult()
        {
            return this.B.Add(this.Value1, this.Value2);
        }
    }
}
