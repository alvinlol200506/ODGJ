# Integration Guide — untuk Tim Art, Audio & UI

Panduan singkat cara **menempelkan visual / suara / UI** ke core dispatch system
**tanpa mengubah atau merusak logic-nya**. Kamu hanya perlu *mendengarkan* event.

> Aturan emas: **JANGAN edit `DispatchManager.cs`**. Cukup bikin script baru yang
> subscribe ke `DispatchEvents`. Kalau scriptmu error, core game tetap jalan.

---

## Pola Dasar (copy-paste template ini)

```csharp
using UnityEngine;
using ODGJ.Dispatch;   // <- wajib, biar kenal event-nya

public class GhostSendAnimation : MonoBehaviour
{
    // 1. Subscribe pas aktif
    private void OnEnable()
    {
        DispatchEvents.OnDispatchStarted  += HandleStarted;
        DispatchEvents.OnDispatchProgress += HandleProgress;
        DispatchEvents.OnDispatchFinished += HandleFinished;
    }

    // 2. WAJIB unsubscribe pas non-aktif (biar nggak error / memory leak)
    private void OnDisable()
    {
        DispatchEvents.OnDispatchStarted  -= HandleStarted;
        DispatchEvents.OnDispatchProgress -= HandleProgress;
        DispatchEvents.OnDispatchFinished -= HandleFinished;
    }

    // 3. Isi reaksimu di sini
    private void HandleStarted(GhostData ghost, RequestData request)
    {
        // contoh: mainkan animasi + SFX kirim hantu
        Debug.Log($"Mainkan animasi kirim {ghost.ghostName}");
    }

    private void HandleProgress(float progress, GhostData ghost, RequestData request)
    {
        // contoh: update loading bar (progress = 0..1)
    }

    private void HandleFinished(DispatchReport report)
    {
        if (report.IsSuccess)
            Debug.Log("Mainkan VFX + SFX SUKSES");
        else
            Debug.Log("Mainkan VFX + SFX GAGAL");
    }
}
```

---

## Event yang Bisa Kamu Dengarkan

| Event | Kapan terjadi | Data yang kamu dapat | Cocok buat |
|-------|---------------|----------------------|------------|
| `OnDispatchStarted` | Hantu mulai dikirim | `GhostData`, `RequestData` | Animasi/SFX "wush" kirim hantu, tutup panel pilih |
| `OnDispatchProgress` | Tiap frame selama dikirim | `float` (0..1), ghost, request | **Loading bar / timer** |
| `OnDispatchFinished` | Dispatch selesai | `DispatchReport` | **Layar hasil**, SFX sukses/gagal, update resource |

### Isi `DispatchReport` (di event Finished)

| Field | Tipe | Arti |
|-------|------|------|
| `Ghost` | `GhostData` | Hantu yang dikirim |
| `Request` | `RequestData` | Orderan yang dikerjakan |
| `SuccessChance` | `float` | Peluang sukses (0..1) — bisa ditampilkan "90%" |
| `IsSuccess` | `bool` | `true` = sukses, `false` = gagal |
| `MoneyDelta` | `int` | Perubahan uang (+reward / −penalty) |

---

## Data yang Bisa Diambil dari SO

**Dari `GhostData`:** `ghostName`, `description`, `portrait` (Sprite — isi gambar hantu di sini),
`stats` (5 stat 1–10).

**Dari `RequestData`:** `clientName`, `requestDescription`, `dispatchDuration`,
`requirements`, `rewardMoney`, `penaltyMoney`.

> **Buat Art:** field `portrait` di `GhostData` itu buat sprite/foto hantu kamu — tinggal
> drag sprite-nya ke slot itu di asset. Belum dipakai logic, jadi aman.

---

## Mau Trigger Dispatch dari Tombol UI Beneran (bukan IMGUI)?

Kalau nanti bikin tombol Unity UI (Canvas), panggil ini di `onClick`:

```csharp
DispatchManager.Instance.TryStartDispatch(ghostYangDipilih, misiYangDipilih);
```

Atau untuk nampilin **preview peluang** sebelum pemain klik kirim:

```csharp
float peluang = DispatchManager.Instance.CalculateSuccessChance(ghost, request);
labelPeluang.text = peluang.ToString("P0");   // contoh: "90%"
```

---

## Checklist Biar Nggak Error

- [ ] Ada `using ODGJ.Dispatch;` di atas script.
- [ ] Subscribe di `OnEnable`, **unsubscribe di `OnDisable`** (jangan lupa yang ini).
- [ ] Jangan edit file di `Core/` atau `Events/` — bikin script sendiri.
- [ ] Kalau butuh data tambahan dari report, minta Alvin tambah field di `DispatchReport`.

Ada yang kurang jelas → tanya **Alvin**.
