using UnityEngine;

namespace JW.DungeonSliding
{
    public class ObjectFader : MonoBehaviour
    {
        [SerializeField] private Material _mat;

        public void FadeOut()
        {
            _mat.SetFloat("_TotalAlpha", 0.5f);
        }
        public void FadeIn()
        {
            _mat.SetFloat("_TotalAlpha", 1.0f);
        }
    }
}
