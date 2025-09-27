# Gameplay Explanations — Immune Defense TD (Body‑Time)

*A plain‑language, quick‑to‑read manual of every mechanic and component in the prototype.*

---

## How to Play (2‑minute tour)

1. Watch the **24h Clock Ring** (top‑right). Phases rotate: **Sleep → Morning → Post‑meal → Evening → Sleep**.
2. During **Planning Windows** (at Sleep onset & right after a Meal), time slows. **Build/upgrade/sell** now—cheap and safe.
3. Place **Metabolic Hubs** (income) in safe nodes. Build a **Macrophage** at your first choke and a **B‑Cell** behind it.
4. Add **Mast** (slow) and **NET** (snare) to lock the choke. Use **Mucus Wall** to reroute a second lane into it.
5. Use the **T‑Commander** (hero) to kite, Dash, and drop **Interferon** zones on Viruses.
6. Spend **Cytokines** at **Sleep** on **Chrono‑Perks** to adapt. Survive all scheduled **Exposure Events** to win.

---

## Core Loop

1. **Phases Advance** → global multipliers change (economy, speed, repairs).
2. **Exposure Fires** → waves attack on selected lanes.
3. **Planning Window** (slow time) → build/upgrade/sell at best value.
4. **Defend** → towers fire, hero assists; **Inflammation** & **Sepsis** update.
5. **Sleep** → auto‑repairs, partial inflammation clear, **pick 1 Chrono‑Perk**.

**Win**: Complete the 24h schedule (10–12 exposures). **Lose**: **Lymph HQ** destroyed or **Sepsis** reaches max.

---

## Phases (Body‑Time)

* **Morning (Cortisol)**: +20% **ATP** income; enemies +10% **speed**. Combat spike.
* **Post‑meal (Insulin window)**: +40% ATP income for **30s**; opens a **Planning Window**.
* **Evening**: **Repairs −20% ATP**; economy normal.
* **Sleep (Melatonin)**: ATP −20%; enemies −25% speed; **auto‑repair** all structures a bit; **perk** selection.

> Tip: Build economy early (Morning/Post‑meal). Repair in **Evening**. Choose perks at **Sleep**.

---

## Resources

### ATP (Energy / Currency)

* **Gain:** Every **6s** from **Metabolic Hubs**, scaled by phase; small amounts on kills.
* **Spend:** Building, upgrading, repairing.
* **Tips:** Protect Hubs; fix damaged ones in **Evening**; sell inside Planning Windows for **80%** refund (60% otherwise).

### Cytokines (Perk Points)

* **Gain:** **Dendritic Outposts**, elites, boss phases.
* **Spend:** Only at **Sleep**, on **Chrono‑Perks** (costs escalate).
* **Tips:** One good perk can swing two exposures; Outposts pay off mid‑run.

### Inflammation (Risk → Power → Tax)

* **Up:** Heavy fighting, **Fever**; some abilities.
* **Down:** **Sleep** clears a chunk.
* **Effect:** **More damage & higher execute** now, but **−ATP next phase** (−10% per +2 over 4).
* **Manage:** Use **control** (Mast/NET/Wall) to avoid runaway brawls; consider **Treg Control** perk.

### Sepsis (Global Danger)

* **Up:** Breaches (enemies hit tissue/HQ), cluster structure losses.
* **Effect:** **Hard loss** at max, regardless of HQ HP.
* **Manage:** Prevent leaks, repair quickly, use **Emergency Build** tokens to plug holes.

### Blood Flow (Lane Speed)

* **What:** 0.9×–1.2× speed per phase; affects **enemies** and **projectiles**.
* **Read:** Shown on lane HUD/preview; expect faster pushes in **Morning**.

---

## Structures (Towers & Utilities)

*All placed on fixed **Build Nodes**; most have **2–3 tiers** or **branch upgrades**.*

1. **Lymph HQ (Base)**
   **Purpose:** Core you must protect; hero revive point.
   **T2:** +Hero revive speed. **T3:** +Income aura & small heal aura.

