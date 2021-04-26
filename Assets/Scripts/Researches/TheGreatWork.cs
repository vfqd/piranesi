namespace Researches
{
    public class TheGreatWork : Research
    {
        protected override void EffectOnComplete()
        {
            base.EffectOnComplete();
            GameController.Instance.LeaveLondon();
        }
    }
}