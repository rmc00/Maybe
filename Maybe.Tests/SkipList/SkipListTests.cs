using System.Collections.Generic;
using System.Linq;
using Maybe.SkipList;
using Xunit;

namespace Maybe.Tests.SkipList
{
    public class SkipListTests
    {
        [Fact]
        public void Add_WithValue_ShouldInsertNewNode()
        {
            var list = new SkipList<int>();
            list.Add(42);
            Assert.Equal(1, list.Count());
        }

        [Fact]
        public void AddRange_WithValues_ShouldInsertNewNodeForEachValue()
        {
            var list = new SkipList<int>();
            list.AddRange(new[] { 42, 27, 33});
            Assert.Equal(3, list.Count());
        }

        [Fact]
        public void Contains_WithValueInList_ShouldReturnTrue()
        {
            var list = new SkipList<int> {42, 33};
            Assert.True(list.Contains(42));
        }

        [Fact]
        public void Contains_WithValueNotInList_ShouldReturnFalse()
        {
            var list = new SkipList<int> { 42, 33 };
            Assert.False(list.Contains(27));
        }

        [Fact]
        public void Remove_WhenValueFoundAndRemoved_ShouldReturnTrue()
        {
            var list = new SkipList<int> {42};
            Assert.True(list.Remove(42));
        }

        [Fact]
        public void Remove_WhenValueNotFound_ShouldReturnFalse()
        {
            var list = new SkipList<int>();
            Assert.False(list.Remove(42));
        }

        [Fact]
        public void Remove_WhenValueFound_ShouldDeleteValueFromList()
        {
            var list = new SkipList<int> {42};
            list.Remove(42);
            Assert.False(list.Contains(42));
        }

        [Fact]
        public void GetEnumerator_WithValues_ShouldReturnAllValuesAsIEnumerable()
        {
            var list = new SkipList<int> {42, 27, 39};
            using (var ie = list.GetEnumerator())
            {
                var count = 0;
                while (ie.MoveNext()) { count++; }
                Assert.Equal(3, count);
            }
        }

        [Fact]
        public void Enumeration_WithNoSpecifiedComparer_ShouldUseDefaultSortOrder()
        {
            var list = new SkipList<int>{42,27,33};
            var content = list.ToList();
            Assert.Equal(27, content[0]);
            Assert.Equal(33, content[1]);
            Assert.Equal(42, content[2]);
        }

        [Fact]
        public void Enumeration_WithSpecificComparer_ShouldUseCustomSortOrder()
        {
            var list = new SkipList<int>(new MyComparer()) {42,27,33};
            var content = list.ToList();
            Assert.Equal(42, content[0]);
            Assert.Equal(33, content[1]);
            Assert.Equal(27, content[2]);
        }

        private class MyComparer : IComparer<int>
        {
            public int Compare(int x, int y)
            {
                if (x < y) { return 1; }
                if (x > y) { return -1; }
                return 0;
            }
        }
    }
}
