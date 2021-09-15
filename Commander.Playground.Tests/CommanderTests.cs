using Commander.Playground.Tests.Contexts.Category;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander.Playground.Tests
{
    [TestClass]
    public class CommanderTests
    {
        private readonly ICommander _commander;

        public CommanderTests()
        {
            var services = new ServiceCollection();

            services.AddCommander(typeof(Product));

            _commander = services.BuildServiceProvider().GetRequiredService<ICommander>();
        }

        [TestMethod]
        public void TestProductAdd_ShoudReturnSuccess()
        {
            var command = new AddProductCommand
            {
                Name = "notebook dell"
            };

            var result = _commander.Execute<AddProductCommand, Product>(command).Result;

            Assert.IsTrue(result.IsSuccess);
        }
    }
}
