namespace Researches
{
    public class UnlockCompound : Research
    {
        protected override void EffectOnComplete()
        {
            base.EffectOnComplete();
            SelectionHandler.Instance.compoundButton.gameObject.SetActive(true);
            SelectionHandler.Instance.compoundButton.GetComponent<BuildingButton>().description = description;
        }
    }
}