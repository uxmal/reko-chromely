using Reko.Chromely.BrowserHost.Functions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Xilium.CefGlue;

namespace Reko.Chromely.BrowserHost
{
    /// <summary>
    /// This factory creates <see cref="RekoResourceHandler"/> instances in response to requests from the browser.
    /// </summary>
    public class RekoSchemeHandlerFactory : CefSchemeHandlerFactory
    {
        protected override CefResourceHandler Create(CefBrowser browser, CefFrame frame, string schemeName, CefRequest request)
        {
            var q = new Uri(request.Url).Query;
            var query = HttpUtility.ParseQueryString(q);
            var bytes = Proto_GeneratePng.Generate(Convert.ToInt32(query["percent"]));

            return new RekoResourceHandler(bytes, "image/png");
        }

        private class RekoResourceHandler : CefResourceHandler
        {
            private byte[] blob;
            private string mimeType;
            private int offset;

            public RekoResourceHandler(byte [] blob, string mimeType)
            {
                this.blob = blob;
                this.mimeType = mimeType;
                this.offset = 0;
            }

            protected override void Cancel()
            {
            }

            protected override void GetResponseHeaders(CefResponse response, out long responseLength, out string? redirectUrl)
            {
                response.MimeType = mimeType;
                responseLength = blob.Length;
                redirectUrl = null;
            }

            protected override bool Open(CefRequest request, out bool handleRequest, CefCallback callback)
            {
                handleRequest = true;
                return true;
            }

            protected override bool Read(IntPtr dataOut, int bytesToRead, out int bytesRead, CefResourceReadCallback callback)
            {
                bytesRead = Math.Min(bytesToRead, blob.Length - offset);
                System.Runtime.InteropServices.Marshal.Copy(this.blob, this.offset, dataOut, bytesRead);
                offset += bytesRead;
                return true;
            }

            protected override bool Skip(long bytesToSkip, out long bytesSkipped, CefResourceSkipCallback callback)
            {
                throw new NotImplementedException();
            }
        }
    }
}
