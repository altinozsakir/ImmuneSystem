using UnityEngine;
using UnityEngine.UI;
using Core.Meta;


namespace Core.UI
{
public class MetersHUD : MonoBehaviour
{
[Header("Refs")]
public InflammationMeter inflammation;
public SepsisMeter sepsis;
public Image inflFill; public Image sepsFill;
public Image inflBack; public Image sepsBack;


[Header("Lerp")]
public float lerpSpeed = 6f;
float _inflT, _sepsT;


void Start()
{
if (!inflammation) inflammation = FindAnyObjectByType<InflammationMeter>();
if (!sepsis) sepsis = FindAnyObjectByType<SepsisMeter>();
if (inflammation) inflammation.OnChanged += _ => SyncColors();
if (sepsis) sepsis.OnChanged += _ => SyncColors();
SyncColors();
}


void Update()
{
if (inflammation && inflFill)
{
_inflT = Mathf.MoveTowards(_inflT, inflammation.Normalized, Time.unscaledDeltaTime * lerpSpeed);
inflFill.fillAmount = _inflT;
}
if (sepsis && sepsFill)
{
_sepsT = Mathf.MoveTowards(_sepsT, sepsis.Normalized, Time.unscaledDeltaTime * lerpSpeed);
sepsFill.fillAmount = _sepsT;
}
}


void SyncColors()
{
if (inflammation && inflammation.config && inflFill && inflBack)
{
var c = Color.Lerp(inflammation.config.lowColor, inflammation.config.highColor, inflammation.Normalized);
inflFill.color = c; inflBack.color = new Color(c.r, c.g, c.b, 0.25f);
}
if (sepsis && sepsis.config && sepsFill && sepsBack)
{
var c = Color.Lerp(sepsis.config.lowColor, sepsis.config.highColor, sepsis.Normalized);
sepsFill.color = c; sepsBack.color = new Color(c.r, c.g, c.b, 0.25f);
}
}
}
}