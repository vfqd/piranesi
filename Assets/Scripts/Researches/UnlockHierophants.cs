namespace Researches
{
    public class UnlockHierophants: Research
    {
        protected override void EffectOnComplete()
        {
            base.EffectOnComplete();
            GameController.Instance.createHierophants = true;
        }
    }
}