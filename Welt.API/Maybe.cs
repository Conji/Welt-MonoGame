﻿#region Copyright
// COPYRIGHT 2016 JUSTIN COX (CONJI)
#endregion

using System;

namespace Welt.API
{
    public struct Maybe<TValue, TException> where TException : Exception
    {
        public TValue Value { get; private set; }
        public TException Error { get; private set; }

        public Maybe(TValue value, TException exception)
        {
            Value = value;
            Error = exception;
            HasError = false;
        } 
        
        public bool HasError { get; private set; }

        public void TrySetValue(Func<TValue> value)
        {
            try
            {
                Value = value.Invoke();
                HasError = false;
            }
            catch (TException e)
            {
                Error = e;
                HasError = true;
            }
            catch (Exception)
            {
                Error = default(TException);
                HasError = true;
            }
        }

        public static Maybe<TValue, TException> Check(Func<TValue> value, Exception exception = null) 
        {
            var maybe = new Maybe<TValue, TException>();
            maybe.TrySetValue(value);
            if (maybe.HasError) maybe.Error = (TException) exception;
            return maybe;
        }

        public static explicit operator TValue(Maybe<TValue, TException> value)
        {
            return value.Value;
        }
    }
}