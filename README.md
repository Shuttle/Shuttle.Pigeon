# Shuttle.Pigeon

A correspondence sending service.


## Transports

`Shuttle.Pigeon.Server` and `Shuttle.Pigeon.WebApi` register a Hopper transport for each transport configuration section that is present. The transport used for a queue is selected by the scheme of its URI, and the URI host selects the named transport (the child key of the section):

| Transport | Section | URI |
| --- | --- | --- |
| Azure Storage Queues | `Shuttle:Hopper:AzureStorageQueues:{name}` | `azuresq://{name}/{queue}` |
| RabbitMQ | `Shuttle:Hopper:RabbitMQ:{name}` | `rabbitmq://{name}/{queue}` |

At least one transport section is required. For Azure Storage Queues, when neither `StorageAccount` nor `ConnectionString` is set, the connection string named `{name}` is used.

Example using RabbitMQ:

```json
{
  "Shuttle": {
    "Hopper": {
      "RabbitMQ": {
        "local": {
          "Host": "localhost",
          "Username": "shuttle",
          "Password": "shuttle!"
        }
      },
      "Inbox": {
        "WorkTransportUri": "rabbitmq://local/pigeon-server-work",
        "DeferredTransportUri": "rabbitmq://local/pigeon-server-deferred",
        "ErrorTransportUri": "rabbitmq://local/pigeon-server-error"
      }
    }
  }
}
```

The WebApi `MessageRoutes` URIs must point at the same transport as the server's work queue.
