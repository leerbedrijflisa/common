namespace Lisa.Common.WebApi
{
    public class Error
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public object Values { get; set; }
    }
}