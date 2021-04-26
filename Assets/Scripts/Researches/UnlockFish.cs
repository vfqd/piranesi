namespace Researches
{
    public class UnlockFish : Research
    {
        protected override void EffectOnComplete()
        {
            base.EffectOnComplete();
            SelectionHandler.Instance.fisheryButton.gameObject.SetActive(true);
            SelectionHandler.Instance.fisheryButton.GetComponent<BuildingButton>().description = description;
        }
    }
}