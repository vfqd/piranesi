namespace Researches
{
    public class UnlockMine : Research
    {
        protected override void EffectOnComplete()
        {
            base.EffectOnComplete();
            SelectionHandler.Instance.mineButton.gameObject.SetActive(true);
            SelectionHandler.Instance.mineButton.GetComponent<BuildingButton>().description = description;
        }
    }
}