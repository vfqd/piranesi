using DG.Tweening;
using UnityEngine;

namespace Buildings
{
    public class Wharf : MonoBehaviour
    {
        private Building _building;

        public float researchTime;
    
        private void Awake()
        {
            _building = GetComponent<Building>();
            researchTime = 0;
        }

        private void Update()
        {
            if (_building.placed)
            {
                int divisor = _building.acolyteCount;

                if (divisor > 0)
                {
                    float interval = TimeController.Instance.GetLengthOfDayForCalculations() / divisor;

                    researchTime += Time.deltaTime;
                    if (researchTime > interval)
                    {
                        researchTime = 0;
                        ResourcesController.Instance.foodCount++;
                        transform.DOPunchScale(Vector3.one * .1f, .33f, 1, .1f);

                    }
                    GameController.Instance.PlacedAltarDialog();
                }
            }
        }
        
        public int GetFoodProducedCount()
        {
            return _building.acolyteCount;
        }
    }
}