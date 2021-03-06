﻿using System;
using System.Text.RegularExpressions;

namespace Payment.Domain
{
    public class ExpiryDate:ValueObject
    {
        private readonly DateTime _value;

        public ExpiryDate(string expiryDateStr)
        {
            _value = ParseExpiryDate(expiryDateStr);

            this.CheckRule(new ExpirationDateShouldBeGreaterThanDatetimeNow(_value));
        }

        private DateTime ParseExpiryDate(string input)
        {
            try
            {
                var regex = new Regex(@"(?<month>\d{2})/(?<year>\d{2})");
                var match = regex.Match(input);
                var year = int.Parse(match.Groups["year"].Value);
                var month = int.Parse(match.Groups["month"].Value);

                int expiryYear = 2000 + year;

                var lastDayOfMonth = DateTime.DaysInMonth(expiryYear, month);
                return new DateTime(expiryYear, month, lastDayOfMonth, 23, 59, 59);
            }
            catch
            {
                throw new ValueObjectException("Invalid card expiry date");
            }

        }
        public override string ToString() => _value.ToString();

        public  DateTime Value => _value;
    }
}