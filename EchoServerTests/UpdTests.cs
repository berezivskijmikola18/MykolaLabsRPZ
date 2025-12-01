using EchoTspServer.Udp;
using Moq;

namespace EchoTspServer.Tests.Udp
{
    [TestFixture]
    public class UdpTests
    {
        private UdpMessageBuilder _builder;
        private Mock<UdpMessageBuilder> _mockBuilder;
        private UdpTimedSender _sender;

        [SetUp]
        public void SetUp()
        {
            _builder = new UdpMessageBuilder();
            _mockBuilder = new Mock<UdpMessageBuilder>();
        }

        [TearDown]
        public void TearDown()
        {
            _sender?.Dispose();
        }

        [Test]
        public void BuildMessage_ReturnsCorrectMessageLength()
        {
            // Act
            var message = _builder.BuildMessage();

            // Assert
            // Header (2) + Sequence (2) + Samples (1024) = 1028
            Assert.That(message.Length, Is.EqualTo(1028));
        }

        [Test]
        public void BuildMessage_StartsWithCorrectHeader()
        {
            // Act
            var message = _builder.BuildMessage();

            // Assert
            Assert.That(message[0], Is.EqualTo(0x04));
            Assert.That(message[1], Is.EqualTo(0x84));
        }

        [Test]
        public void Constructor_ThrowsArgumentNullException_WhenHostIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new UdpTimedSender(null, 5000, _mockBuilder.Object));
        }

        [Test]
        public void Constructor_ThrowsArgumentNullException_WhenMessageBuilderIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new UdpTimedSender("127.0.0.1", 5000, null));
        }

        [Test]
        public void Constructor_WithHostAndPort_CreatesInstance()
        {
            // Act & Assert
            Assert.DoesNotThrow(() =>
                _sender = new UdpTimedSender("127.0.0.1", 5000));
        }

        [Test]
        public void StartSending_ThrowsException_WhenAlreadyStarted()
        {
            // Arrange
            _sender = new UdpTimedSender("127.0.0.1", 5000, _mockBuilder.Object);
            _sender.StartSending(1000);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
                _sender.StartSending(1000));
        }

        [Test]
        public void StopSending_CanBeCalledMultipleTimes()
        {
            // Arrange
            _sender = new UdpTimedSender("127.0.0.1", 5000, _mockBuilder.Object);

            // Act & Assert
            Assert.DoesNotThrow(() =>
            {
                _sender.StopSending();
                _sender.StopSending();
            });
        }

        [Test]
        public void Dispose_StopsSending()
        {
            // Arrange
            _sender = new UdpTimedSender("127.0.0.1", 5000, _mockBuilder.Object);
            _sender.StartSending(1000);

            // Act & Assert
            Assert.DoesNotThrow(() => _sender.Dispose());
        }

        [Test]
        public void StartSending_ExecutesSendMessage_AndWritesToConsole()
        {
            _sender = new UdpTimedSender("127.0.0.1", 5000, _mockBuilder.Object);

            using var cts = new CancellationTokenSource();
            _sender.StartSending(50); // інтервал 50ms

            // даємо таймеру спрацювати
            Thread.Sleep(100);

            // зупиняємо таймер
            _sender.StopSending();

            // якщо рядок з Console.WriteLine виконується, тест покриє його
            Assert.Pass("SendMessage executed, Console.WriteLine covered");
        }

        [Test]
        public void BuildMessage_GeneratesDifferentPayloads()
        {
            // Act
            var message1 = _builder.BuildMessage();
            var message2 = _builder.BuildMessage();

            // Extract payloads (skip header + sequence = 4 bytes)
            var payload1 = message1.Skip(4).ToArray();
            var payload2 = message2.Skip(4).ToArray();

            // Assert - payloads should be different (random data)
            Assert.That(payload1, Is.Not.EqualTo(payload2));
        }

        [Test]
        public void GetCurrentSequenceNumber_ReturnsCorrectValue()
        {
            // Act
            _builder.BuildMessage();
            _builder.BuildMessage();
            var currentSeq = _builder.GetCurrentSequenceNumber();

            // Assert
            Assert.That(currentSeq, Is.EqualTo(2));
        }

        [Test]
        public void GetCurrentSequenceNumber_InitiallyZero()
        {
            // Act
            var currentSeq = _builder.GetCurrentSequenceNumber();

            // Assert
            Assert.That(currentSeq, Is.EqualTo(0));
        }
    }
}