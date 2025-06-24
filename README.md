![HyperMsg Logo](images/logo.png)

# HyperMsg Core

HyperMsg is a modular messaging framework for .NET, designed to abstract communication protocols and provide core facilities for buffering, serialization, and message dispatching.

## Solution Structure

- **HyperMsg.Core**: Core library with buffering, coding (serialization), and messaging abstractions and implementations.
- **HyperMsg.Hosting**: Extensions for integrating messaging into .NET Generic Host (background services).
- **Test Projects**: Unit tests for core and hosting components.

## Main Components

### 1. Buffering (`HyperMsg.Core.Buffers`)
- `Buffer`: Implements `IBufferReader` and `IBufferWriter` for efficient memory management.
- `IBufferReader` / `IBufferWriter`: Interfaces for reading from and writing to buffers.
- `BufferExtensions`: Extension methods for buffer operations (advance, write, try-write).
- `MemoryReader` / `MemoryWriter`: Delegates for reading/writing memory blocks.

### 2. Coding (Serialization) (`HyperMsg.Core.Coding`)
- `Decoder` / `Encoder`: Delegates for decoding/encoding messages from/to byte buffers.
- `DecodingResult`: Record struct holding a decoded message and the number of bytes decoded.
- `EncodingWriter` / `DecodingReader`: Factories for creating encoding/decoding pipelines.

### 3. Messaging (`HyperMsg.Core.Messaging`)
- `MessageBroker`: Central class implementing `IDispatcher`, `IHandlerRegistry`, and `IMessagingContext`. Manages message handlers and dispatches messages to them.
- `IDispatcher`: Interface for dispatching messages synchronously/asynchronously.
- `IHandlerRegistry`: Interface for registering/unregistering message handlers.
- `IMessagingContext`: Provides access to dispatcher and handler registry.
- `IMessagingComponent`: Interface for attachable/detachable messaging components.
- `ServiceCollectionExtensions`: Extension methods for registering messaging services/components with DI.

### 4. IO (`HyperMsg.Core.IO`)
- `StreamReader` / `StreamWriter`: Factories for creating readers/writers that operate on .NET streams using the buffer abstractions.

### 5. Hosting Integration (`HyperMsg.Hosting`)
- `MessagingWorker`: Background service base class for message-driven workers. Provides hooks for handler registration and heartbeat logic.
- `MessagingExtensions`: Extension method to add messaging worker and context to DI and host.

## Testing

- Unit tests are provided for buffer operations, encoding/decoding, message broker behavior, service registration, and integration with DI.
- Tests use xUnit and FakeItEasy for mocking and assertions.

## Build & CI

- Uses .NET 8.0.
- Includes GitHub Actions workflows for build, test, and package publishing.

---

**In summary:**
HyperMsg provides a lightweight, extensible infrastructure for message-based communication, with a focus on modularity, testability, and integration with .NET's dependency injection and hosting model. The core is built around buffer management, serialization, and a flexible message broker pattern.

![HyperMsg Logo](images/footer.png)