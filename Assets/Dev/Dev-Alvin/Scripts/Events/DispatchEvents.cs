using System;

namespace ODGJ.Dispatch
{
    /// <summary>
    /// Observer Pattern pakai System.Action. Hub event statis ini memutus
    /// hard-dependency antara core logic (DispatchManager) dan layer presentasi
    /// (UI, VFX, Audio). Subscriber tinggal "+=" tanpa tahu siapa yang nge-raise.
    ///
    /// Catatan untuk subscriber: SELALU unsubscribe di OnDisable/OnDestroy
    /// supaya tidak ada memory leak / NullReference saat scene di-reload.
    /// </summary>
    public static class DispatchEvents
    {
        /// <summary>Hantu mulai dikirim. Trigger animasi/SFX "kirim hantu".</summary>
        public static event Action<GhostData, RequestData> OnDispatchStarted;

        /// <summary>Progress pengiriman 0..1 tiap frame. Untuk loading bar / timer UI.</summary>
        public static event Action<float, GhostData, RequestData> OnDispatchProgress;

        /// <summary>Dispatch selesai. Trigger layar hasil & update resource pemain.</summary>
        public static event Action<DispatchReport> OnDispatchFinished;

        // --- Raise helpers (hanya dipanggil dari DispatchManager) ---

        public static void RaiseStarted(GhostData ghost, RequestData request)
            => OnDispatchStarted?.Invoke(ghost, request);

        public static void RaiseProgress(float progress01, GhostData ghost, RequestData request)
            => OnDispatchProgress?.Invoke(progress01, ghost, request);

        public static void RaiseFinished(DispatchReport report)
            => OnDispatchFinished?.Invoke(report);
    }
}
