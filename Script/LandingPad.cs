using Godot;

public partial class LandingPad : CsgBox3D
{
	[Export(PropertyHint.File, "*tscn")]
	public string Path;
}
