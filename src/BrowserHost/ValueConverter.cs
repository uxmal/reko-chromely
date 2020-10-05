using System;
using System.Collections.Generic;
using System.Text;
using Xilium.CefGlue;

namespace Reko.Chromely.BrowserHost
{
    class ValueConverter
    {
        public static CefV8Value ConvertToJsValue(object? value)
        {
            if (value is null)
                return CefV8Value.CreateNull();
            return value switch
            {
                string s => CefV8Value.CreateString(s),
                _ => throw new NotImplementedException()
            };
        }

        public static object? ConvertFromJsValue(CefV8Value jsValue)
        {
            if (jsValue.IsNull)
                return null;
            if (jsValue.IsUndefined)
                return null;
            if (jsValue.IsString)
                return jsValue.GetStringValue();
            if (jsValue.IsInt)
                return jsValue.GetIntValue();
            if (jsValue.IsDouble)
                return jsValue.GetDoubleValue();
            if (jsValue.IsBool)
                return jsValue.GetBoolValue();
            if (jsValue.IsDate)
                return jsValue.GetDateValue();
            if (jsValue.IsArray)
                throw new NotImplementedException("Arrays not handled yet.");
            if (jsValue.IsArrayBuffer)
                throw new NotImplementedException("ArrayBuffers not handled yet.");
            if (jsValue.IsObject)
                throw new NotImplementedException("Objects not handled yet.");
            throw new NotImplementedException($"JS type not handled in {nameof(ValueConverter)}.{nameof(ConvertFromJsValue)}: {jsValue}.");
        }

        public static object?[] ConvertFromJsValues(CefV8Value[] jsValues)
        {
            object?[] result = new object?[jsValues.Length];
            for (int i = 0; i < result.Length; ++i)
            {
                result[i] = ConvertFromJsValue(jsValues[i]);
            }
            return result;
        }
    }
}
