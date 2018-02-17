using System;
using Maybe.SkipList;
using Xunit;

namespace Maybe.Test.SkipList
{
    public class NodeTests
    {
        [Fact]
        public void Constructor_WithValue_SetsValueProperty()
        {
            const int value = 42;
            var node = new Node<int>(value, 0);
            Assert.Equal(value, node.Value);
        }
        
        [Fact]
        public void Constructor_WithLevel_ShouldSetupNodesToThatLevel()
        {
            const int level = 3;
            var node = new Node<int>(42, level);
            Assert.Equal(level, node.Next.Length);
        }

        [Fact]
        public void Constructor_WithNegativeLevel_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Node<int>(42, -5));
        }

        [Fact]
        public void HasNextAtLevel_WithNegativeLevel_ShouldThrowArgumentException()
        {
            var node = new Node<int>(42, 2);
            Assert.Throws<ArgumentException>(() => node.HasNextAtLevel(-2));
        }

        [Fact]
        public void HasNextAtLevel_WithLevelGreaterThanNodeLevel_ShouldReturnFalse()
        {
            var node = new Node<int>(42, 1);
            Assert.False(node.HasNextAtLevel(2));
        }

        [Fact]
        public void HasNextAtLevel_WithNodeSetNull_ShouldReturnFalse()
        {
            var node = new Node<int>(42, 1);
            Assert.False(node.HasNextAtLevel(1));
        }

        [Fact]
        public void HasNextAtLevel_WithValidNodeAtIndex_ShouldReturnTrue()
        {
            var node = new Node<int>(42, 2);
            node.Next[1] = new Node<int>(46, 1);
            Assert.True(node.HasNextAtLevel(1));
        }
    }
}
