using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using driving_school_management.ViewModels;

public class AdminHoSoController : Controller
{
    private readonly IConfiguration _configuration;

    public AdminHoSoController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IActionResult Index(string cccd, string ten, string hang, string trangthai)
    {
        ViewBag.Hangs = GetHangs();

        var data = GetHoSo(cccd, ten, hang, trangthai);

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            return PartialView("_HoSoTable", data);
        }

        return View(data);
    }

    public IActionResult Details(int id)
    {
        var model = GetHoSoDetail(id);

        if (model == null)
        {
            return NotFound();
        }

        return View(model);
    }

    public IActionResult Approve(int id)
    {
        ExecuteStatusProcedure("SP_DUYET_HOSO", id);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Reject(int id)
    {
        ExecuteStatusProcedure("SP_TUCHOI_HOSO", id);
        return RedirectToAction(nameof(Index));
    }

    private List<HoSoVM> GetHoSo(string cccd, string ten, string hang, string trangthai)
    {
        List<HoSoVM> list = new List<HoSoVM>();

        using (OracleConnection conn = new OracleConnection(_configuration.GetConnectionString("OracleDb")))
        {
            conn.Open();

            using (OracleCommand cmd = new OracleCommand("SP_GET_DANHSACH_HOSO", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("P_CCCD", OracleDbType.Varchar2).Value =
                    string.IsNullOrWhiteSpace(cccd) ? DBNull.Value : (object)cccd;

                cmd.Parameters.Add("P_TEN", OracleDbType.Varchar2).Value =
                    string.IsNullOrWhiteSpace(ten) ? DBNull.Value : (object)ten;

                cmd.Parameters.Add("P_HANG", OracleDbType.Varchar2).Value =
                    string.IsNullOrWhiteSpace(hang) ? DBNull.Value : (object)hang;

                cmd.Parameters.Add("P_TRANGTHAI", OracleDbType.Varchar2).Value =
                    string.IsNullOrWhiteSpace(trangthai) ? DBNull.Value : (object)trangthai;

                cmd.Parameters.Add("P_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new HoSoVM
                        {
                            HoSoId = Convert.ToInt32(reader["HOSOID"]),
                            HoTen = reader["HOTEN"]?.ToString(),
                            CCCD = reader["SOCMNDCCCD"]?.ToString(),
                            TenHoSo = reader["TENHOSO"]?.ToString(),
                            Hang = reader["TENHANG"]?.ToString(),
                            NgayDangKy = Convert.ToDateTime(reader["NGAYDANGKY"]),
                            TrangThai = reader["TRANGTHAI"]?.ToString()
                        });
                    }
                }
            }
        }

        return list;
    }

    private List<dynamic> GetHangs()
    {
        List<dynamic> list = new List<dynamic>();

        using (OracleConnection conn = new OracleConnection(_configuration.GetConnectionString("OracleDb")))
        {
            conn.Open();

            using (OracleCommand cmd = new OracleCommand("SP_GET_HANGGPLX", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("P_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new
                        {
                            HANGID = Convert.ToInt32(reader["HANGID"]),
                            TENHANG = reader["TENHANG"]?.ToString()
                        });
                    }
                }
            }
        }

        return list;
    }

    private HoSoDetailVM GetHoSoDetail(int id)
    {
        HoSoDetailVM model = null;

        using (OracleConnection conn = new OracleConnection(_configuration.GetConnectionString("OracleDb")))
        {
            conn.Open();

            using (OracleCommand cmd = new OracleCommand("SP_GET_CHITIET_HOSO", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("P_HOSOID", OracleDbType.Int32).Value = id;
                cmd.Parameters.Add("P_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        model = new HoSoDetailVM
                        {
                            HoSoId = Convert.ToInt32(reader["HOSOID"]),
                            TenHoSo = reader["TENHOSO"]?.ToString(),
                            LoaiHoSo = reader["LOAIHOSO"]?.ToString(),
                            NgayDangKy = reader["NGAYDANGKY"] == DBNull.Value ? null : Convert.ToDateTime(reader["NGAYDANGKY"]),
                            TrangThai = reader["TRANGTHAI"]?.ToString(),
                            GhiChu = reader["GHICHU"]?.ToString(),

                            HocVienId = Convert.ToInt32(reader["HOCVIENID"]),
                            HoTen = reader["HOTEN"]?.ToString(),
                            CCCD = reader["SOCMNDCCCD"]?.ToString(),
                            NamSinh = reader["NAMSINH"] == DBNull.Value ? null : Convert.ToDateTime(reader["NAMSINH"]),
                            GioiTinh = reader["GIOITINH"]?.ToString(),
                            SDT = reader["SDT"]?.ToString(),
                            Email = reader["EMAIL"]?.ToString(),
                            AvatarUrl = reader["AVATARURL"]?.ToString(),

                            HangId = Convert.ToInt32(reader["HANGID"]),
                            MaHang = reader["MAHANG"]?.ToString(),
                            TenHang = reader["TENHANG"]?.ToString(),

                            KhamSucKhoeId = reader["KHAMSUCKHOEID"] == DBNull.Value ? null : Convert.ToInt32(reader["KHAMSUCKHOEID"]),
                            HieuLuc = reader["HIEULUC"]?.ToString(),
                            ThoiHan = reader["THOIHAN"] == DBNull.Value ? null : Convert.ToDateTime(reader["THOIHAN"]),
                            KhamMat = reader["KHAMMAT"]?.ToString(),
                            HuyetAp = reader["HUYETAP"]?.ToString(),
                            ChieuCao = reader["CHIEUCAO"] == DBNull.Value ? null : Convert.ToDecimal(reader["CHIEUCAO"]),
                            CanNang = reader["CANNANG"] == DBNull.Value ? null : Convert.ToDecimal(reader["CANNANG"])
                        };
                    }
                }
            }

            if (model != null)
            {
                using (OracleCommand cmd = new OracleCommand("SP_GET_ANH_GKSK_BY_HOSO", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("P_HOSOID", OracleDbType.Int32).Value = id;
                    cmd.Parameters.Add("P_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            model.AnhMinhChungs.Add(new AnhGkskVM
                            {
                                AnhId = Convert.ToInt32(reader["ANHID"]),
                                KhamSucKhoeId = Convert.ToInt32(reader["KHAMSUCKHOEID"]),
                                UrlAnh = reader["URLANH"]?.ToString()
                            });
                        }
                    }
                }
            }
        }

        return model;
    }
    private void ExecuteStatusProcedure(string procedureName, int hoSoId)
    {
        using (OracleConnection conn = new OracleConnection(_configuration.GetConnectionString("OracleDb")))
        {
            conn.Open();

            using (OracleCommand cmd = new OracleCommand(procedureName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("P_HOSOID", OracleDbType.Int32).Value = hoSoId;
                cmd.ExecuteNonQuery();
            }
        }
    }
}