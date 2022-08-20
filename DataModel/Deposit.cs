using System;

namespace Курсовая.DataModel
{
    [Serializable]
    public class Deposit
    {
        public DateTime DateOfCreation { get; set; }
        public string Currency { get; set; }
        public int Term { get; set; }
        public double Amount { get; set; }
        public double Profit { get; set; }


        public Deposit(DateTime dateOfCreation, string currency, int term, double amount, double profit)
        {
            DateOfCreation = dateOfCreation;
            Currency = currency;
            Term = term;
            Amount = amount;
            Profit = profit;
        }
    }
}
