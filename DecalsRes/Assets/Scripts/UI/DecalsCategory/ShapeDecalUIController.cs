public class ShapeDecalUIController : BaseDecalUIController
{
	public override void OnDecalButtonClick(int index)
	{
		base.OnDecalButtonClick(index);
		decalsUIController.OnCreatingDecal(DecalType.Shape, index, null);
	}

}
