﻿using System.Reflection;
using Alisha;
using Newtonsoft.Json.Serialization;

namespace Alisha.UpdateWatcher.Converter
{
    public class NullToEmptyStringValueProvider : IValueProvider
    {
        readonly PropertyInfo _MemberInfo;
        public NullToEmptyStringValueProvider(PropertyInfo memberInfo)
        {
            _MemberInfo = memberInfo;
        }

        public object GetValue(object target)
        {
            object result = _MemberInfo.GetValue(target);
            if (_MemberInfo.PropertyType == typeof(string) && result == null) result = "";
            return result;

        }

        public void SetValue(object target, object value)
        {
            _MemberInfo.SetValue(target, value);
        }
    }
}