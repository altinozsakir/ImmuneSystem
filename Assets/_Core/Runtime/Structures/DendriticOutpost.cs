using System.Collections.Generic; using UnityEngine;
using Core.TimeSystem; using Core.Economy; using Core.Enemies; using Core.Perks;


namespace Core.Structures
{
public class DendriticOutpost : MonoBehaviour
{
public BodyClockDirector clock; public ResourceBank bank; public PerkManager perks;
[Header("Tagging")]
public float tagRadius = 6f; public LayerMask enemyMask; public int maxTagsPerCycle = 2; public float scanInterval = 0.2f;
[Header("Sleep Bonus")] public int cytokinesOnSleep = 1;


readonly HashSet<Transform> _taggedThisCycle = new();
float _scan; Collider[] _buf = new Collider[32];


void Awake(){ if (!clock) clock = FindAnyObjectByType<BodyClockDirector>(); if (!bank) bank = FindAnyObjectByType<ResourceBank>(); if (!perks) perks = FindAnyObjectByType<PerkManager>(); }
void OnEnable(){ if (clock) clock.OnPhaseChanged += OnPhaseChanged; }
void OnDisable(){ if (clock) clock.OnPhaseChanged -= OnPhaseChanged; }


void Update()
{
_scan -= Time.deltaTime; if (_scan > 0f) return; _scan = scanInterval;
int hits = Physics.OverlapSphereNonAlloc(transform.position, tagRadius, _buf, enemyMask, QueryTriggerInteraction.Ignore);
for (int i = 0; i < hits; i++)
{
var tr = _buf[i].attachedRigidbody ? _buf[i].attachedRigidbody.transform : _buf[i].transform;
if (!tr || !tr.gameObject.activeInHierarchy) continue;
if (_taggedThisCycle.Count >= maxTagsPerCycle) break;
if (tr.GetComponent<EnemyElite>()) _taggedThisCycle.Add(tr);
}
}


void OnPhaseChanged(BodyPhase p)
{
if (p != BodyPhase.Sleep) return;
// Grant Cytokines
if (bank && cytokinesOnSleep > 0) bank.AddCytokines(cytokinesOnSleep);
// Reroll charges equal to elite tags (cap via maxTagsPerCycle)
if (perks) perks.AddRerollCharges(Mathf.Min(maxTagsPerCycle, _taggedThisCycle.Count));
_taggedThisCycle.Clear();
}


void OnDrawGizmosSelected(){ Gizmos.color=new Color(1f,0.8f,0.3f,0.4f); Gizmos.DrawWireSphere(transform.position, tagRadius);}
}
}