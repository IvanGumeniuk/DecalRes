public class LogoDecalUIController : BaseDecalUIController
{
	public override void OnDecalButtonClick(int index)
	{
		base.OnDecalButtonClick(index);
		decalsUIController.OnCreatingDecal(DecalType.Logo, index, null);
	}
}
