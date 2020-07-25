/// <summary>All view types that will be opened and can be closed by "Back button".  
/// OnClick callback can only take primitives as argument. To know which view was opened you can use this enum.</summary>
public enum SubviewType
{
	None = 0,
	Colors = 1,
	Wraps = 2,
	Decals = 3,
	WindowsLabels = 4,
	Parts = 5,
	Shapes = 6,
	Logos = 7,
	Stickers = 8,
	Stripes = 9,
	CustomPainting = 10
}