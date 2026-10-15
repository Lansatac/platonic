# Platonic

**Mission:** make data's state something you describe once and observe cheaply, so UI, logic, networking, and saves all stay correct without custom wiring.

Platonic is a reactive data-binding library for Unity. State lives in small versioned values. You derive new values from them, and observers react only when something actually changed.

## The problem

Game state is read by many consumers: HUDs, tooltips, AI, networking, save/load. The usual ways of keeping them in sync each fail in their own way.

| Approach | What goes wrong |
|---|---|
| **Events** | Subscriptions must be balanced with unsubscriptions. A missed event leaves the UI stale. A screen that opens late never saw the event that already fired. |
| **Polling and re-reading** | Simple, but you re-read and re-format everything every frame, whether or not it changed. |
| **Hand-written refresh code** | Derived values ("max HP = base + gear + buffs") get recomputed in several places, and the copies drift. |
| **Per-screen glue** | Each new screen needs custom code that knows the shape of your model. |

What we want is simpler than any of these: *tell me whether this is different from the last time I looked.*

## The answer: every value has a version

Every value that can change carries a `ulong` version. An observer remembers the last version it saw and acts only when the number moves:

```csharp
ulong _seen;

void Update()
{
    if (items.Version != _seen)
    {
        _seen = items.Version;
        Refresh();
    }
}
```

Comparing two numbers replaces events, diffing, and dirty flags. There is nothing to unsubscribe, and it stays correct no matter how many places mutate the data. Writing the same value twice bumps the version once, so no-op writes never wake observers.

Data is a bag of named, typed fields. Names are generated from a `FieldNames` asset:

```csharp
int health = data.Get_Health();                    // generated extension method
data.GetField(Names.Health).Value = health - 10;   // bumps the field's version
```

## What you get

### Derived values that stay correct

Describe a computed value once, in terms of its sources. It is lazy and cached, recomputes only when a source version moves, and its own version changes only when the result does:

```csharp
IField<int> maxHealth = Names.MaxHealth.From(baseHealth, modifiers, level,
    (b, m, l) => ComputeMaxHealth(b, m, l));
```

Derived values are versioned too, so they compose into other derived values.

### Pointer-following that tracks both ends

"Show the health of whichever unit is selected" changes when the selection changes *or* when that unit's health changes. `Lookup` tracks both:

```csharp
IVersionedValue<int> selectedHealth = Versioned.Lookup(
    selectedId,                             // IVersionedValue<UnitId>
    id => units.GetHealthField(id),         // UnitId -> IVersionedValue<int>?
    defaultValue: 0);
```

### One-line UI bindings

UI Toolkit controls bind in a single call and update only when the source version moves:

```csharp
label.BindText(data, Names.Name);
healthBar.BindProgressBar(healthFraction);
panel.BindDisplay(isVisible);
buyButton.BindEnabled(canAfford);
```

For uGUI, renderer components such as `TextRenderer` do the same from the inspector: give one a data source, a format string (`{0} / {1}`), and the fields to watch.

### Versioned collections

`VersionedList<T>` and `VersionedDictionary<K, V>` bump their version when membership changes. Items carry their own versions, so editing one item does not trigger observers of the list.

### Generic, saveable data

Because data is a bag of named fields, one binding or renderer works on any data that has the field. Fields serialize by a stable ID derived from their namespaced name, not by object layout.

## Design rules

- **Version scope matches observable scope.** Independently observable things get independent versions. A child's change must not bleed into its parent's version.
- **Compose peers, not descendants.** Combine versions of things at the same level.
- **Prefer `From` and `Lookup` to `Calculate`.** `Versioned.Calculate` wraps non-versioned state but re-evaluates on every access. Use it as a last resort, and only for cheap checks.
- **Field names are serialization keys.** They cannot be renamed once saves exist. Prefer generic names (`Count`, `Index`) so shared UI and logic can reuse them.

## Install

Platonic is a Unity package (`Packages/platonic`). It has no hard dependencies. The TextMeshPro renderers (`Runtime/TextMeshPro`) compile only when TextMeshPro is installed, and the localized renderers only when Unity Localization is installed. The UI Toolkit bindings need neither.

For the full API (versioning rules, all `Lookup` variants, the field-name registry, code generator details), see `docs/systems/platonic.md` in the host project.
