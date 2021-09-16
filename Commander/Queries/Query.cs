using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander
{
    public abstract class Query:Message
    {
        public IQueryResult Validations { get; set; }

        protected Query()
        {
            Timestamp = DateTime.Now;
        }

        public abstract bool ValidateThis();
    }
}
