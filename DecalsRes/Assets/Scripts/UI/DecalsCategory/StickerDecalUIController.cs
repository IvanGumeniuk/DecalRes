public class StickerDecalUIController : BaseDecalUIController
{
	public override void OnDecalButtonClick(int index)
	{
		base.OnDecalButtonClick(index);
		decalsUIController.OnCreatingDecal(DecalType.Sticker, index, null);
	}

}
