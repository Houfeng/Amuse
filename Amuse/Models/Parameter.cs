
namespace Amuse.Models
{
    internal class Parameter
    {
        public Parameter(Method method)
        {
            this.Method = method;
        }
        public virtual Method Method { private set; get; }
        public virtual string Name { get; set; }
        public virtual string Ref { get; set; }
        public virtual string Value { get; set; }
        public virtual string Trim { get; set; }
        public virtual string Type { get; set; }
    }
}
