using System;

namespace Курсовая.DataModel
{
    [Serializable]
    public class Account
    {
        public int Id { get; set; }
        public string firstName { get; set; }
        public string secondName { get; set; }

        public string password { get; set; }

        public double dollarBalance { get; set; } = 0;
        public double euroBalance { get; set; } = 0;
        public double hryvniaBalance { get; set; } = 0;


        public string phoneNumber { get; set; }
        public string cardNumber { get; set; }
        public string IconName { get; set; }

        public Account(string fN, string sN, string password, string phoneNumber, int number, string iconName)
        {
            Id = number + 1;
            firstName = fN;
            secondName = sN;
            this.password = password;
            this.phoneNumber = phoneNumber;
            DateTime dt = DateTime.Now;
            cardNumber = "".PadLeft(7 - number.ToString().Length, '7') + $"0{number + 1}{dt.Hour}{dt.Day}{dt.Month.ToString().PadLeft(2, '0')}{dt.Year.ToString().Remove(0, 2)}";
            IconName = iconName;
        }
    }
}
