namespace Researches
{
    public class AddResearchPoints: Research
    {
        public int researchPoints;
        
        protected override void EffectOnComplete()
        {
            base.EffectOnComplete();
            ResearchController.Instance.storedResearch += researchPoints;
        }
    }
}