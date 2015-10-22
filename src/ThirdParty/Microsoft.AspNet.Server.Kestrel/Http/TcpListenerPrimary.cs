﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Net;
using Microsoft.AspNet.Server.Kestrel.Infrastructure;
using Microsoft.AspNet.Server.Kestrel.Networking;
using Microsoft.Framework.Logging;

namespace Microsoft.AspNet.Server.Kestrel.Http
{
    /// <summary>
    /// An implementation of <see cref="ListenerPrimary"/> using TCP sockets.
    /// </summary>
    public class TcpListenerPrimary : ListenerPrimary
    {
        public TcpListenerPrimary(ServiceContext serviceContext) : base(serviceContext)
        {
        }

        /// <summary>
        /// Creates the socket used to listen for incoming connections
        /// </summary>
        protected override UvStreamHandle CreateListenSocket(string host, int port)
        {
            var socket = new UvTcpHandle(Log);
            socket.Init(Thread.Loop, Thread.QueueCloseHandle);
            socket.Bind(host, port);
            socket.Listen(Constants.ListenBacklog, ConnectionCallback, this);
            return socket;
        }

        /// <summary>
        /// Handles an incoming connection
        /// </summary>
        /// <param name="listenSocket">Socket being used to listen on</param>
        /// <param name="status">Connection status</param>
        protected override void OnConnection(UvStreamHandle listenSocket, int status)
        {
            var acceptSocket = new UvTcpHandle(Log);
            acceptSocket.Init(Thread.Loop, Thread.QueueCloseHandle);

            try
            {
                listenSocket.Accept(acceptSocket);
            }
            catch (UvException ex)
            {
                Log.LogError("TcpListenerPrimary.OnConnection", ex);
                return;
            }

            DispatchConnection(acceptSocket);
        }
    }
}
