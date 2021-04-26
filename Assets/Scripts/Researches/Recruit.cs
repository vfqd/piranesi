namespace Researches
{
    public class Recruit : Research
    {
        
        protected override void EffectOnComplete()
        {
            base.EffectOnComplete();
            MapController.Instance.AddNewAcolytes(5);
        }
    }
}