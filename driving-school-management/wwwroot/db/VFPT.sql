create or replace PROCEDURE PROC_GET_USERS (
    p_cursor OUT SYS_REFCURSOR
)
AS
BEGIN
    OPEN p_cursor FOR
        SELECT 
            u.userId,
            u.userName,
            u.isActive,
            r.roleName,
            hv.hoTen,
            hv.sdt,
            hv.email
        FROM "User" u
        JOIN "Role" r ON u.roleId = r.roleId
        LEFT JOIN HocVien hv ON u.userId = hv.userId
        ORDER BY u.userId;
END;
/

create or replace PROCEDURE PROC_GET_USER_DETAIL (
    p_userId IN NUMBER,
    p_cursor OUT SYS_REFCURSOR
)
AS
BEGIN
    OPEN p_cursor FOR
        SELECT 
            u.userId,
            u.userName,
            u.isActive,
            u.roleId,
            r.roleName,
            hv.hocVienId,
            hv.hoTen,
            hv.soCmndCccd,
            hv.namSinh,
            hv.gioiTinh,
            hv.sdt,
            hv.email,
            hv.avatarUrl
        FROM "User" u
        JOIN "Role" r ON u.roleId = r.roleId
        LEFT JOIN HocVien hv ON u.userId = hv.userId
        WHERE u.userId = p_userId;
END;
/

create or replace PROCEDURE PROC_UPDATE_USER
(
    p_userId IN NUMBER,
    p_userName IN NVARCHAR2,
    p_isActive IN NUMBER,
    p_hoTen IN NVARCHAR2,
    p_sdt IN NVARCHAR2,
    p_email IN NVARCHAR2
)
AS
BEGIN
    UPDATE "User"
    SET userName = p_userName,
        isActive = p_isActive
    WHERE userId = p_userId;
END;
/
CREATE OR REPLACE PROCEDURE PROC_UPDATE_USER
(
    p_userId IN NUMBER,
    p_userName IN NVARCHAR2,
    p_isActive IN NUMBER,
    p_hoTen IN NVARCHAR2,
    p_sdt IN NVARCHAR2,
    p_email IN NVARCHAR2
)
AS
BEGIN
    UPDATE "User"
    SET userName = p_userName,
        isActive = p_isActive
    WHERE userId = p_userId;
END;
/

CREATE OR REPLACE PROCEDURE PROC_UPDATE_ROLE_SECURE
(
    p_userId IN NUMBER,
    p_roleId IN NUMBER,
    p_adminUser IN NVARCHAR2,
    p_adminPassword IN NVARCHAR2,
    o_result OUT NUMBER
)
AS
    v_password NVARCHAR2(200);
BEGIN
    -- lấy password admin
    SELECT "password"
    INTO v_password
    FROM "User"
    WHERE userName = p_adminUser;

    -- kiểm tra password (tạm so sánh thẳng)
    IF v_password != p_adminPassword THEN
        o_result := -1;
        RETURN;
    END IF;

    -- update role
    UPDATE "User"
    SET roleId = p_roleId
    WHERE userId = p_userId;

    o_result := 1;

EXCEPTION
    WHEN NO_DATA_FOUND THEN
        o_result := -2;
END;
/



COMMIT;

