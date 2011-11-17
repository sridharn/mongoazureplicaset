## MongoDB Replica Sets on Azure 

## Building
### Prerequisites
  * .Net 4.0.
  * Visual Studio 2010 with SP1 – (currently has been tested with Ultimate Edition)
  * Windows Azure SDK 1.6 
  * Windows Azure Tools for Visual Studio 2010 1.6
  * MongoDB v2.1.0-pre- (currently embedded)
  * MongoDB C# driver v1.4-pre (embedded)

### Build
  * Open MongoDBReplicaSet.sln from Visual Studio 2010 and build

### Running on emulator
  * Running locally on the emulator should work by default
  * The default data dir size is 1GB
  * The default data dir size is 512MB

### Deploying to Azure
  * Edit settings on both the ReplicaSetRole and MvcMovie role to update storage
    * The data dir should use http as the connection mechanism
    * The diagnostics connection string should use https
    * Edit local storage to choose appropriate size

## Maintainers
* Sridhar Nanjundeswaran       sridhar@10gen.com
