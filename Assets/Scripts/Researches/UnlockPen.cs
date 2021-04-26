namespace Researches
{
    public class UnlockPen: Research
    {
        protected override void EffectOnComplete()
        {
            base.EffectOnComplete();
            SelectionHandler.Instance.penButton.gameObject.SetActive(true);
            SelectionHandler.Instance.penButton.GetComponent<BuildingButton>().description = description;
        }
    }
}