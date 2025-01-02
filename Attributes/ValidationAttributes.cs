using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace AngularWithASP.Server.Attributes;

public class ValidationAttributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ValidTransactionAmountAttribute : ValidationAttribute
    {
        public ValidTransactionAmountAttribute(string errorMessage) 
        {
            ErrorMessage = errorMessage;
        }

        public override bool IsValid(object value)
        { 
            if (value == null)
                return false;

            if (value is int amount)
                return amount > 0;


            return false;
        }
    }
}
