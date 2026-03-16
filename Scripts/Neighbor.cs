using Godot;

namespace FarmGame.Scripts
{
	public struct Neighbor<T>
	{
		public T Element;
		public Vector2I Offset;
	}
}
