namespace FarmGame.Scripts.Field.Chunks
{
    [System.Serializable]
    public class OutOfChunkBoundsException : System.Exception
    {
        public OutOfChunkBoundsException() { }
        public OutOfChunkBoundsException(string message) : base(message) { }
        public OutOfChunkBoundsException(string message, System.Exception inner) : base(message, inner) { }
    }
}