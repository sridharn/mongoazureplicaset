/*
 * Copyright 2010-2011 10gen Inc.
 * file : Constants.cs
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace MongoDB.Azure.ReplicaSets.ReplicaSetRole {
    using System;

    internal static class Constants {
        #region DO NOT MODIFY
        internal const string MongodDataBlobContainerName = "mongoddatadrive{0}";
        internal const string MongodDataBlobName = "mongoddblob{0}.vhd";
        internal const string MongodLogBlobContainerName = "mongodlogdrive{0}";
        internal const string MongodLogBlobName = "mongodlblob{0}.vhd";

        internal const string MongoCloudDataDir = "MongoDBDataDir";
        internal const string MongoCloudLogDir = "MongoDBLogDir";
        internal const string MongoLocalDataDir = "MongoDBLocalDataDir";
        internal const string MongoLocalLogDir = "MongoDBLocalLogDir";

        internal const string MongoTraceDir = "MongoTraceDir";

        internal const string MongoBinaryFolder = @"approot\MongoDBBinaries";
        internal const string MongoLogFileName = "mongod.log";
        internal const string MongodCommandLineCloud = "--port {0} --dbpath {1} --logpath {2} --nohttpinterface --logappend --replSet {3} ";
        internal const string MongodCommandLineEmulated = "--port {0} --dbpath {1} --logpath {2} --replSet {3} ";


        internal const string TraceLogFileDir = "TraceLogFileDir";
        internal const string MongodDataBlobCacheDir = "MongodDataBlobCacheDir";
        internal const string MongodLogBlobCacheDir = "MongodLogBlobCacheDir";
        internal const string DiagnosticsConnectionString = "DiagnosticsConnectionString";

        internal const string TraceLogFile = "ReplicaSetWorkerTrace.log";

        internal const string ReplicaSetNameSetting = "ReplicaSetName";
        #endregion DO NOT MODIFY

        #region Configurable Section
        internal const int MaxDBDriveSize = 1 * 1024;
        internal const int MaxLogDriveSize = 1024; // in MB
        internal const int MountSleep = 30 * 1000; // 30 seconds;

        internal static readonly TimeSpan DiagnosticTransferInterval = TimeSpan.FromMinutes(30);
        internal static readonly TimeSpan PerfCounterTransferInterval = TimeSpan.FromMinutes(15);
        #endregion Configurable Section

    }
}
