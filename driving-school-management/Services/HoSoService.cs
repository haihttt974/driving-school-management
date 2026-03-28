using driving_school_management.Models.DTOs;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

public class HoSoService
{
    private readonly string _conn;

    public HoSoService(IConfiguration config)
    {
        _conn = config.GetConnectionString("OracleDb");
    }

    public List<HoSoDto> GetDanhSach()
    {
        var list = new List<HoSoDto>();

        using (var conn = new OracleConnection(_conn))
        {
            conn.Open();

            using (var cmd = new OracleCommand("SP_GET_DANH_SACH_HOSO", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new HoSoDto
                        {
                            HoSoId = Convert.ToInt32(reader["hoSoId"]),
                            HoTen = reader["hoTen"].ToString(),
                            Sdt = reader["sdt"].ToString(),
                            TenHang = reader["tenHang"].ToString(),
                            NgayDangKy = Convert.ToDateTime(reader["ngayDangKy"]),
                            TrangThai = reader["trangThai"].ToString(),
                            SoThangConLai = Convert.ToInt32(reader["soThangConLai"]),
                            SoNgayConLai = Convert.ToInt32(reader["soNgayConLai"])
                        });
                    }
                }
            }
        }

        return list;
    }

    public HoSoDetailDto GetDetail(int hoSoId)
    {
        var result = new HoSoDetailDto();

        using (var conn = new OracleConnection(_conn))
        {
            conn.Open();

            using (var cmd = new OracleCommand("SP_GET_HOSO_DETAIL", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("p_hoSoId", OracleDbType.Int32).Value = hoSoId;

                cmd.Parameters.Add("p_info", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("p_images", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                // ===== Cursor info =====
                using (var reader = ((OracleRefCursor)cmd.Parameters["p_info"].Value).GetDataReader())
                {
                    if (reader.Read())
                    {
                        result.HoSoId = Convert.ToInt32(reader["hoSoId"]);
                        result.HoTen = reader["hoTen"]?.ToString();
                        result.SoCmndCccd = reader["soCmndCccd"]?.ToString();

                        result.NamSinh = reader["namSinh"] == DBNull.Value
                            ? null
                            : (DateTime?)reader["namSinh"];

                        result.GioiTinh = reader["gioiTinh"]?.ToString();
                        result.Sdt = reader["sdt"]?.ToString();
                        result.Email = reader["email"]?.ToString();
                        result.AvatarUrl = reader["avatarUrl"]?.ToString();

                        result.TenHang = reader["tenHang"]?.ToString();
                        result.NgayDangKy = Convert.ToDateTime(reader["ngayDangKy"]);
                        result.TrangThai = reader["trangThai"]?.ToString();
                        result.GhiChu = reader["ghiChu"]?.ToString();

                        result.HieuLuc = reader["hieuLuc"]?.ToString();

                        result.ThoiHan = reader["thoiHan"] == DBNull.Value
                            ? null
                            : (DateTime?)reader["thoiHan"];

                        result.KhamMat = reader["khamMat"]?.ToString();
                        result.HuyetAp = reader["huyetAp"]?.ToString();

                        result.ChieuCao = reader["chieuCao"] == DBNull.Value
                            ? null
                            : Convert.ToDecimal(reader["chieuCao"]);

                        result.CanNang = reader["canNang"] == DBNull.Value
                            ? null
                            : Convert.ToDecimal(reader["canNang"]);
                    }
                }

                // ===== Cursor images =====
                using (var reader = ((OracleRefCursor)cmd.Parameters["p_images"].Value).GetDataReader())
                {
                    while (reader.Read())
                    {
                        result.Images.Add(reader["urlAnh"].ToString());
                    }
                }
            }
        }

        return result;
    }

    public List<MyHoSoCardDto> GetMyHoSo(int userId)
    {
        var list = new List<MyHoSoCardDto>();

        using (var conn = new OracleConnection(_conn))
        {
            conn.Open();

            using (var cmd = new OracleCommand("SP_GET_MY_HOSO", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("p_userId", OracleDbType.Int32).Value = userId;
                cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new MyHoSoCardDto
                        {
                            HoSoId = Convert.ToInt32(reader["hoSoId"]),
                            HoTen = reader["hoTen"]?.ToString(),
                            AvatarUrl = reader["avatarUrl"]?.ToString(),
                            TenHoSo = reader["tenHoSo"]?.ToString(),
                            TenHang = reader["tenHang"]?.ToString(),
                            NgayDangKy = Convert.ToDateTime(reader["ngayDangKy"]),
                            TrangThai = reader["trangThai"]?.ToString(),
                            SoThangConLai = Convert.ToInt32(reader["soThangConLai"]),
                            SoNgayConLai = Convert.ToInt32(reader["soNgayConLai"])
                        });
                    }
                }
            }
        }

        return list;
    }

    public HoSoDetailDto? GetDetailByUser(int hoSoId, int userId)
    {
        HoSoDetailDto? result = null;

        using (var conn = new OracleConnection(_conn))
        {
            conn.Open();

            using (var cmd = new OracleCommand("SP_GET_MY_HOSO_DETAIL", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("p_hoSoId", OracleDbType.Int32).Value = hoSoId;
                cmd.Parameters.Add("p_userId", OracleDbType.Int32).Value = userId;
                cmd.Parameters.Add("p_info", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("p_images", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                using (var reader = ((OracleRefCursor)cmd.Parameters["p_info"].Value).GetDataReader())
                {
                    if (reader.Read())
                    {
                        result = new HoSoDetailDto
                        {
                            HoSoId = Convert.ToInt32(reader["hoSoId"]),
                            HoTen = reader["hoTen"]?.ToString(),
                            SoCmndCccd = reader["soCmndCccd"]?.ToString(),
                            NamSinh = reader["namSinh"] == DBNull.Value ? null : (DateTime?)reader["namSinh"],
                            GioiTinh = reader["gioiTinh"]?.ToString(),
                            Sdt = reader["sdt"]?.ToString(),
                            Email = reader["email"]?.ToString(),
                            AvatarUrl = reader["avatarUrl"]?.ToString(),
                            TenHoSo = reader["tenHoSo"]?.ToString(),
                            LoaiHoSo = reader["loaiHoSo"]?.ToString(),
                            NgayDangKy = reader["ngayDangKy"] == DBNull.Value ? null : (DateTime?)reader["ngayDangKy"],
                            TrangThai = reader["trangThai"]?.ToString(),
                            GhiChu = reader["ghiChu"]?.ToString(),
                            TenHang = reader["tenHang"]?.ToString(),
                            HieuLuc = reader["hieuLuc"]?.ToString(),
                            ThoiHan = reader["thoiHan"] == DBNull.Value ? null : (DateTime?)reader["thoiHan"],
                            KhamMat = reader["khamMat"]?.ToString(),
                            HuyetAp = reader["huyetAp"]?.ToString(),
                            ChieuCao = reader["chieuCao"] == DBNull.Value ? null : Convert.ToDecimal(reader["chieuCao"]),
                            CanNang = reader["canNang"] == DBNull.Value ? null : Convert.ToDecimal(reader["canNang"])
                        };
                    }
                }

                if (result != null)
                {
                    using (var reader = ((OracleRefCursor)cmd.Parameters["p_images"].Value).GetDataReader())
                    {
                        while (reader.Read())
                        {
                            var urlAnh = reader["urlAnh"]?.ToString();
                            if (!string.IsNullOrWhiteSpace(urlAnh))
                            {
                                result.Images.Add(urlAnh);
                            }
                        }
                    }
                }
            }
        }

        return result;
    }

    public MyHoSoPageDto GetMyHoSoPage(int userId)
    {
        var result = new MyHoSoPageDto();

        result.Cards = GetMyHoSo(userId);

        foreach (var card in result.Cards)
        {
            var detail = GetDetailByUser(card.HoSoId, userId);
            if (detail != null)
            {
                detail.SoThangConLai = card.SoThangConLai;
                detail.SoNgayConLai = card.SoNgayConLai;

                if (string.IsNullOrWhiteSpace(detail.AvatarUrl))
                {
                    detail.AvatarUrl = "/images/avatar/default.png";
                }

                result.Details.Add(detail);
            }
        }

        return result;
    }
}