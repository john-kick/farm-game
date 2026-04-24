using System;

namespace FarmGame.Scripts.Field.Chunks;

[Serializable]
internal class MissingChunkException : Exception
{
    public MissingChunkException() { }
    public MissingChunkException(string message) : base(message) { }
    public MissingChunkException(string message, Exception innerException) : base(message, innerException) { }
}