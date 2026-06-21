namespace ODGJ.Dispatch
{
    /// <summary>
    /// Payload hasil akhir sebuah dispatch. Dikirim lewat event OnDispatchFinished
    /// supaya UI/Audio bisa nampilin layar hasil & resource manager bisa update uang
    /// tanpa perlu tahu cara kerja DispatchManager.
    /// </summary>
    public class DispatchReport
    {
        public GhostData Ghost { get; }
        public RequestData Request { get; }

        /// <summary>Peluang sukses hasil kalkulasi (0..1), sebelum di-roll RNG.</summary>
        public float SuccessChance { get; }

        public bool IsSuccess { get; }

        /// <summary>Perubahan uang pemain: positif kalau sukses, negatif kalau gagal.</summary>
        public int MoneyDelta { get; }

        public DispatchReport(GhostData ghost, RequestData request,
                              float successChance, bool isSuccess, int moneyDelta)
        {
            Ghost = ghost;
            Request = request;
            SuccessChance = successChance;
            IsSuccess = isSuccess;
            MoneyDelta = moneyDelta;
        }
    }
}
