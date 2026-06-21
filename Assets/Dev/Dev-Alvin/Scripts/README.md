# Dispatch System — Core Architecture (Dukun Santet)

Dokumentasi arsitektur **core game loop** untuk game *Dispatch/Management* bertema Dukun Santet
(GameSeed 2026). Dibuat oleh **Alvin**. Semua kode ada di folder ini
(`Assets/Dev/Dev-Alvin/Scripts/`), namespace **`ODGJ.Dispatch`**.

> **TL;DR untuk programmer lain:** Logic dispatch sudah jadi & decoupled. Kamu nggak perlu
> ngubah `DispatchManager` untuk nambah fitur visual/suara — cukup **subscribe ke event**.
> Lihat `INTEGRATION_GUIDE.md` untuk caranya.

---

## 1. Konsep Gameplay

Pemain = **Dukun**. Alurnya:

1. Datang **orderan gaib** dari klien (`RequestData`).
2. Pemain pilih **hantu** (`GhostData`) — Kuntilanak, Tuyul, dsb — yang punya 5 stat.
3. Hantu **dikirim** (ada delay waktu pengiriman).
4. Sistem menghitung **% sukses** = stat hantu vs syarat misi (+ sedikit RNG).
5. Hasil **sukses/gagal** → uang pemain bertambah/berkurang.

---

## 2. Tiga Pilar Arsitektur

| Pilar | Implementasi | Kenapa |
|-------|--------------|--------|
| **Data-Driven** | ScriptableObject (`GhostData`, `RequestData`) | Designer atur balancing tanpa sentuh kode |
| **Decoupling** | Observer Pattern via `System.Action` (`DispatchEvents`) | Art/Audio nempel tanpa rusak core logic |
| **Simulasi waktu** | Coroutine di `DispatchManager` | Delay pengiriman tanpa nge-block game |

---

## 3. Struktur Folder

```
Scripts/
├─ Data/                      ← Data-Driven (ScriptableObject)
│  ├─ GhostStats.cs           enum GhostStatType + struct GhostStats (5 stat)
│  ├─ GhostData.cs            SO: identitas + stat hantu
│  └─ RequestData.cs          SO: orderan klien + requirement + reward
├─ Events/                    ← Observer Pattern
│  ├─ DispatchEvents.cs       hub event statis (Started / Progress / Finished)
│  └─ DispatchReport.cs       payload hasil dispatch
├─ Core/                      ← Logic
│  ├─ DispatchManager.cs      singleton: timer + calculation engine + broadcast
│  └─ PlayerWallet.cs         contoh subscriber (update uang)
└─ Debug/
   └─ DispatchDebugTester.cs  dummy button (IMGUI) + Debug.Log, TANPA Canvas
```

---

## 4. Alur Data (siapa manggil siapa)

```
  [Pemain klik tombol]
          │
          ▼
  DispatchDebugTester ──► DispatchManager.TryStartDispatch(ghost, request)
                                  │
                                  ├─► raise OnDispatchStarted ........► (Audio/VFX "kirim hantu")
                                  │
                                  │   Coroutine: tunggu dispatchDuration detik
                                  │   └─► raise OnDispatchProgress (tiap frame, 0..1) ► (loading bar)
                                  │
                                  ├─► CalculateSuccessChance() + roll RNG
                                  │
                                  └─► raise OnDispatchFinished(report)
                                              │
                                              ├─► PlayerWallet (update uang)
                                              └─► UI hasil / SFX sukses-gagal (nanti)
```

**Inti decoupling:** `DispatchManager` cuma **menyiarkan** event. Dia TIDAK tahu siapa yang
mendengarkan. UI, VFX, dan Audio tinggal subscribe. Jadi kalau ada yang ngerusak script Audio,
core logic tetap aman.

---

## 5. Penjelasan Tiap Komponen

### `GhostStats.cs`
- `enum GhostStatType { MAG, SEN, CHA, RES, PRA }`
  → Magic, Sense, Charisma, Resilience, Practicality.
- `struct GhostStats` — 5 int (skala 1–10) + method `Get(GhostStatType)` untuk baca stat by enum.

