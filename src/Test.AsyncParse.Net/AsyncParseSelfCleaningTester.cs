using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AsyncParse.Net.BuiltIns;
using AsyncParse.Net.Model;
using AsyncParse.Net.Service;

namespace Test.AsyncParse.Net
{
    public class AsyncParseSelfCleaningTester<T> : IDisposable where T : ParseObject
    {
        private List<ParseObject> _list = new List<ParseObject>();

        private readonly IParseRegistry<T> _model;
        private Stopwatch _stopwatch;

        public AsyncParseSelfCleaningTester(AsyncParseService parse, string className)
        {
            _model = parse.CreateRegistry<T>(className);
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
        }

        public IParseRegistry<T> Model
        {
            get { return _model; }
        }

        public long Elapsed()
        {
            _stopwatch.Stop();
            return _stopwatch.ElapsedMilliseconds;
        }

        public void Dispose()
        {
            var failed = false;
            failed = !Clean();
            if (failed)
            {
                throw new Exception("Problem deleting objects");
            }
        }

        public bool Clean()
        {
            bool failed = false;
            while (_list.Count > 0)
            {
                var result = _list[0];
                _list.RemoveAt(0);
                try
                {
                    if (_model.Delete(result) == false)
                    {
                        failed = true;
                    }
                }
                catch (Exception)
                {
                    failed = true;
                }
            }
            return !failed;
        }

        public bool SimpleAdd(object value)
        {
            return Model.SimpleAdd(value);
        }

        public void AddObject(ParseObject value)
        {
            _list.Add(value);
        }

        public AsyncCallResult<ParseObject> Add(object value)
        {
            var alpha = Model.Add(value);
            AssertThat.IsTrue(alpha.FailureReason == AsyncCallFailureReason.None);
            AssertThat.IsNotNull(alpha.Contents.objectId);
            AssertThat.IsNotNull(alpha.Contents.createdAt);
            _list.Add(alpha.Contents);
            return alpha;
        }



    }


    [DebuggerStepThrough, DebuggerNonUserCode]
    public class AssertThat
    {
        public static void AreEqual<T>(T expected, T actual)
        {
            if ((IsNull(actual) && !IsNull(expected))
                || (actual.Equals(expected) == false))
            {
                throw new Exception("Expected [" + expected + "] but got [" + actual + "]");
            }

        }

        private static bool IsNull<T>(T actual)
        {
            return Equals(actual, default(T));
        }

        public static void IsTrue(bool actual)
        {
            if (actual == false)
            {
                throw new Exception("Expected <true> but got <false>");
            }
        }

        public static void IsNotNull<T>(T actual)
        {
            if (IsNull(actual))
            {
                throw new Exception("Expected <not_null> but got <null>");
            }
        }
    }

    public class TestPoint : ParseObject
    {
        public string runId { get; set; }
        public string id { get; set; }
        public TestPointProp prop { get; set; }
        public int index { get; set; }
        public DateTime timestamp { get; set; }
        public GeoPoint location { get; set; }
        public Pointer related { get; set; }
        public ParseDate parseDate { get; set; }
    }

    public class TestPointProp : ValueBase
    {
        
    }

}
