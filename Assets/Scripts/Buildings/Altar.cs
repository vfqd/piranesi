using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace Buildings
{
    public class Altar : MonoBehaviour
    {
        private Building _building;

        public Animator animator;
        
        public RuntimeAnimatorController normalIdle;
        public RuntimeAnimatorController sacrificeIdle;

        public Transform hierophantSpawnPoint;
        public Character hierophant;
        
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
                int divisor = _building.acolyteCount+1;
                float interval = TimeController.Instance.GetLengthOfDayForCalculations() / divisor;

                researchTime += Time.deltaTime;
                if (researchTime > interval)
                {
                    researchTime = 0;
                    ResearchController.Instance.IncreaseProgressOfCurrentResearch(GetResearchProducedCount() / divisor);
                    transform.DOPunchScale(Vector3.one * .1f, .33f, 1, .1f);
                }
                GameController.Instance.PlacedAltarDialog();
            }
        }

        public void CreateHierophant()
        {
            if (ResourcesController.Instance.prisonersCount > 0)
            {
                foreach (var building in MapController.Instance.buildings)
                {
                    if (building.type == Building.Type.Pen)
                    {
                        building.RemoveAcolyte();
                        ResourcesController.Instance.prisonersCount--;
                        animator.runtimeAnimatorController = sacrificeIdle;
                        Invoke(nameof(BackToNormal),sacrificeIdle.animationClips[0].length);
                        break;
                    }
                }
            }
        }

        public void BackToNormal()
        {
            animator.runtimeAnimatorController = normalIdle;
            Instantiate(hierophant, hierophantSpawnPoint.position, Quaternion.identity);
            ResourcesController.Instance.hierophantCount++;
        }

        public int GetResearchProducedCount()
        {
            return _building.acolyteCount+1;
        }
    }
}