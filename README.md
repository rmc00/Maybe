# Maybe.NET
[![Build status](https://ci.appveyor.com/api/projects/status/vqsk4kisx1xogmeh?svg=true)](https://ci.appveyor.com/project/rmc00/maybe)

Maybe.NET is a lightweight library of probabilistic data structures for .NET. The library currently features Bloom Filters, Counting Bloom Filters, and Skip Lists. And more data structures are coming soon! Stop scouring the Internet and re-writing the same classes over and over -- use Maybe.NET.

## Installation

Installation is super simple with NuGet! Just use this command to install from the Visual Studio Package Manager Console:

    Install-Package Maybe.NET

## Usage
Maybe.NET has a clear, intuitive API that is easy to pick up. You can check out the Maybe.Tests project for examples of using each method. Here are some quick examples to get you started.

### Bloom Filter Usage
The bloom filter is a handy collection to which you can add items and check if an item is contained in the collection. They are very fast and memory-efficient, but it comes at a small cost: the filter can definitely say if an item is *NOT* in the collection, but it can't say for sure that an item is in the collection, only that it *MIGHT* be. You can use the constructor to specify your targetted maximum rate of errors. (Lower error rates may use more memory)

```
var filter = BloomFilter<int>.Create(50, 0.02);
filter.Add(42);
filter.Add(27);
filter.Add(33);

filter.Contains(55); // returns false (the item is NOT in the collection)
filter.Contains(27); // returns true (the item MIGHT be in the collection)
```

### Counting Bloom Filter Usage
Counting bloom filters extend regular bloom filter functionality by allowing items to be removed from the collection. This can be useful functionality, but it opens the possibility of false negatives.

```
var filter = CountingBloomFilter<int>.Create(50, 0.02);
filter.Add(42);
filter.Contains(42); // returns true
filter.Remove(42);
filter.Contains(42); // returns false
```

### Skip List Usage
Skip lists are sort of like a singly linked list, but they have additional links to other nodes further out in the list. The structure of the links is similar to building Binary Search into the Skip List. However, the Skip List uses randomness to avoid expensive balancing operations when the list is being modified. This structure allows for searching in logarithmic time on average, but doesn't incur the cost of balancing a tree that is normally incurred for fast search. See the [wikipedia article](https://en.wikipedia.org/wiki/Skip_list) for detailed information.

```
var list = new SkipList<int> {42, 33};
list.Contains(42); // returns true
list.Contains(91); // returns false
```

## Contributing
Contributions are always welcome! Please feel free to submit pull requests and to open issues. I prefer to have tests on all public methods if possible and where ever else makes sense.

## License

Free to use under MIT license
