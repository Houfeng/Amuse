using System.Collections.Generic;

namespace Amuse.Models
{
    internal class Method
    {
        public Method(Bean bean)
        {
            this.Bean = bean;
            this.Parameters = new List<Parameter>();
        }
        public virtual Bean Bean { private set; get; }
        public virtual string Name { get; set; }
        public virtual List<Parameter> Parameters { get; set; }
    }
}
