using System.Text.RegularExpressions;

namespace UsageAlert
{
    public class EmailValidation
    {
        public int Result { get; }

        public EmailValidation(string emailHeader, string emailSender, string expectedSender)
        {
            Result = ValidateEmail(emailHeader, emailSender, expectedSender);
        }

        private int ValidateEmail(string emailHeader, string emailSender, string expectedSender)
        {
            var regexheader = new Regex(@"Usage");

            if (regexheader.IsMatch(emailHeader) == true && emailSender == expectedSender)
            {
                return 0;
            }
            else
                return 1;
        }
    }
}
