namespace FarmGame.Scripts.Field
{
    public partial class MissingTileException : System.Exception
    {
        public MissingTileException() { }
        public MissingTileException(string message) : base(message) { }
        public MissingTileException(string message, System.Exception inner) : base(message, inner) { }
    }
}