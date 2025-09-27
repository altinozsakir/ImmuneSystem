# Architecture Map + Live Debug HUD (One‑Pager)

## A) Big‑Picture Architecture (runtime dataflow)

```
                           ┌───────────────────────────────────────────────────────┐
                           │                    Player Input                       │
                           │ (Build/Confirm/Cancel, Move/Dash, Debug hotkeys)     │
                           └───────────────┬───────────────────────────────────────┘
                                           │
┌───────────────────────────── Systems / Meta / Economy ─────────────────────────────┐
│                                                                                   │
│  ┌──────────────┐      OnPhaseChanged   ┌───────────────────────────┐             │
│  │ BodyClock    │ ─────────────────────▶ │   ClockRingHUD (UI)      │             │
│  │  Director    │ ── OnMultipliers ───▶ │   (ring, wedge, pips)     │             │
│  └──────┬───────┘      (atp,spd,rep)    └───────────────────────────┘             │
│         │OnPlanningWindow                                                            │
│         │                                                                             │
│     ┌───▼────────┐     atpPerTick×phase×tax      ┌──────────────────────────┐       │
│     │ Resource   │ ─────────────────────────────▶ │  HUD Text / BankDisplay │       │
│     │   Bank     │ ◀──────── SpendATP ─────────── │  (ATP amount)           │       │
│     └───┬────────┘                               └──────────────────────────┘       │
│         │ OnExternalMultiplierRequest (ATPTaxAdapter)                                │
│  ┌──────▼───────────┐           binds            ┌──────────────────────────┐       │
│  │ Inflammation     │ ─────────────────────────▶ │ GlobalCombatModifiers    │       │
│  │ + MeterConfig    │                           │ (DamageMult/Execute+)    │       │
│  └──────┬───────────┘                           └──────────────────────────┘       │
│         │ Add/Set                                ┌──────────────────────────┐       │
│  ┌──────▼───────────┐  OnDefeat                  │  MetersHUD (UI)         │       │
│  │  Sepsis Meter    │───────────────────────────▶│  (Inflam/Sepsis bars)   │       │
│  └──────────────────┘                            └──────────────────────────┘       │
└───────────────────────────────────────────────────────────────────────────────────┘

┌────────────────────────────── World / Combat / Towers ─────────────────────────────┐
│                                                                                   │
│ ┌───────────┐    spawns    ┌──────────────┐   moves along   ┌──────────────────┐  │
│ │ Spawner   │ ───────────▶ │  Enemy       │ ───────────────▶ │  EnemyMoverSpline│  │
│ └───────────┘              │  (Health,    │                 └──────────────────┘  │
│                            │  Status,     │      speed = base × phase × CC        │
│                            │  CrowdControl│◀──────────────┐                        │
│                            └──────┬───────┘               │ slows/snares           │
│                                   │                       │                        │
│       tower placement              │              ┌───────▼──────────┐              │
│ ┌──────────────────────┐  targets │ closest-to-  │ MastTrapPulse     │              │
│ │ PlacementController  │──────────┼ goal         │  (AoE slow + Mark)│              │
│ └──────────────────────┘          │              └───────┬──────────┘              │
│                                   │                      │ exposure start          │
│                        ┌──────────▼─────────┐            │                        │
│                        │ TowerTargetingGoal │            │                        │
│                        └──────────┬─────────┘    ┌───────▼──────────┐              │
│                                   │ fires       │  NETPitAura       │              │
│                       ┌───────────▼────────┐    │  (snare 2.5s)     │              │
│                       │   TowerShooter     │    └───────────────────┘              │
│                       └───────────┬────────┘                                      │
│                                   │ spawns                                        │
│                    ┌──────────────▼────────────┐                                   │
│                    │   ProjectileHoming        │ --(DamagePacket)--> Health        │
│                    │   (Mark/Neutralize/Exec)  │      │  (resist→mark→HP)          │
│                    └───────────────────────────┘      └─▶ DestroyOnDeath           │
│                                         ▲
│                                         └─ DamagePopupSpawner → DamagePopup (UI)
└───────────────────────────────────────────────────────────────────────────────────┘
```

---
