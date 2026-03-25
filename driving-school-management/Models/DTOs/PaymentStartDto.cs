namespace driving_school_management.Models.DTOs
{
    public class PaymentStartDto
    {
        public int IsValid { get; set; }
        public string Message { get; set; } = string.Empty;

        public int PhieuId { get; set; }
        public int KhoaHocId { get; set; }
        public string TenKhoaHoc { get; set; } = string.Empty;
        public int HoSoId { get; set; }
        public string HoTenHocVien { get; set; } = string.Empty;
        public string TenHang { get; set; } = string.Empty;
        public decimal SoTien { get; set; }
        public DateTime? NgayLap { get; set; }
        public string NoiDungMacDinh { get; set; } = string.Empty;
        public string PhuongThuc { get; set; } = string.Empty;
    }
}