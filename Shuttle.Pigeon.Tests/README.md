# Overview

Right-click on the `Shuttle.Pigeon.Tests` project and click on `Manage User Secrets`.

Add the relevant sections from the following content:

```
{
  "Fixtures": {
    "Mail": {
      "Recipient": "{your-address}@domain.com",
      "Sender": "noreply@domain.com"
    }
  }
  "Shuttle": {
    "Pigeon": {
      "Postmark": {
        "ServerToken": "{server-token}"
      }
      "Smtp": {
        "Host": "mail.domain.com",
        "Username": "noreply@domain.com",
        "Password": "<Password>",
        "SenderAddress": "noreply@domain.com",
        "SenderDisplayName": "<SenderDisplayName>"
      }
    }
  }
}
```