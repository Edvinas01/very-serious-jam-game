using InSun.GameCore.Messaging;
using NUnit.Framework;

namespace InSun.GameCore.Tests.Editor
{
    internal sealed class SimpleMessageBusTests
    {
        private SimpleMessageBus messageBus;

        [SetUp]
        public void SetUp()
        {
            messageBus = new SimpleMessageBus();
        }

        [Test]
        public void Should_PublishMessage()
        {
            //
            // Given: listener registered for a message type
            //
            TestMessage received = default;
            messageBus.AddListener<TestMessage>(msg => { received = msg; });

            //
            // When: publishing a message
            //
            var message = new TestMessage(67);
            messageBus.PublishMessage(message);

            //
            // Then: listener should receive the message
            //
            Assert.AreEqual(67, received.Value);
        }

        [Test]
        public void Should_PublishMessage_NoListeners()
        {
            //
            // Given: no listeners registered
            //

            //
            // When: publishing
            //

            //
            // Then: publishing should not throw
            //
            Assert.DoesNotThrow(() => messageBus.PublishMessage(new TestMessage()));
        }

        [Test]
        public void Should_PublishMessage_MultipleListeners()
        {
            //
            // Given: multiple listeners
            //
            var callCount = 0;
            messageBus.AddListener<TestMessage>(_ => { callCount++; });
            messageBus.AddListener<TestMessage>(_ => { callCount++; });
            messageBus.AddListener<TestMessage>(_ => { callCount++; });

            //
            // When: publishing a message
            //
            messageBus.PublishMessage(new TestMessage());

            //
            // Then: all listeners should be called
            //
            Assert.AreEqual(3, callCount);
        }

        [Test]
        public void Should_PublishMessage_CorrectType()
        {
            //
            // Given: listeners registered for two different message types
            //
            var messageACount = 0;
            var messageBCount = 0;
            messageBus.AddListener<TestMessageA>(_ => { messageACount++; });
            messageBus.AddListener<TestMessageB>(_ => { messageBCount++; });

            //
            // When: publishing one message
            //
            messageBus.PublishMessage(new TestMessageA());

            //
            // Then: only one listener should be called
            //
            Assert.AreEqual(1, messageACount);
            Assert.AreEqual(0, messageBCount);
        }

        [Test]
        public void Should_RemoveListener()
        {
            //
            // Given: removed listener
            //
            var messageCount = 0;

            messageBus.AddListener<TestMessage>(OnMessage);
            messageBus.RemoveListener<TestMessage>(OnMessage);

            //
            // When: publishing a message
            //
            messageBus.PublishMessage(new TestMessage());

            //
            // Then: removed listener should not be called
            //
            Assert.AreEqual(0, messageCount);

            return;

            void OnMessage(TestMessage _)
            {
                messageCount++;
            }
        }

        [Test]
        public void Should_PublishMessage_ListenerRemovesSelf()
        {
            //
            // Given: listener which removes self
            //
            var messageCount = 0;

            messageBus.AddListener<TestMessage>(OnMessage);
            messageBus.AddListener<TestMessage>(_ => { messageCount++; });

            //
            // When: publishing a message
            //
            messageBus.PublishMessage(new TestMessage());

            //
            // Then: remaining listener should still be called
            //
            Assert.AreEqual(1, messageCount);

            return;

            void OnMessage(TestMessage _)
            {
                messageBus.RemoveListener<TestMessage>(OnMessage);
            }
        }

        private struct TestMessage : IMessage
        {
            public int Value { get; }

            public TestMessage(int value)
            {
                Value = value;
            }
        }

        private struct TestMessageA : IMessage
        {
        }

        private struct TestMessageB : IMessage
        {
        }
    }
}
