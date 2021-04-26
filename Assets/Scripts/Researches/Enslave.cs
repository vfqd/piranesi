namespace Researches
{
    public class Enslave : Research
    {
        
        protected override void EffectOnComplete()
        {
            base.EffectOnComplete();
            MapController.Instance.AddNewPrisoners(5);
        }
    }
}