#region
// Copyright 2020 the Reko contributors.

// Permission is hereby granted, free of charge, to any person obtaining a copy 
// of this software and associated documentation files (the "Software"), to 
// deal in the Software without restriction, including without limitation the
// 
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
// sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
// THE SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using Xilium.CefGlue;

namespace Reko.Chromely.BrowserHost
{
    /// <summary>
    /// Utility class that converts values to and from CefV8Values based on type.
    /// </summary>
    static class ValueConverter
    {
        public static CefV8Value ConvertToJsValue(object? value)
        {
            if (value is null)
                return CefV8Value.CreateNull();
            return value switch
            {
                CefV8Value v => v,
                string s => CefV8Value.CreateString(s),
                bool b => CefV8Value.CreateBool(b),
                Exception ex => MakeJsRejection(ex),
                _ => throw new NotImplementedException()
            };
        }

        /// <summary>
        /// Make a js object out of an exception.
        /// </summary>
        /// <param name="ex">Exception to convert</param>
        /// <returns>A JS object with a 'message' and a 'trace' property.</returns>
        private static CefV8Value MakeJsRejection(Exception? ex)
        {
            var sbMessage = new StringBuilder();
            var sbTrace = new StringBuilder();
            var sep = "";
            while (ex != null)
            {
                sbMessage.Append(sep);
                sbMessage.Append(ex.Message);
                sep = " ";
                sbTrace.Append(ex.StackTrace);
                sbTrace.AppendLine("---");
                ex = ex.InnerException;
            }
            var obj = CefV8Value.CreateObject();
            obj.SetValue("message", CefV8Value.CreateString(sbMessage.ToString()));
            obj.SetValue("trace", CefV8Value.CreateString(sbTrace.ToString()));
            return obj;
        }

        public static object? ConvertFromJsValue(CefV8Value jsValue)
        {
            //$TODO: file an issue complaining about the lack of a 'int CefV8Value.GetValueType()' method
            // so this totem-pole could be converted to a simpler switch statement/expression.

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

        /// <summary>
        /// Convert an array of JS values to their .NET counterparts.
        /// </summary>
        /// <param name="jsValues">Array of JS values to convert.</param>
        /// <returns>An array of converted .NET objects.</returns>
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
