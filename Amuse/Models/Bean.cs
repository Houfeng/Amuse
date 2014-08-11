using System.Collections.Generic;

namespace Amuse.Models
{
    internal class Bean
    {
        public Bean()
        {
            this.Properties = new List<Property>();
            this.Methods = new List<Method>();
        }

        public virtual string Name { get; set; }

        public virtual string Type { get; set; }

        public virtual string Mode { get; set; }

        public virtual string Group { get; set; }

        public virtual List<Property> Properties { get; set; }

        public virtual List<Method> Methods { get; set; }

        public virtual Constructor Constructor { get; set; }

        public virtual string FactoryMethod { get; set; }
    }
}
