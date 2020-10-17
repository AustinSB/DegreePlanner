using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace DegreePlanner
{
    public static class Validate
    {
        public static void SetMinimumDate(DatePicker start, DatePicker end)
        {
            start.MinimumDate = DateTime.Now;
            end.MinimumDate = DateTime.Now;
        }

        public static bool IsStartDateBeforeEndDate(DatePicker start, DatePicker end)
        {
            if (start.Date < end.Date)
            {
                return true;
            }
            else if (start.Date >= end.Date)
            {
                return false;
            }
            return false;
        }

        public static bool DoesEntryContainText(string text)
        {
            if (String.IsNullOrWhiteSpace(text))
            {
                return false;
            }
            return true;
        }

        public static bool IsPhoneValid(string phone)
        {
            if (phone.Length == 10 && phone.All(c => Char.IsDigit(c)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsEmailValid(string email)
        {
            if (String.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            try
            {
                var address = new System.Net.Mail.MailAddress(email);
                return address.Address == email;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
