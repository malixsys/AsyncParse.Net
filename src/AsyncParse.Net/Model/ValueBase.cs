namespace AsyncParse.Net.Service
{
    public class ValueBase
    {
        public string Value { get; set; }
        public override string ToString()
        {
            return Value;
        }
    }
}