2. **Metabolic Hub** *(Economy)*
   **Purpose:** Generates ATP every tick.
   **T2 Branch:** **Mito Park** (+50% income, fragile) **/ Liver Depot** (normal income, armor aura to Hubs).
   **Use:** Build early in safe nodes; repair if damaged.

3. **Macrophage Post** *(Melee DPS + Execute + Marks)*
   **Purpose:** Frontline brawler; executes <15% HP; applies **Mark**.
   **T2:** **Phagosome Forge** (+10% execute) **/ Opsonin Beacon** (+1 Mark on hit).
   **Use:** At chokepoints before traps.

4. **B‑Cell Turret** *(Ranged DPS + Debuff)*
   **Purpose:** Long‑range damage; applies **Neutralize** (−20% enemy DPS, max 2).
   **T2:** **IgG** (single‑target +35% dmg) **/ IgA** (×3 multishot, −15% per arrow).
   **Use:** Cover long straights; pair with Macrophage/Marks.

5. **Mast Cell Trap** *(AoE Slow + Pulse)*
   **Purpose:** Periodic slow aura; **Degranulate** burst; optional **Mark** on pulse.
   **T2:** +slow **duration** **/ +1 Mark per pulse**.
   **Use:** Lock enemies inside a kill zone.

6. **NET Pit** *(Snare)*
   **Purpose:** 2.5s root field (refreshes at event start); optional ROS DoT.
   **T2:** **+Radius** **/ +3 ROS DoT**.
   **Use:** Place on the choke to stop fast pushes.

7. **Mucus Wall** *(Path Control)*
   **Purpose:** Blocks small enemies; slows big; **reroutes** path when placed on wall nodes.
   **T2:** **Tight Junction** (tankier) **/ Cilia Gate** (toggle open/close).
   **Use:** Funnel 2–3 lanes into your best choke.

8. **Dendritic Outpost** *(Tech / Perk Economy)*
   **Purpose:** +1 **Cytokine** at Sleep; tagging elites improves **perk quality** (extra rerolls).
   **Use:** Mid‑run investment that boosts perk power.

9. **Complement Pad** *(Hazard Finisher)*
   **Purpose:** **Lyse** (insta‑kill) **Marked** enemies under a threshold (proc chance & threshold tunable).
   **Use:** Downstream of Mark sources at tight curves.

---

## Hero — T‑Commander

* **Move:** WASD / left‑stick.
* **Chemotaxis Dash:** Short CD; passes through enemies; applies brief **slow**.
* **Primary Attack:** Melee swipe that applies **Mark**.
* **Q — Interferon Shout:** 6s zone; enemies −20% atk speed; **Virus** take +25% damage.
* **E — Adrenal Surge:** +25% attack speed to nearby structures for 5s.
* **R — Apoptosis Strike:** Execute a **Marked** elite <20% HP (boss resistant).
* **Downed:** Revive at HQ or via HQ medic add‑on.

**Play Tips**: Dash through the front of a pack, drop Interferon on Viruses, then kite back to your choke.

---

## Status Effects & Damage Types

* **Opsonization — Mark:** +10% damage taken per stack; enables Complement **Lysis**.
* **Neutralize:** −20% enemy outgoing damage; stacks to 2.
* **Bleed:** True DoT; stacks additively.
* **Infected:** Tag used by some counters (e.g., hero R effectiveness on hosts).
* **Slow:** Movement debuff (effective **cap −70%** after resist).
* **Lyse:** Instant kill when threshold & conditions met.

**Damage Types:** **Phagocytic (Physical)**, **ROS (Chemical)**, **Antibody (Piercing)**, **True**.
*Enemies have different resist profiles—swap sources as needed.*

---

## Enemies & Hazards

* **Bacteria:** Baseline grunt; **Capsule** variant reduces Marks by 1 on hit; may **Divide** on death.
  *Counter:* Macrophage + Marks, Mast slow, Complement near capillaries.
