namespace driving_school_management.ViewModels
{
    public class HoSoDetailVM
    {
        public int HoSoId { get; set; }
        public string TenHoSo { get; set; }
        public string LoaiHoSo { get; set; }
        public DateTime? NgayDangKy { get; set; }
        public string TrangThai { get; set; }
        public string GhiChu { get; set; }

        public int HocVienId { get; set; }
        public string HoTen { get; set; }
        public string CCCD { get; set; }
        public DateTime? NamSinh { get; set; }
        public string GioiTinh { get; set; }
        public string SDT { get; set; }
        public string Email { get; set; }
        public string AvatarUrl { get; set; }

        public int HangId { get; set; }
        public string MaHang { get; set; }
        public string TenHang { get; set; }

        public int? KhamSucKhoeId { get; set; }
        public string HieuLuc { get; set; }
        public DateTime? ThoiHan { get; set; }
        public string KhamMat { get; set; }
        public string HuyetAp { get; set; }
        public decimal? ChieuCao { get; set; }
        public decimal? CanNang { get; set; }

        public List<AnhGkskVM> AnhMinhChungs { get; set; } = new List<AnhGkskVM>();
    }
}