using NUnit.Framework;
using NetSdrClientApp.Networking;
using System.Diagnostics.CodeAnalysis;

namespace NetSdrClientApp.Tests
{
    [TestFixture]
    public class TcpClientWrapperTests
    {
        private string _host = "127.0.0.1";
        private UdpClientWrapper _client1;
        private UdpClientWrapper _client2;

        [SetUp]
        public void Setup()
        {
            _client1 = new UdpClientWrapper(5000);
            _client2 = new UdpClientWrapper(5000);
        }

        [TearDown]
        public void TearDown()
        {
            _client1.Dispose();
            _client2.Dispose();
        }

        [Test]
        public void Constructor_SetsHostAndPort()
        {
            var client = new TcpClientWrapper(_host, 1234);
            Assert.That(client, Is.Not.Null);
        }

        [Test]
        public void GetHashCode_ReturnsConsistentValue()
        {
            int hash1 = _client1.GetHashCode();
            int hash2 = _client1.GetHashCode();
            Assert.That(hash1, Is.EqualTo(hash2));
        }

        [Test]
        public void GetHashCode_DifferentClientsWithSamePort_ReturnsSameHash()
        {
            int hash1 = _client1.GetHashCode();
            int hash2 = _client2.GetHashCode();
            Assert.That(hash1, Is.EqualTo(hash2));
        }

        [Test]
        public void Equals_ReturnsTrue_ForSamePortAndAddress()
        {
            Assert.That(_client1.Equals(_client2), Is.True);
        }

        [Test]
        public void Equals_ReturnsFalse_ForDifferentPort()
        {
            var clientDifferentPort = new UdpClientWrapper(6000);
            Assert.That(_client1.Equals(clientDifferentPort), Is.False);
            clientDifferentPort.Dispose();
        }

        [Test]
        public void Equals_ReturnsFalse_ForDifferentType()
        {
            Assert.That(_client1.Equals("some string"), Is.False);
            Assert.That(_client1.Equals(null), Is.False);
        }

        [Test]
        public void Dispose_SetsDisposedFlag()
        {
            _client1.Dispose();
            // використати рефлексію для перевірки приватного поля _disposed
            var disposedField = typeof(UdpClientWrapper)
                .GetField("_disposed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            bool disposedValue = (bool)disposedField!.GetValue(_client1)!;
            Assert.That(disposedValue, Is.True);
        }

        [Test]
        public void Connected_ReturnsFalse_WhenNotConnected()
        {
            var client = new TcpClientWrapper(_host, 1234);
            Assert.That(client.Connected, Is.False);
        }
    }
}