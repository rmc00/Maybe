using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Maybe.SkipList;
using Xunit;

namespace Maybe.Test.SkipList
{
    public class SkipListTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public void Add_WithValue_ShouldInsertNewNode()
        {
            var list = new SkipList<int>();
            list.Add(42);
            Assert.Single(list);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void AddRange_WithValues_ShouldInsertNewNodeForEachValue()
        {
            var list = new SkipList<int>();
            list.AddRange(new[] { 42, 27, 33});
            Assert.Equal(3, list.Count());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Deserialize_WithValues_ShouldCreateNewListWithSameValues()
        {
            using (var stream = new MemoryStream())
            {
                var list = new SkipList<int>();
                list.AddRange(new[] { 42, 27, 33 });

                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, list);
                stream.Flush();
                stream.Position = 0;
                var newList = (SkipList<int>) formatter.Deserialize(stream);

                Assert.True(newList.Contains(42));
                Assert.True(newList.Contains(27));
                Assert.True(newList.Contains(33));
                Assert.Equal(list.Count(), newList.Count());
            }
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Contains_WithValueInList_ShouldReturnTrue()
        {
            var list = new SkipList<int> {42, 33};
            Assert.True(list.Contains(42));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Contains_WithValueNotInList_ShouldReturnFalse()
        {
            var list = new SkipList<int> { 42, 33 };
            Assert.False(list.Contains(27));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Remove_WhenValueFoundAndRemoved_ShouldReturnTrue()
        {
            var list = new SkipList<int> {42};
            Assert.True(list.Remove(42));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Remove_WhenValueNotFound_ShouldReturnFalse()
        {
            var list = new SkipList<int>();
            Assert.False(list.Remove(42));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Remove_WhenValueFound_ShouldDeleteValueFromList()
        {
            var list = new SkipList<int> {42};
            list.Remove(42);
            Assert.False(list.Contains(42));
        }

        [Fact]
        [Trait("Category", "Unit")]
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
        [Trait("Category", "Unit")]
        public void Enumeration_WithNoSpecifiedComparer_ShouldUseDefaultSortOrder()
        {
            var list = new SkipList<int>{42,27,33};
            var content = list.ToList();
            Assert.Equal(27, content[0]);
            Assert.Equal(33, content[1]);
            Assert.Equal(42, content[2]);
        }

        [Fact]
        [Trait("Category", "Unit")]
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
