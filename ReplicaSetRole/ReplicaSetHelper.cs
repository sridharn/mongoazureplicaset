/*
 * Copyright 2010-2011 10gen Inc.
 * file : ReplicaSetHelper.cs
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

    using Microsoft.WindowsAzure.ServiceRuntime;

    using MongoDB.Bson;
    using MongoDB.Driver;
    using MongoDB.Azure.ReplicaSets.MongoDBHelper;

    using System;

    internal static class ReplicaSetHelper {

        private static int replicaSetRoleCount = 0;
        private static string nodeName = "#d{0}";
        private static string nodeAddress = "{0}:{1}";

        static ReplicaSetHelper() {
            //TODO
            replicaSetRoleCount = 1;
        }

        internal static void RunInitializeCommandLocally(string rsName) {
            var membersDocument = new BsonArray();
            for (int i = 0; i < replicaSetRoleCount; i++) {
                membersDocument.Add(new BsonDocument {
                    {"_id", i},
                    {"host", string.Format(nodeName, i)}
                });
            }
            var cfg = new BsonDocument {
                {"_id", rsName},
                {"members", membersDocument}
            };
            var initCommand = new CommandDocument {
                {"replSetInitiate", cfg}
            };
            var server = MongoDBHelper.GetLocalSlaveOkConnection();
            try {
                var result = server.RunAdminCommand(initCommand);
            } catch {
                // TODO - need to do the right thing here
                // for now do nothing to assume init went through
            }
        }


        internal static void ExecuteCloudCommandLocally(int myId) {
            var nodeDocument = new BsonDocument();
            foreach (var instance in RoleEnvironment.Roles[MongoDBHelper.MongoRoleName].Instances) {
                var endpoint = instance.InstanceEndpoints[MongoDBHelper.MongodPortKey].IPEndpoint;
                nodeDocument.Add(
                    string.Format(nodeName, Utilities.ParseNodeInstanceId(instance.Id)),
                    string.Format(nodeAddress, endpoint.Address, endpoint.Port)
                    );
            }

            var commandDocument = new BsonDocument {
                {"cloud", 1},
                {"nodes", nodeDocument},
                {"me", string.Format(nodeName, myId)}
            };

            var cloudCommand = new CommandDocument(commandDocument);

            var server = MongoDBHelper.GetLocalSlaveOkConnection();
            var result = server.RunAdminCommand(cloudCommand);

        }

        internal static bool ReplicaSetInitialized() {
            try {
                var result = ReplicaSetGetStatus();
                BsonValue startupStatus;
                result.Response.TryGetValue("startupStatus", out startupStatus);
                if (startupStatus != null) {
                    if (startupStatus == 3) {
                        return false;
                    }
                }
                return true;
            } catch {
                return false;
            }
        }

        private static CommandResult ReplicaSetGetStatus() {
            var server = MongoDBHelper.GetLocalSlaveOkConnection();
            var result = server.RunAdminCommand("replSetGetStatus");
            return result;
        }

    }
}
