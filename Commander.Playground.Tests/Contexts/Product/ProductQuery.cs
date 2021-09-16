using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander.Playground.Tests
{
    public class ProductQuery : Query
    {
        public ProductQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
        public override bool ValidateThis()
        {
            return true;
        }
    }
}
