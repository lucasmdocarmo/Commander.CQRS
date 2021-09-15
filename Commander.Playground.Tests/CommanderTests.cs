using Commander.Playground.Tests.Contexts.Category;
using Commander.Playground.Tests.Contexts.Handlers;
using Commander.Playground.Tests.Contexts.Product.Events;
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

            services.AddCommander<AddProductCommand>();
            services.AddCommander<ProductAddedEvent>();
            services.AddCommander<ProductDeletedEvent>();

            _commander = services.BuildServiceProvider().GetRequiredService<ICommander>();
        }

        [TestMethod]
        public void TestProductAdd_ShoudReturnSuccess()
        {
            var command = new AddProductCommand
            {
                Name = "notebook dell"
            };

            var validateCommand = command.ValidateThis();
            if(!validateCommand)
            {
                CommandResult.IsFailed(command.Validations.Message);
            }
            var result = _commander.Execute<AddProductCommand, Product>(command).Result;

            Assert.IsTrue(result.IsSuccess);
        }
        [TestMethod]
        public async Task TestProductAddEvent_ShouldReturnTre()
        {
            var @event = new ProductAddedEvent("teste");
            var result = await _commander.Publish(@event).ConfigureAwait(false);

            Assert.IsTrue(result.IsSuccess);
        }
        [TestMethod]
        public async Task TestDeleteAddEvent_ShouldReturnTre()
        {
            var @event = new ProductDeletedEvent(Guid.NewGuid());
            var result = await _commander.Publish(@event).ConfigureAwait(false);

            Assert.IsTrue(result.IsSuccess);
        }
    }
}
