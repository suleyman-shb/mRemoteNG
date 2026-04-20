using mRemoteNG.Connection;
using mRemoteNG.Container;
using NUnit.Framework;

namespace mRemoteNGTests.Connection
{
    [TestFixture]
    public class ConnectionFrameColorTests
    {
        [Test]
        public void ConnectionFrameColorIsInherited()
        {
            var parent = new ContainerInfo
            {
                ConnectionFrameColor = ConnectionFrameColor.Red
            };

            var child = new ConnectionInfo
            {
                Inheritance = { ConnectionFrameColor = true }
            };

            parent.AddChild(child);

            Assert.That(child.ConnectionFrameColor, Is.EqualTo(ConnectionFrameColor.Red));
        }

        [Test]
        public void ConnectionFrameColorIsNotInheritedWhenDisabled()
        {
            var parent = new ContainerInfo
            {
                ConnectionFrameColor = ConnectionFrameColor.Red
            };

            var child = new ConnectionInfo
            {
                ConnectionFrameColor = ConnectionFrameColor.Blue,
                Inheritance = { ConnectionFrameColor = false }
            };

            parent.AddChild(child);

            Assert.That(child.ConnectionFrameColor, Is.EqualTo(ConnectionFrameColor.Blue));
        }

        [Test]
        public void ConnectionFrameColorIsCloned()
        {
            var original = new ConnectionInfo
            {
                ConnectionFrameColor = ConnectionFrameColor.Purple
            };

            var cloned = original.Clone();

            Assert.That(cloned.ConnectionFrameColor, Is.EqualTo(ConnectionFrameColor.Purple));
        }
    }
}
