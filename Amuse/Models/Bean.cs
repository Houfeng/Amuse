using System.Collections.Generic;

namespace Amuse.Models
{
    internal class Bean
    {
        public Bean()
        {
            this.Properties = new List<Property>();
        }
        public string Name { get; set; }
        public string Class { get; set; }
        public string Mode { get; set; }
        public string Group { get; set; }
        public List<Property> Properties { get; set; }
        public string FactoryMethod { get; set; }
    }
}
