using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetArchTest.Rules;

namespace NetSdrClientAppTests
{
    public class ArchitectureTest
    {
        [Test]
        public void UI_InfrastructureDependency()
        {
            var assembly = typeof(NetSdrClientApp.NetSdrClient).Assembly;

            var result = Types.InAssembly(assembly)
                .That()
                .ResideInNamespace("NetSdrClientApp")
                .ShouldNot()
                .HaveDependencyOn("EchoTspServer")
                .GetResult();

            Assert.IsTrue(result.IsSuccessful);
        }
    }
}