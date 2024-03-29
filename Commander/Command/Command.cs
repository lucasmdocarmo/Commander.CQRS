﻿using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander
{
    public abstract class Command: Message
    {
        public ICommandResult Validations { get; set; }

        protected Command()
        {
            Timestamp = DateTime.Now;
        }

        public abstract bool ValidateThis();
    }
}
