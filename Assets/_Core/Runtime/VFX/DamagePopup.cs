// Assets/_Core/Runtime/VFX/DamagePopup.cs
using UnityEngine;
#if TMP_PRESENT || UNITY_TEXTMESHPRO
using TMPro;
#endif

namespace Core.VFX
{
    public class DamagePopup : MonoBehaviour
    {
        [Header("Motion")]
        public float lifetime = 0.9f;
        public float riseSpeed = 1.6f;
        public float fadeStart = 0.4f;
        public Vector3 randomJitter = new Vector3(0.15f, 0.05f, 0f);
        public bool billboardToCamera = true;

        [Header("Colors")]
        public Color damageColor = Color.white;
        public Color hpColor = new Color(0.9f, 1f, 0.9f, 1f);
        public Color executeColor = new Color(1f, 0.9f, 0.2f, 1f);

#if TMP_PRESENT || UNITY_TEXTMESHPRO
        TMP_Text _mainTMP, _subTMP;
#endif
        TextMesh _mainLegacy, _subLegacy;
        float _t;

        public void SetDamageAndHP(float dmg, float hp, float maxHp, bool isExecute)
        {
            string dmgStr = "-" + Mathf.RoundToInt(dmg);
            string hpStr  = Mathf.CeilToInt(hp) + " / " + Mathf.CeilToInt(maxHp);

#if TMP_PRESENT || UNITY_TEXTMESHPRO
            if (!_mainTMP)  _mainTMP  = transform.Find("Main")?.GetComponent<TMP_Text>();
            if (!_subTMP)   _subTMP   = transform.Find("Sub")?.GetComponent<TMP_Text>();
            if (_mainTMP)   { _mainTMP.text = dmgStr; _mainTMP.color = isExecute ? executeColor : damageColor; }
            if (_subTMP)    { _subTMP.text  = hpStr;  _subTMP.color  = hpColor; }
#endif
            if (!_mainLegacy) _mainLegacy = transform.Find("Main")?.GetComponent<TextMesh>();
            if (!_subLegacy)  _subLegacy  = transform.Find("Sub")?.GetComponent<TextMesh>();
            if (_mainLegacy)  { _mainLegacy.text = dmgStr; _mainLegacy.color = isExecute ? executeColor : damageColor; _mainLegacy.characterSize = 0.12f; }
            if (_subLegacy)   { _subLegacy.text  = hpStr;  _subLegacy.color  = hpColor; _subLegacy.characterSize = 0.09f; }

            transform.position += new Vector3(
                Random.Range(-randomJitter.x, randomJitter.x),
                Random.Range(0f, randomJitter.y),
                0f);
        }

        void Update()
        {
            _t += Time.deltaTime;
            transform.position += Vector3.up * riseSpeed * Time.deltaTime;

            float k = 0f;
            if (_t > fadeStart) k = Mathf.InverseLerp(fadeStart, lifetime, _t);

#if TMP_PRESENT || UNITY_TEXTMESHPRO
            if (_mainTMP) { var c = _mainTMP.color; c.a = 1f - k; _mainTMP.color = c; }
            if (_subTMP)  { var c = _subTMP.color;  c.a = 1f - k; _subTMP.color  = c; }
#endif
            if (_mainLegacy) { var c = _mainLegacy.color; c.a = 1f - k; _mainLegacy.color = c; }
            if (_subLegacy)  { var c = _subLegacy.color;  c.a = 1f - k; _subLegacy.color  = c; }

            if (billboardToCamera && Camera.main) transform.forward = Camera.main.transform.forward;
            if (_t >= lifetime) Destroy(gameObject);
        }
    }
}
