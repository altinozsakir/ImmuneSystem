// Assets/_Core/Runtime/UI/HealthNumberBillboard.cs
using UnityEngine;
#if TMP_PRESENT || UNITY_TEXTMESHPRO
using TMPro;
#endif

namespace Core.UI
{
    public class HealthNumberBillboard : MonoBehaviour
    {
        public Core.Combat.Health health;
        public Vector3 worldOffset = new Vector3(0, 1.2f, 0);
#if TMP_PRESENT || UNITY_TEXTMESHPRO
        TMP_Text _tmp;
#endif
        TextMesh _legacy;

        void Awake()
        {
            if (!health) health = GetComponentInParent<Core.Combat.Health>();
#if TMP_PRESENT || UNITY_TEXTMESHPRO
            _tmp = GetComponent<TMP_Text>();
#endif
            _legacy = GetComponent<TextMesh>();
        }

        void LateUpdate()
        {
            if (!health) return;
            Vector3 pos = health.transform.position + worldOffset;
            transform.position = pos;
            if (Camera.main) transform.forward = Camera.main.transform.forward;

            int cur = Mathf.CeilToInt(Mathf.Max(0f, health.Current));
#if TMP_PRESENT || UNITY_TEXTMESHPRO
            if (_tmp) { _tmp.text = cur.ToString(); }
#endif
            if (_legacy) { _legacy.text = cur.ToString(); }
        }
    }
}
