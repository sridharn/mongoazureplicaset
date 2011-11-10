## MongoDB Replica Sets on Azure ## 

## Building
### Prerequisites
  * .Net 4.0.
  * Visual Studio 2010 with SP1 – (currently has been tested with Ultimate Edition)
  * Windows Azure SDK 1.5 
  * Windows Azure Tools for Visual Studio 2010 1.5
  * MongoDB v2.1.0-pre-
  * MongoDB C# driver v1.3. May need 1.4 (or 1.4-pre)

### Build
  * Open MongoDBReplicaSet.sln from Visual Studio 2010 and build

### Running on emulator
  * Running locally on the emulator should work by default
  * The default log and data dir sizes are 1GB

### Deploying to Azure
  * Create a cloud config (copy the local one)
  * Edit settings on both the ReplicaSetRole and MvcMovie role to update storage
    * The data dir should use http as the connection mechanism
    * The diagnostics connection string should use https

## Maintainers
* Sridhar Nanjundeswaran       sridhar@10gen.com
