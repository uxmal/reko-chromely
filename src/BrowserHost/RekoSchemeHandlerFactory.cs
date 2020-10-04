using Reko.Chromely.BrowserHost.Functions;
using System;
using System.Collections.Generic;
using System.Text;
using Xilium.CefGlue;

namespace Reko.Chromely.BrowserHost
{
    public class RekoSchemeHandlerFactory : CefSchemeHandlerFactory
    {
        protected override CefResourceHandler Create(CefBrowser browser, CefFrame frame, string schemeName, CefRequest request)
        {
            var bytes = Proto_GeneratePng.Generate();
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
                //DebuggerHelper.Launch();
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
