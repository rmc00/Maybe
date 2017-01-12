using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maybe.CountMinSketch
{
    // porting from Apache spark: https://github.com/apache/spark/blob/master/common/sketch/src/main/java/org/apache/spark/util/sketch/CountMinSketchImpl.java
    //public class CountMinSketch<T> : CountMinSketchBase<T>
    //{
    //    private const long PRIME_MODULUS = (1L << 31) - 1;

    //    private int _depth;
    //    private int _width;
    //    private long[][] _table;
    //    private long[] _hashA;
    //    private long _totalCount;
    //    private double _epsilon;
    //    private double _confidence;

    //    public CountMinSketch(int depth, int width, int seed)
    //    {
    //        if(depth <= 0) {  throw new ArgumentException("Depth must be a positive integer.", nameof(depth));}
    //        if(width <= 0) {  throw new ArgumentException("Width must be a positive integer.", nameof(width));}

    //        _depth = depth;
    //        _width = width;
    //        _epsilon = 2d/width;
    //        _confidence = 1 - 1/Math.Pow(2, depth);
    //        InitTablesWith(depth, width, seed);
    //    }

    //    public CountMinSketch(double epsilon, double confidence, int seed)
    //    {
    //        if(epsilon <= 0d) { throw new ArgumentException("Relative error must be positive.", nameof(epsilon)); }
    //        if(confidence <= 0d || confidence >= 1d) { throw new ArgumentException("Confidence must be greater than 0 and less than 1", nameof(confidence)); }

    //        _epsilon = epsilon;
    //        _confidence = confidence;
    //        _width = (int) Math.Ceiling(2/epsilon);
    //        _depth = (int) Math.Ceiling(-Math.Log(1 - confidence)/Math.Log(2));
    //        InitTablesWith(_depth, _width, seed);
    //    }

    //    private void InitTablesWith(int depth, int width, int seed)
    //    {
            
    //    }
    //}
}
