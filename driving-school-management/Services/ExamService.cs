using driving_school_management.Models.DTOs;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace driving_school_management.Services
{
    public class ExamService
    {
        private readonly string _connectionString;

        public ExamService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OracleDb");
        }

        private static int GetInt32Safe(object value)
        {
            if (value == null || value == DBNull.Value) return 0;
            if (value is OracleDecimal od) return od.ToInt32();
            if (value is decimal d) return (int)d;
            if (value is int i) return i;
            if (value is long l) return (int)l;
            if (value is short s) return s;
            return int.Parse(value.ToString()!);
        }

        private static decimal GetDecimalSafe(object value)
        {
            if (value == null || value == DBNull.Value) return 0;
            if (value is OracleDecimal od) return od.Value;
            if (value is decimal d) return d;
            if (value is double db) return (decimal)db;
            if (value is float f) return (decimal)f;
            if (value is int i) return i;
            if (value is long l) return l;
            return decimal.Parse(value.ToString()!);
        }

        public async Task<List<UserExamDto>> GetKyThiForUserAsync(int userId)
        {
            var list = new List<UserExamDto>();

            using var conn = new OracleConnection(_connectionString);
            using var cmd = new OracleCommand("PKG_KYTHI.GET_KYTHI_FOR_USER", conn);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("p_userId", OracleDbType.Int32).Value = userId;
            cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            await conn.OpenAsync();

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new UserExamDto
                {
                    KyThiId = GetInt32Safe(reader["KYTHIID"]),
                    TenKyThi = reader["TENKYTHI"] == DBNull.Value ? null : reader["TENKYTHI"].ToString(),
                    LoaiKyThi = reader["LOAIKYTHI"] == DBNull.Value ? null : reader["LOAIKYTHI"].ToString(),
                    HoSoId = GetInt32Safe(reader["HOSOID"]),
                    HangId = GetInt32Safe(reader["HANGID"]),
                    MaHang = reader["MAHANG"] == DBNull.Value ? null : reader["MAHANG"].ToString(),
                    TenHang = reader["TENHANG"] == DBNull.Value ? null : reader["TENHANG"].ToString(),
                    HocPhi = GetDecimalSafe(reader["HOCPHI"]),
                    DuDieuKienThiTotNghiep = GetInt32Safe(reader["DUDKTOTNGHIEP"]) == 1,
                    DuDieuKienThiSatHach = GetInt32Safe(reader["DUDKSATHACH"]) == 1,
                    DauTotNghiep = GetInt32Safe(reader["DAUTOTNGHIEP"]) == 1,
                    DaHoanThanh = GetInt32Safe(reader["DAHOANTHANH"]) == 1,
                    CoTheDangKy = GetInt32Safe(reader["COTHEDANGKY"]) == 1,
                    SoKyThiDangKy = GetInt32Safe(reader["SOKYTHIDANGKY"]),
                    TongPhiDuKien = GetDecimalSafe(reader["TONGPHIDUKIEN"]),
                    CanhBao = reader["CANHBAO"] == DBNull.Value ? null : reader["CANHBAO"].ToString()
                });
            }

            return list;
        }

        public async Task<UserExamConfirmDto?> GetConfirmDangKyInfoAsync(int userId, int kyThiId)
        {
            var result = new UserExamConfirmDto();

            using var conn = new OracleConnection(_connectionString);
            using var cmd = new OracleCommand("PKG_KYTHI.GET_CONFIRM_DANGKY_INFO", conn);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("p_userId", OracleDbType.Int32).Value = userId;
            cmd.Parameters.Add("p_kyThiId", OracleDbType.Int32).Value = kyThiId;
            cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            await conn.OpenAsync();

            using var reader = await cmd.ExecuteReaderAsync();
            var hasData = false;

            while (await reader.ReadAsync())
            {
                if (!hasData)
                {
                    result.HoTen = reader["HOTEN"] == DBNull.Value ? null : reader["HOTEN"].ToString();
                    result.HoSoId = GetInt32Safe(reader["HOSOID"]);
                    result.HangId = GetInt32Safe(reader["HANGID"]);
                    result.MaHang = reader["MAHANG"] == DBNull.Value ? null : reader["MAHANG"].ToString();
                    result.TenHang = reader["TENHANG"] == DBNull.Value ? null : reader["TENHANG"].ToString();
                    result.HocPhi = GetDecimalSafe(reader["HOCPHI"]);
                    result.LePhiMotKy = GetDecimalSafe(reader["LEPHIMOTKY"]);
                    result.TongPhi = GetDecimalSafe(reader["TONGPHI"]);
                    result.DuDieuKienThiTotNghiep = GetInt32Safe(reader["DUDKTOTNGHIEP"]) == 1;
                    result.DuDieuKienThiSatHach = GetInt32Safe(reader["DUDKSATHACH"]) == 1;
                    result.DauTotNghiep = GetInt32Safe(reader["DAUTOTNGHIEP"]) == 1;
                    result.DaHoanThanh = GetInt32Safe(reader["DAHOANTHANH"]) == 1;
                    hasData = true;
                }

                result.DanhSachKyThi.Add(new UserExamConfirmItemDto
                {
                    ThuTu = GetInt32Safe(reader["THUTU"]),
                    KyThiId = GetInt32Safe(reader["KYTHIID"]),
                    TenKyThi = reader["TENKYTHI"] == DBNull.Value ? null : reader["TENKYTHI"].ToString(),
                    LoaiKyThi = reader["LOAIKYTHI"] == DBNull.Value ? null : reader["LOAIKYTHI"].ToString(),
                    GhiChu = reader["GHICHU"] == DBNull.Value ? null : reader["GHICHU"].ToString()
                });
            }

            return hasData ? result : null;
        }
    }
}