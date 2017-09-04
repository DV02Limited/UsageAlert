using System;
using System.Text.RegularExpressions;

namespace UsageAlert
{
    public class SetVariable
    {
        //Set instance variables
        public string Smsnumber { get; }
        public string Customer { get; }
        public string UsageType { get; }
        public string Date { get; }
        public string User { get; }
        public string DataUsage { get; }
        public string MoneyUsage { get; } = "0.00";
        public string SmsTextMonetary { get; }
        public string SmsTextData { get; }

        public SetVariable(string emailBody)
        {
            Smsnumber = CreateSmsNumber(emailBody);
            Customer = CreateCustomer(emailBody);
            UsageType = CreateUsageType(emailBody);
            Date = CreateDate(emailBody);
            User = CreateUser(emailBody);
            DataUsage = CreateDataUsage(emailBody);
            MoneyUsage = CreateMoneyUsage(emailBody);
            SmsTextMonetary = CreateSmsTextMonetary();
            SmsTextData = CreateSmsTextData();
        }

        private string CreateSmsNumber(string emailBody)
        {
            Match match = new Regex("Usage for\\s(.\\d+)").Match(new HtmlToText().ConvertHtml(emailBody)); // Find mobile number in usage alert received from Daisy
            string pattern = "^0";
            string replacement = "44";
            string input = match.Groups[1].ToString();
            string Smsnumber = new Regex(pattern).Replace(input, replacement); // Drop '0' from mobile number and convert into E.164 format
            return Smsnumber;
        }

        private string CreateCustomer(string emailBody)
        {
            Match match = new Regex("Cost Centre:\\s+(.*)] has exceeded").Match(new HtmlToText().ConvertHtml(emailBody)); // Find customer from received Daisy email
            if (match.Success)
                return match.Groups[1].Value; //return customer name back to MiCC Enterprise
            return "Error";
        }

        private string CreateUsageType(string emailBody)
        {
            Match match = new Regex("Usage Alert\\s+(.*) has been triggered").Match(new HtmlToText().ConvertHtml(emailBody)); // Get usage alert type from received Daisy email
            if (match.Success)
                return match.Groups[1].Value; // Send back to MiCC Enterprise if usage alert if either cost or data
            return "Error";
        }

        private string CreateDate(string emailBody)
        {
            Match match = new Regex("triggered on(.*)").Match(new HtmlToText().ConvertHtml(emailBody));
            if (match.Success)
                return match.Groups[1].Value;
            return "Error";
        }

        private string CreateUser(string emailBody)
        {
            Match match = new Regex("\\((.*)\\)").Match(new HtmlToText().ConvertHtml(emailBody)); // Get mobile user name from received Daisy email
            if (match.Success)
                return match.Groups[1].Value;
            return "Error";
        }

        private string CreateDataUsage(string emailBody)
        {
            return new Regex("exceeded\\s(.*)\\sTotal Data").Match(new HtmlToText().ConvertHtml(emailBody)).Groups[1].Value;
        }

        private string CreateMoneyUsage(string emailBody)
        {
            try
            {
                return (Decimal.Parse(new Regex("exceeded\\s£(.\\d*\\..\\d+)").Match(new HtmlToText().ConvertHtml(emailBody)).Groups[1].Value) * new Decimal(2)).ToString();
            }
            catch (FormatException)
            {
                return "0.00";
            }
            
        }

        private string CreateSmsTextMonetary()
        {
            // Create SMS text message for monetary usage alert and return it back to MiCC Enterprise
            return $"Dear Customer, Usage Limit for {this.Smsnumber} has exceeded £{this.MoneyUsage} Total Cost";
        }

        private string CreateSmsTextData()
        {
            // Create SMS text message for data usage alert and return it back to MiCC Enterprise
            return $"Dear Customer, Your data limit of {this.DataUsage}MB has been reached and an Auto-Bar applied";
        }
    }
}
