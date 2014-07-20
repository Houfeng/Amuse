namespace Amuse.Demo.Interfaces
{
    public interface IA
    {
        IB B { get; set; }
        int ToResult();
        int Value1 { get; set; }
        int Value2 { get; set; }
    }
}
