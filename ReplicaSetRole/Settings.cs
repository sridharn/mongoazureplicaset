﻿/*
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

namespace MongoDB.Azure.ReplicaSets.ReplicaSetRole
{

    using System;

    using Microsoft.WindowsAzure.ServiceRuntime;

    internal static class Settings
    {
        #region DO NOT MODIFY
        internal const string MongodDataBlobContainerName = "mongoddatadrive{0}";
        internal const string MongodDataBlobName = "mongoddblob{0}.vhd";
        internal const string MongodLogBlobContainerName = "mongodlogdrive{0}";
        internal const string MongodLogBlobName = "mongodlblob{0}.vhd";

        internal const string MongoCloudDataDir = "MongoDBDataDir";
        internal const string MongoCloudLogDir = "MongoDBLogDir";
        internal const string MongoLocalDataDir = "MongoDBLocalDataDir";
        internal const string MongoLocalLogDir = "MongoDBLocalLogDir";
        internal const string MongoDataDirSize = "MongoDataDirSize";
        internal const string MongoLogDirSize = "MongoLogDirSize";
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

        // Default drive sizes in MB
        internal const int DefaultEmulatedDBDriveSize = 1024;
        internal const int DefaultEmulatedLogDriveSize = 512;
        internal const int DefaultDeployedDBDriveSize = 1024 * 1024;
        internal const int DefaultDeployedLogDriveSize = 512 * 1024;

        #endregion DO NOT MODIFY

        #region Configurable Section
        internal static readonly int MaxDBDriveSize; // in MB
        internal static readonly int MaxLogDriveSize; // in MB

        internal static readonly TimeSpan DiagnosticTransferInterval = TimeSpan.FromMinutes(1);
        internal static readonly TimeSpan PerfCounterTransferInterval = TimeSpan.FromMinutes(15);
        #endregion Configurable Section

        static Settings()
        {           
            if (RoleEnvironment.IsEmulated)
            {
                MaxDBDriveSize = DefaultEmulatedDBDriveSize;
                MaxLogDriveSize = DefaultEmulatedLogDriveSize;
            }
            else
            {
                MaxDBDriveSize = DefaultDeployedDBDriveSize;
                MaxLogDriveSize = DefaultDeployedLogDriveSize;
            }

            string mongoDataDirSize = null; 
            try 
            {
                mongoDataDirSize = RoleEnvironment.GetConfigurationSettingValue(MongoDataDirSize);
            }
            catch (RoleEnvironmentException)
            {
                // setting does not exist use default
            }
            catch (Exception)
            {
                // setting does not exist?
            }
            
            if (!string.IsNullOrEmpty(mongoDataDirSize))
            {
                int.TryParse(mongoDataDirSize, out MaxDBDriveSize);
            }

            string mongoLogDirSize = null;
            try
            {
                mongoLogDirSize = RoleEnvironment.GetConfigurationSettingValue(MongoLogDirSize);
            }
            catch (RoleEnvironmentException)
            {
                // setting does not exist use default
            }
            catch (Exception)
            {
                // setting does not exist?
            }

            if (!string.IsNullOrEmpty(mongoLogDirSize))
            {
                int.TryParse(mongoLogDirSize, out MaxLogDriveSize);
            }

        }

    }
}
