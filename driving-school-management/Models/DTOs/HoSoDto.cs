namespace driving_school_management.Models.DTOs
{
    public class HoSoDto
    {
        public int HoSoId { get; set; }
        public string HoTen { get; set; }
        public string Sdt { get; set; }
        public string TenHang { get; set; }
        public DateTime NgayDangKy { get; set; }
        public string TrangThai { get; set; }
        public int SoThangConLai { get; set; }
        public int SoNgayConLai { get; set; }
    }
}
