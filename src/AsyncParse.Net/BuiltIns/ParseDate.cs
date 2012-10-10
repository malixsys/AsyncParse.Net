using System;

namespace AsyncParse.Net.BuiltIns
{
    public class ParseDate
    {
        public DateTime Value { get; set;} 
        
        public static implicit operator ParseDate(DateTime date)
        {
            return new ParseDate {Value = date};
        }
    }
}