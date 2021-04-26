namespace Researches
{
    public class UnlockTower: Research
    {
        protected override void EffectOnComplete()
        {
            base.EffectOnComplete();
            SelectionHandler.Instance.towerButton.gameObject.SetActive(true);
            SelectionHandler.Instance.towerButton.GetComponent<BuildingButton>().description = description;
        }
    }
}