* **Virus:** Fast; can **Infect** host nodes to spawn **Infected Hosts** (tanky adds).
  *Counter:* **Interferon** field + B‑Cell coverage + NET root.
* **Fungi:** Slow, armored; ignores weak walls.
  *Counter:* Sustained DPS (B‑Cell IgG/IgA) + NET root; ROS DoT helps.
* **Biofilm Engineer (Elite):** Projects **300‑HP shields** to bacteria.
  *Counter:* **Mast** pulses to pop shields, then focus fire.
* **Mutator Virus (Boss):** Every **25% HP** rotates immunity (slow‑immune → mark‑immune → +50% physical resist); spawns Infected Hosts; drops Cytokines at phase changes.
  *Counter:* Adapt per phase; time **Interferon**; keep Marks rolling when allowed.
* **Toxin Cloud (Hazard):** Drifting slow‑fire aura for towers.
  *Counter:* **Interferon** clears faster / avoid until it moves.

---

## Exposure Events & Scheduling

* **Types:** **Inhalation** (airway lanes), **Meal** (gut boost + event), **Cut** (skin breach).
* **Scheduling:** 10–12 events placed around the **24h ring** with lane picks and blood‑flow modifiers.
* **Fever Spike (Crisis):** Late‑run; dmg↑, enemy speed↓, ATP income↓; raises **Inflammation** faster but can drop extra **Cytokines** if managed.

**Dynamic Difficulty (soft):** If time‑to‑kill or damage taken trends too high, perk offers bias toward counters (e.g., Th2 or Treg).

---

## Chrono‑Perks (Sleep)

Pick **1 of 2** at **Sleep** (costs escalate):

* **Th1 Tilt:** +10% hero damage; +10% execute threshold.
* **Th2 Tilt:** +15% B‑Cell & Mast potency.
* **Complement Boost:** +5% lysis chance; +2% threshold.
* **Treg Control:** −2 Inflammation now; −10% outgoing dmg next Active phase.
* **Memory Formation:** Random building **+1 tier** for the next **2** exposure events.

---

## Economy, Repairs, Refunds & Tokens

* **Income:** Continuous **ATP ticks** from Hubs (phase‑scaled).
* **Repairs:** Cheapest in **Evening** (−20%); small **auto‑repairs** at **Sleep**.
* **Refunds:** **80%** inside Planning Windows; **60%** otherwise.
* **Emergency Build Tokens:** **2× per run**—instant full‑speed placement during combat.

---

## UI & Readability

* **Clock Ring HUD:** current phase, upcoming **Exposure**/**Meal**/**Sleep** icons; glowing **Planning Window** wedge.
* **Top‑left:** **ATP**. **Top‑right:** **Sepsis** + phase label.
* **Bottom‑center:** **Hero HP** + **Q/E/R** cooldowns.
* **Toasts:** Phase changes, Fever start/end, Perk chosen, Income summaries.

---

## Win/Lose Conditions

* **Win:** Survive all scheduled exposure events in the 24h cycle.
* **Lose:** **Lymph HQ** destroyed **or** **Sepsis** bar reaches max.

---

## Beginner Loadout (When in doubt)

1. **2× Metabolic Hubs** in safe spots.
2. **Macrophage** at first choke; **B‑Cell** covering lane.
3. Add **Mast** then **NET** at the choke.
4. **Mucus Wall** to funnel a second lane into that choke.
5. Mid‑run, place **Dendritic Outpost**; later, add **Complement Pad** near the choke.

---

## Glossary

* **Mark (Opsonization):** A stacking tag that makes enemies take more damage; enables Complement **Lysis**.
* **Neutralize:** Debuff that lowers enemy outgoing damage.
* **Planning Window:** Short slow‑time to safely build/sell/upgrade with better value.
* **Blood Flow:** Global modifier that speeds/slows lanes & projectiles by phase.
