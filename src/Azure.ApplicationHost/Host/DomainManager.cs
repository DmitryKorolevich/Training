// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Net.Sockets;
using System.Runtime.Versioning;
using System.Threading;
using Azure.ApplicationHost.Constants;

namespace Azure.ApplicationHost.Host
{
    public static class DnxHostedApplication
    {
        private static readonly Version DefaultFrameworkVersion = new Version(4, 6, 1);

        private static ApplicationMainInfo _info;
        private static FrameworkName _dnxTfm;

        public static int Init()
        {
            _info = new ApplicationMainInfo();
            _dnxTfm = DetermineDefaultFramework();
            return Main(new[] {"run"});
        }

        private static FrameworkName DetermineDefaultFramework()
        {
            return new FrameworkName(FrameworkNames.LongNames.Dnx, DefaultFrameworkVersion);
        }

        private static int Main(string[] argv)
        {
            // Create the socket on a new thread to warm up the configuration stack
            // before any other code starts to run. This allows us to startup up much
            // faster.
            ThreadPool.UnsafeQueueUserWorkItem(_ =>
            {
                var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Dispose();
            }, null);

            return HostEngine.Execute(argv, _dnxTfm, _info);
        }

        public struct ApplicationMainInfo
        {
            public string OperatingSystem;
            public string OsVersion;
            public string Architecture;
            public string RuntimeDirectory;
            public string ApplicationBase;
            public string Framework;
        }
    }
}
