# Save System & Audio Manager

Dua sistem tambahan buat core game. Semua di namespace `ODGJ.Dispatch`.
Dibuat oleh **Alvin**.

---

## 💾 Save System

Yang di-save (untuk sekarang, sesuai permintaan ketua): **uang pemain** + **daftar hantu yang sudah unlock**. Itu saja dulu.

**Kenapa PlayerPrefs, bukan file JSON?** Karena jalan di semua platform termasuk **WebGL**
(save-to-file bermasalah di WebGL) — penting kalau kita submit versi browser ke itch.io.

### File terkait

| File | Peran |
|------|-------|
| `Core/SaveSystem.cs` | Lapisan persistence terpusat (satu-satunya yang sentuh PlayerPrefs) |
| `Core/PlayerWallet.cs` | Pegang uang runtime, load/save uang lewat `SaveSystem` |
| `Core/GhostUnlockManager.cs` | Pegang daftar unlock runtime, load/save lewat `SaveSystem` |
| `Data/GhostData.cs` | Ditambah field `ghostId` (ID stabil buat save/load) |

### Data yang disimpan (2 key PlayerPrefs terpisah)

| Key | Isi | Contoh |
|-----|-----|--------|
| `PlayerGold` | `int` uang pemain | `500` |
| `UnlockedGhosts` | JSON list ID hantu | `{"ids":["kuntilanak","tuyul"]}` |

> Key dipisah supaya `PlayerWallet` (uang) & `GhostUnlockManager` (unlock) bisa nulis
> masing-masing tanpa saling timpa.

### ⚠️ Penting: isi `Ghost Id` di tiap asset hantu

`GhostData` sekarang punya field **`ghostId`**. Ini ID unik & **STABIL** untuk save/load.
- Isi dengan string kecil unik, misal `kuntilanak`, `tuyul`, `pocong`.
- **Jangan diubah** setelah dipakai (kalau berubah, hantu dianggap "belum unlock" lagi).
- Kalau dikosongkan → otomatis pakai `ghostName` (kurang aman kalau nama diedit).

### Cara pakai

```csharp
// Cek / buka unlock
bool punya = GhostUnlockManager.Instance.IsUnlocked(ghost);
GhostUnlockManager.Instance.Unlock(ghost);          // otomatis ke-save

// Ambil hanya hantu yang sudah terbuka (buat isi menu pilih hantu)
var tersedia = GhostUnlockManager.Instance.GetUnlocked(semuaHantu);

// Uang
int uang = PlayerWallet.Instance.Money;
PlayerWallet.Instance.AddMoney(100);
PlayerWallet.Instance.TrySpend(50);

// Reset semua save (debug)
SaveSystem.ClearAll();
```

### Setup di scene
1. Taruh `PlayerWallet`, `GhostUnlockManager` (dan `AudioManager`) di sebuah GameObject
   di scene pertama. Ketiganya `DontDestroyOnLoad`, jadi cukup sekali.
2. Di `GhostUnlockManager`, isi list **Default Unlocked** dengan hantu starter
   (yang otomatis terbuka buat pemain baru).

### Event

`GhostUnlockManager.OnGhostUnlocked` (`Action<GhostData>`) — subscribe untuk munculin
notifikasi / SFX "hantu baru terbuka".

---

## 🔊 Audio Manager

`Audio/AudioManager.cs` — singleton (DDOL), dua channel: **BGM** (loop) & **SFX** (one-shot).

**Nyambung otomatis ke gameplay** lewat Observer Pattern: dia dengerin `DispatchEvents`
lalu mainkan SFX yang pas — `DispatchManager` sama sekali tidak tahu soal audio.

| Event dispatch | SFX yang dimainkan |
|----------------|--------------------|
| `OnDispatchStarted` | `sfxDispatchStart` (suara kirim hantu) |
| `OnDispatchFinished` (sukses) | `sfxSuccess` |
| `OnDispatchFinished` (gagal) | `sfxFail` |

### Setup
1. Taruh `AudioManager` di GameObject scene pertama.
2. Drag clip ke slot Inspector: `Default Bgm`, `Sfx Dispatch Start`, `Sfx Success`, `Sfx Fail`.
   - Slot `AudioSource` boleh dikosongkan — dibuat otomatis saat runtime.
3. BGM otomatis main di `Start()`.

### API buat UI/klik tombol

```csharp
AudioManager.Instance.PlaySFX(clipKlik);       // SFX one-shot
AudioManager.Instance.PlayBGM(clipMenu);        // ganti BGM
AudioManager.Instance.StopBGM();
AudioManager.Instance.SetBgmVolume(0.5f);       // 0..1
AudioManager.Instance.SetSfxVolume(0.8f);
```

> Catatan: volume BELUM di-save (sesuai arahan, dulu simpan uang & unlock saja).
> Kalau nanti butuh, gampang ditambah ke `SaveSystem`.

---

## Cara Tes Cepat
Buka `DispatchDebugTester` (sudah ada tombol baru): **Unlock Hantu Terpilih**, lihat
**Uang** & **Total unlock**, dan **RESET SAVE**. Kirim hantu → dengar SFX sukses/gagal
kalau `AudioManager` + clip sudah dipasang.

Pertanyaan → **Alvin**.
