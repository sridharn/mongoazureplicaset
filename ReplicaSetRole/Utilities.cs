﻿/*
 * Copyright 2010-2011 10gen Inc.
 * file : Utilities.cs
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

    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.ServiceRuntime;
    using Microsoft.WindowsAzure.StorageClient;

    using System;

    internal static class Utilities {

        internal static int ParseNodeInstanceId(string id) {
            int instanceIndex = 0;
            if (RoleEnvironment.IsEmulated) {
                int.TryParse(id.Substring(id.LastIndexOf("_") + 1), out instanceIndex);
            } else {
                int.TryParse(id.Substring(id.LastIndexOf(".") + 1), out instanceIndex);
            }
            return instanceIndex;
        }

        internal static string GetMountedPathFromBlob(
            string localCachePath,
            string cloudDir,
            string containerName,
            string blobName,
            int driveSize,
            out CloudDrive mongoDrive) {
                return GetMountedPathFromBlob(
                    localCachePath,
                    cloudDir,
                    containerName,
                    blobName,
                    driveSize,
                    false,
                    null,
                    null,
                    out mongoDrive);
        }

        internal static string GetMountedPathFromBlob(
            string localCachePath,
            string cloudDir,
            string containerName,
            string blobName,
            int driveSize,
            bool fallback,
            CloudDrive fallbackDrive,
            string fallbackDriverLetter,
            out CloudDrive mongoDrive) {

            if (fallback) {
                if (fallbackDrive == null) {
                    throw new ArgumentNullException("fallbackDrive");
                }

                if (string.IsNullOrEmpty(fallbackDriverLetter)) {
                    throw new ArgumentNullException("fallbackDriveLetter");
                }
            }

            DiagnosticsHelper.TraceInformation(string.Format("In mounting cloud drive for dir {0}", cloudDir));

            CloudStorageAccount storageAccount = null;

            if (fallback) {
                try {
                    storageAccount = CloudStorageAccount.FromConfigurationSetting(cloudDir);
                } catch {
                    // case for fallback to data dir for log also
                    DiagnosticsHelper.TraceInformation(string.Format("{0} is not found. using backup", cloudDir));
                    mongoDrive = fallbackDrive;
                    return fallbackDriverLetter;
                }
            } else {
                storageAccount = CloudStorageAccount.FromConfigurationSetting(cloudDir);
            }
            var blobClient = storageAccount.CreateCloudBlobClient();

            DiagnosticsHelper.TraceInformation("Get container");
            // this should be the name of your replset
            var driveContainer = blobClient.GetContainerReference(containerName);

            // create blob container (it has to exist before creating the cloud drive)
            try {
                driveContainer.CreateIfNotExist();
            } catch (Exception e) {
                DiagnosticsHelper.TraceInformation("Exception when creating container");
                DiagnosticsHelper.TraceInformation(e.Message);
                DiagnosticsHelper.TraceInformation(e.StackTrace);
            }

            var mongoBlobUri = blobClient.GetContainerReference(containerName).GetPageBlobReference(blobName).Uri.ToString();
            DiagnosticsHelper.TraceInformation(string.Format("Blob uri obtained {0}", mongoBlobUri));

            // create the cloud drive
            mongoDrive = storageAccount.CreateCloudDrive(mongoBlobUri);
            try {
                mongoDrive.Create(driveSize);
            } catch (Exception e) {
                // exception is thrown if all is well but the drive already exists
                DiagnosticsHelper.TraceInformation("Exception when creating cloud drive. safe to ignore");
                DiagnosticsHelper.TraceInformation(e.Message);
                DiagnosticsHelper.TraceInformation(e.StackTrace);

            }

            DiagnosticsHelper.TraceInformation("Initialize cache");
            var localStorage = RoleEnvironment.GetLocalResource(localCachePath);

            CloudDrive.InitializeCache(localStorage.RootPath.TrimEnd('\\'),
                localStorage.MaximumSizeInMegabytes);

            // mount the drive and get the root path of the drive it's mounted as
            try {
                DiagnosticsHelper.TraceInformation(string.Format("Trying to mount blob as azure drive on {0}",
                    RoleEnvironment.CurrentRoleInstance.Id));
                var driveLetter = mongoDrive.Mount(localStorage.MaximumSizeInMegabytes,
                    DriveMountOptions.None);
                DiagnosticsHelper.TraceInformation(string.Format("Write lock acquired on azure drive, mounted as {0}, on role instance",
                    driveLetter, RoleEnvironment.CurrentRoleInstance.Id));
                return driveLetter;
            } catch (Exception e) {
                DiagnosticsHelper.TraceWarning("could not acquire blob lock.");
                DiagnosticsHelper.TraceWarning(e.Message);
                DiagnosticsHelper.TraceWarning(e.StackTrace);
                throw;
            }
        }

    }
}