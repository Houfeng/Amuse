
namespace Amuse.Models
{
    internal class Property
    {
        public Property(Bean bean)
        {
            this.Bean = bean;
        }
        public virtual Bean Bean { private set; get; }
        public virtual string Name { get; set; }
        public virtual string Ref { get; set; }
        public virtual string Value { get; set; }
        public virtual string Type { get; set; }
    }
}
