public class LogoDecalUIController : BaseDecalUIController
{
	public override void OnDecalButtonClick(int index)
	{
		decalsUIController.OnCreatingDecal(DecalType.Logo, index, null);
	}
}
