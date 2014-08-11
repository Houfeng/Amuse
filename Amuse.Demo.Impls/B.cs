using Amuse.Demo.Interfaces;

namespace Amuse.Demo.Impls
{
    public class B : IB
    {
        public int Value1 { get; set; }
        public int Value2 { get; set; }
        public int Add(int x, int y)
        {
            return x + y;
        }
        public B(IA a) { }
        public B(int value1, int value2)
        {
            this.Value1 = value1;
            this.Value2 = value2;
        }

        public void SetValue(int value1, int value2)
        {
            this.Value1 = value1;
            this.Value2 = value2;
        }

    }
}
