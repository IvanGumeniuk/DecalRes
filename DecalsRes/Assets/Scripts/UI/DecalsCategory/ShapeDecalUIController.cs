public class ShapeDecalUIController : BaseDecalUIController
{
	public override void OnDecalButtonClick(int index)
	{
		decalsUIController.OnCreatingDecal(DecalType.Shape, index, null);
	}

}
