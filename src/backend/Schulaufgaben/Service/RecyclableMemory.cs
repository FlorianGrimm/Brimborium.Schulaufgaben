// MIT - Florian Grimm

namespace Brimborium.Schulaufgaben.Service;

public static class RecyclableMemory {
    private static Microsoft.IO.RecyclableMemoryStreamManager? _Instance;
    public static Microsoft.IO.RecyclableMemoryStreamManager Instance {
        get {
            return _Instance ??= new Microsoft.IO.RecyclableMemoryStreamManager();
        }
    }
}