/*
 * Copyright 2010-2011 10gen Inc.
 * file : MongoDBHelper.cs
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

namespace MongoDB.Azure.ReplicaSets.MongoDBHelper {

    using Microsoft.WindowsAzure.ServiceRuntime;

    using MongoDB.Driver;

    using System;
    using System.Net;
    using System.Text;

    public static class MongoDBHelper {

        public const string MongodPortKey = "MongodPort";
        public const string MongoRoleName = "ReplicaSetRole";

        internal static IPEndPoint GetLocalEndpoint() {
            var currentRoleInstance = RoleEnvironment.CurrentRoleInstance;
            var mongodEndpoint = currentRoleInstance.InstanceEndpoints[MongodPortKey].IPEndpoint;
            return mongodEndpoint;
        }

        public static MongoServer GetLocalConnection() {
            var mongodEndpoint = GetLocalEndpoint();
            var connectionString = new StringBuilder();
            connectionString.Append("mongodb://");
            connectionString.Append(string.Format("localhost:{0}",
                mongodEndpoint.Port));
            var server = MongoServer.Create(connectionString.ToString());
            return server;
        }

        public static MongoServer GetLocalSlaveOkConnection() {
            var mongodEndpoint = GetLocalEndpoint();
            var connectionString = new StringBuilder();
            connectionString.Append("mongodb://");
            connectionString.Append(string.Format("localhost:{0}",
                mongodEndpoint.Port));
            connectionString.Append("/?slaveOk=true");
            var server = MongoServer.Create(connectionString.ToString());
            return server;
        }
    }
}
