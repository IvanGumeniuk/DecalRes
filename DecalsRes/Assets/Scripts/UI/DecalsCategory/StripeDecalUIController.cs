public class StripeDecalUIController : BaseDecalUIController
{
	public override void OnDecalButtonClick(int index)
	{
		decalsUIController.OnCreatingDecal(DecalType.Stripe, index, null);
	}

}
