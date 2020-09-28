public class StripeDecalUIController : BaseDecalUIController
{
	public override void OnDecalButtonClick(int index)
	{
		base.OnDecalButtonClick(index);
		decalsUIController.OnCreatingDecal(DecalType.Stripe, index, null);
	}

}
