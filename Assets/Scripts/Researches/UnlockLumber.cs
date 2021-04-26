namespace Researches
{
    public class UnlockLumber : Research
    {
        protected override void EffectOnComplete()
        {
            base.EffectOnComplete();
            SelectionHandler.Instance.lumbercampButton.gameObject.SetActive(true);
            SelectionHandler.Instance.lumbercampButton.GetComponent<BuildingButton>().description = description;
        }
    }
}