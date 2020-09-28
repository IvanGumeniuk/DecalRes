public class StickerDecalUIController : BaseDecalUIController
{
	public override void OnDecalButtonClick(int index)
	{
		decalsUIController.OnCreatingDecal(DecalType.Sticker, index, null);
	}

}
