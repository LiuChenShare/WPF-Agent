﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StreamExtended.Network;
using Titanium.Web.Proxy.Models;
using Titanium.Web.Proxy.Shared;

namespace Titanium.Web.Proxy.Http
{
    internal static class HeaderParser
    {
        internal static async Task ReadHeaders(CustomBinaryReader reader, HeaderCollection headerCollection)
        {
            string tmpLine;
            while (!string.IsNullOrEmpty(tmpLine = await reader.ReadLineAsync()))
            {
                var header = tmpLine.Split(ProxyConstants.ColonSplit, 2);
                headerCollection.AddHeader(header[0], header[1]);
            }
        }

        /// <summary>
        /// Increase size of buffer and copy existing content to new buffer
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="size"></param>
        private static void ResizeBuffer(ref byte[] buffer, long size)
        {
            var newBuffer = new byte[size];
            Buffer.BlockCopy(buffer, 0, newBuffer, 0, buffer.Length);
            buffer = newBuffer;
        }
    }
}
