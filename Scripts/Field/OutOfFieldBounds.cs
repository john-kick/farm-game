namespace FarmGame.Scripts.Field
{
    [System.Serializable]
    public class OutOfFieldBoundsException : System.Exception
    {
        public OutOfFieldBoundsException() { }
        public OutOfFieldBoundsException(string message) : base(message) { }
        public OutOfFieldBoundsException(string message, System.Exception inner) : base(message, inner) { }
    }
}