### `GhostData.cs` (ScriptableObject)
- Identitas hantu: `ghostName`, `description`, `portrait` (sprite, buat Art nanti), `stats`.
- Buat asset: **klik kanan di Project → Create → ODGJ → Ghost Data**.

### `RequestData.cs` (ScriptableObject)
- Orderan klien: `clientName`, `requestDescription`, `dispatchDuration` (detik),
  `requirements` (array `StatRequirement`), `rewardMoney`, `penaltyMoney`.
- `StatRequirement` = { stat apa, nilai minimal }.
- Buat asset: **Create → ODGJ → Request Data**.

### `DispatchEvents.cs` (static)
Hub event — lihat tabel di bawah.

### `DispatchManager.cs` (singleton MonoBehaviour)
- `TryStartDispatch(ghost, request)` → mulai dispatch (return `false` kalau hantu lagi sibuk).
- `CalculateSuccessChance(ghost, request)` → **public**, bisa dipakai UI buat *preview* peluang.
- Support **banyak dispatch sekaligus**; 1 hantu tidak bisa dikirim dobel.
- Auto-spawn sendiri kalau belum ada di scene.

### `PlayerWallet.cs`
- Contoh subscriber: dengerin `OnDispatchFinished`, update `Money`. **Tidak** punya referensi
  langsung ke `DispatchManager` — ini bukti pola Observer jalan.

### `DispatchDebugTester.cs`
- Test harness pakai tombol IMGUI (`OnGUI`) → langsung jalan tanpa setup Canvas.

---

## 6. Daftar Event

| Event | Signature | Buat trigger apa |
|-------|-----------|------------------|
| `OnDispatchStarted` | `Action<GhostData, RequestData>` | Animasi/SFX "kirim hantu" |
| `OnDispatchProgress` | `Action<float, GhostData, RequestData>` | Loading bar / timer (float = progress 0..1) |
| `OnDispatchFinished` | `Action<DispatchReport>` | Layar hasil, SFX sukses/gagal, update resource |

`DispatchReport` berisi: `Ghost`, `Request`, `SuccessChance`, `IsSuccess`, `MoneyDelta`.

---

## 7. Calculation Engine (rumus % sukses)

```
Untuk tiap requirement misi:
    kontribusi = clamp01( statHantu / nilaiDibutuhkan )

peluang = rata-rata semua kontribusi
peluang = clamp( peluang, minSuccessChance, maxSuccessChance )   // default 0.05 – 0.95
sukses  = Random.value <= peluang                                // roll RNG
```

**Contoh:** misi butuh `MAG ≥ 10` & `CHA ≥ 5`. Hantu punya `MAG = 8`, `CHA = 5`.
→ kontribusi = `0.8` dan `1.0` → rata-rata `0.9` → peluang sukses **90%**.

Tuning `minSuccessChance`, `maxSuccessChance`, `noRequirementChance` ada di Inspector
`DispatchManager`.

---

## 8. Cara Menjalankan / Tes

1. **Create → ODGJ → Ghost Data** — bikin beberapa hantu, isi stat.
2. **Create → ODGJ → Request Data** — bikin beberapa orderan + requirement.
3. Buat GameObject kosong → tempel `DispatchDebugTester` → isi array `ghosts` & `requests`.
4. **Play** → tombol muncul di kiri-atas Game view → klik **KIRIM HANTU**.
5. Lihat **Console** untuk seluruh proses (START → PROGRESS → FINISHED).

> `DispatchManager` & `PlayerWallet` bisa ditaruh di scene, tapi `DispatchManager` juga
> auto-spawn kalau lupa. Untuk lihat uang berubah, taruh `PlayerWallet` di sebuah GameObject.

---

## 9. Status & Batasan (Sprint 48 Jam)

✅ Sudah ada: data SO, event system, calculation engine, coroutine delay, debug tester.
🚧 Belum (sengaja): UI/Canvas asli, antrian orderan otomatis, save/load, art & audio.

Pertanyaan arsitektur → **Alvin**. Panduan nempel visual/suara → baca `INTEGRATION_GUIDE.md`.
