using System;

namespace Курсовая.DataModel
{
    [Serializable]
    public class Transaction
    {
        public string Type { get; set; }
        public DateTime Time { get; set; }
        public double Cash { get; set; }
        public string Currency { get; set; }

        public Transaction(string type, DateTime time, double cash, string currency)
        {
            Type = type;
            Time = time;
            Cash = cash;
            Currency = currency;
        }
    }
}
