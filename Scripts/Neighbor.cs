using Godot;

namespace FarmGame.Tiles
{
	public struct Neighbor<T>
	{
		public T Element;
		public Vector2I Offset;
	}
}
