using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;
using driving_school_management.ViewModels;

public class AdminUserService
{
    private readonly string _connectionString;

    public AdminUserService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("OracleDb");
    }

    public List<dynamic> GetUsers()
    {
        var list = new List<dynamic>();

        using var conn = new OracleConnection(_connectionString);
        using var cmd = new OracleCommand("PROC_GET_USERS", conn);

        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

        conn.Open();

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new
            {
                userId = Convert.ToInt32(reader["userId"]),
                userName = Convert.ToString(reader["userName"]),
                roleName = Convert.ToString(reader["roleName"]),
                hoTen = Convert.ToString(reader["hoTen"]),
                isActive = Convert.ToInt32(reader["isActive"])
            });
        }

        return list;
    }

    public dynamic GetUserDetail(int userId)
    {
        using var conn = new OracleConnection(_connectionString);
        using var cmd = new OracleCommand("PROC_GET_USER_DETAIL", conn);

        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.Add("p_userId", OracleDbType.Int32).Value = userId;
        cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

        conn.Open();

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new
            {
                userId = Convert.ToInt32(reader["userId"]),
                userName = Convert.ToString(reader["userName"]),
                roleId = Convert.ToInt32(reader["roleId"]),
                roleName = Convert.ToString(reader["roleName"]),
                hoTen = Convert.ToString(reader["hoTen"]),
                sdt = Convert.ToString(reader["sdt"]),
                email = Convert.ToString(reader["email"]),
                isActive = Convert.ToInt32(reader["isActive"])
            };
        }

        return null;
    }

    public void UpdateUser(UserUpdateVM model)
    {
        using var conn = new OracleConnection(_connectionString);
        using var cmd = new OracleCommand("PROC_UPDATE_USER", conn);

        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.Add("p_userId", OracleDbType.Int32).Value = model.UserId;
        cmd.Parameters.Add("p_userName", OracleDbType.NVarchar2).Value = model.UserName;
        cmd.Parameters.Add("p_isActive", OracleDbType.Int32).Value = model.IsActive;
        cmd.Parameters.Add("p_hoTen", OracleDbType.NVarchar2).Value = model.HoTen;
        cmd.Parameters.Add("p_sdt", OracleDbType.NVarchar2).Value = model.Sdt;
        cmd.Parameters.Add("p_email", OracleDbType.NVarchar2).Value = model.Email;

        conn.Open();
        cmd.ExecuteNonQuery();
    }

    public void UpdateRole(int userId, int roleId)
    {
        using var conn = new OracleConnection(_connectionString);
        using var cmd = new OracleCommand("PROC_UPDATE_ROLE", conn);

        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.Add("p_userId", OracleDbType.Int32).Value = userId;
        cmd.Parameters.Add("p_roleId", OracleDbType.Int32).Value = roleId;

        conn.Open();
        cmd.ExecuteNonQuery();
    }

    public int UpdateRoleSecure(int userId, int roleId, string adminUser, string adminPassword)
    {
        if (string.IsNullOrWhiteSpace(adminUser))
            return -2;

        if (string.IsNullOrWhiteSpace(adminPassword))
            return -1;

        using var conn = new OracleConnection(_connectionString);
        conn.Open();

        string? storedHash;

        using (var checkCmd = new OracleCommand(
            @"SELECT ""password""
              FROM ""User""
              WHERE userName = :adminUser", conn))
        {
            checkCmd.CommandType = CommandType.Text;
            checkCmd.Parameters.Add(":adminUser", OracleDbType.NVarchar2).Value = adminUser;

            var result = checkCmd.ExecuteScalar();
            if (result == null || result == DBNull.Value)
                return -2;

            storedHash = result.ToString();
        }

        if (string.IsNullOrWhiteSpace(storedHash))
            return -2;

        if (!BCrypt.Net.BCrypt.Verify(adminPassword, storedHash))
            return -1;

        using var cmd = new OracleCommand("PROC_UPDATE_ROLE_SECURE", conn);
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.Add("p_userId", OracleDbType.Int32).Value = userId;
        cmd.Parameters.Add("p_roleId", OracleDbType.Int32).Value = roleId;
        cmd.Parameters.Add("o_result", OracleDbType.Int32).Direction = ParameterDirection.Output;

        cmd.ExecuteNonQuery();

        var resultObj = cmd.Parameters["o_result"].Value;

        if (resultObj is OracleDecimal oracleDecimal)
            return oracleDecimal.ToInt32();

        return 0;
    }
}