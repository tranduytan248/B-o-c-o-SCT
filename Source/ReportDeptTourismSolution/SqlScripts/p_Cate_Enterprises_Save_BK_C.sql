-- Database: [baocao.sct.cenit.vn.cate]
-- p_Cate_Enterprises_Save_BK_C: ban sao cua p_Cate_Enterprises_Save dung rieng cho man hinh thong tin doanh nghiep (MyEnterprise/Info).
-- Khac biet: khi cap nhat KHONG ghi cac truong dang bi khoa tren man hinh (TaxCode, BusinessAddress, StreetName,
-- WardId, WardName, ProvinceId, ProvinceName, TypeBusiness, IndustryIds) => cac truong nay luon giu nguyen gia tri trong DB
-- (tranh viec SP goc tu suy ra Tinh tu Xa/Phuong va ghi NULL vao Tinh cho doanh nghiep chua co Xa/Phuong).
-- Mapping: App_Data/Modules/Cate_StoredProcedures.xml -> Cate_Enterprises_SaveInfo
-- Can chay script nay tren DB TRUOC khi deploy code (danh sach procedure duoc nap 1 lan khi app khoi dong).
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF OBJECT_ID(N'dbo.p_Cate_Enterprises_Save_BK_C', N'P') IS NULL
    EXEC(N'CREATE PROCEDURE [dbo].[p_Cate_Enterprises_Save_BK_C] AS RETURN 0')
GO
ALTER PROCEDURE [dbo].[p_Cate_Enterprises_Save_BK_C]
	@EnterpriseId INT,
	@OwnerEnterpriseName NVARCHAR(2500),
	@BusinessName NVARCHAR(2500),
	@TaxCode VARCHAR(500),
	@BusinessAddress NVARCHAR(2500),
	@StreetName NVARCHAR(2500),
	@WardId INT ,
	@WardName NVARCHAR(2500),
	 --@TypeBusiness INT ,
	 --@TypeBusinessName NVARCHAR(1500) ,
	 -- ---
	 --@MainIndustryId INT,
	 --
	@IndustryIds VARCHAR(50),
	@TypeBusinessIds VARCHAR(50),
	 --
	@EnterpriseTypeId INT,
	@EconomicSectorId INT,
	@EnterpriseStatusId INT,
	 ---
	@LegalRepresentationName NVARCHAR(1500),
	@LegalRepresentationPhone VARCHAR(50),
	@LegalRepresentationEmail VARCHAR(500),
	@Website VARCHAR(1500),
	@Phone VARCHAR(50),
	@Email VARCHAR(1500),
	@Reason NVARCHAR(2500),
	@SaveBy VARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;
    -- _BK_C: ban sao cua p_Cate_Enterprises_Save dung rieng cho man hinh MyEnterprise (doanh nghiep tu cap nhat).
    -- Khac biet: khi cap nhat KHONG ghi cac truong bi khoa tren man hinh (TaxCode, BusinessAddress, StreetName,
    -- WardId, WardName, ProvinceId, ProvinceName, TypeBusiness, IndustryIds) => cac truong nay luon giu nguyen gia tri trong DB.
    BEGIN TRANSACTION
    DECLARE @ActionType VARCHAR(500)='INSERT';
    DECLARE @ProvinceId       INT
           ,@ProvinceName     NVARCHAR(250)
    
    --IF OBJECT_ID('tempdb..#Industry') IS NOT NULL
    --    DROP TABLE #Industry
    
    --SELECT CAST(fs.Name AS INT) AS IndustryId
    --INTO   #Industry
    --FROM   dbo.fnSplit(@IndustryIds ,',') AS fs
    
    BEGIN TRY
        IF EXISTS(
               SELECT 1
               FROM   Cate_Enterprises AS ce
               WHERE  ce.TaxCode = @TaxCode
                      --AND ce.BusinessName = @BusinessName
                      AND ce.IsDeleted = 0
                      AND ce.EnterpriseId<>@EnterpriseId
           )
           --OR (
           --       @EnterpriseId <= 0
           --       AND EXISTS (
           --               SELECT 1
           --               FROM   Cate_Enterprises AS ce
           --               WHERE  ce.TaxCode = @TaxCode
           --                      AND ce.IsDeleted = 0
           --           )
           --   )
        BEGIN
            IF @@TRANCOUNT>0
            BEGIN
                ROLLBACK TRANSACTION
            END
            
            RETURN-9
        END
        
        --IF EXISTS(
        --       SELECT 1
        --       FROM   Cate_Enterprises AS ce
        --       WHERE  ce.TaxCode = @TaxCode
        --              AND ce.IsDeleted = 0
        --   )
        --BEGIN
        --    IF @@TRANCOUNT > 0
        --    BEGIN
        --        ROLLBACK TRANSACTION
        --    END
        
        --    RETURN - 9
        --END
        
        SELECT @ProvinceId = cp.ProvinceId
              ,@ProvinceName = cp.ProvinceName
        FROM   Cate_Wards           AS cw
               JOIN Cate_Provinces  AS cp
                    ON  cp.ProvinceId = cw.ProvinceId
        WHERE  cw.WardId = @WardId
        
        IF NOT EXISTS (
               SELECT 1
               FROM   Cate_Enterprises AS kgj
               WHERE  kgj.EnterpriseId = @EnterpriseId
           )
           --IF @EnterpriseId <= 0
           --   AND NOT EXISTS (
           --           SELECT 1
           --           FROM   Cate_Enterprises AS ce
           --           WHERE  ce.TaxCode = @TaxCode
           --                  AND ce.IsDeleted = 0
           --       )
        BEGIN
            INSERT INTO Cate_Enterprises(
                       -- EnterpriseId -- this column value is auto-generated
                       OwnerEnterpriseName
                      ,BusinessName
                      ,TaxCode
                      ,BusinessAddress
                      ,StreetName
                      ,WardId
                      ,WardName
                      ,ProvinceId
                      ,ProvinceName
                      ,TypeBusiness
                       --,TypeBusinessName
                      ,LegalRepresentationName
                      ,LegalRepresentationPhone
                      ,LegalRepresentationEmail
                      ,Website
                      ,Phone
                      ,Email
                      ,IsDeleted
                      ,CreatedBy
                      ,CreatedDate
                      ,IsApproved
                   )
                   VALUES
                   (
                       @OwnerEnterpriseName
                      ,@BusinessName
                      ,@TaxCode
                      ,@BusinessAddress
                      ,@StreetName
                      ,@WardId
                      ,@WardName
                      ,@ProvinceId
                      ,@ProvinceName
                      ,@TypeBusinessIds
                       --,@TypeBusinessName
                      ,@LegalRepresentationName
                      ,@LegalRepresentationPhone
                      ,@LegalRepresentationEmail
                      ,@Website
                      ,@Phone
                      ,@Email
                      ,0
                      ,@SaveBy
                      ,GETDATE()
                      ,1
                   )
            
            SET @EnterpriseId = SCOPE_IDENTITY()
            INSERT INTO Cate_EnterprisePermissions(EnterpriseId ,ForUser)
                   VALUES
                   (@EnterpriseId ,@SaveBy)
        END
        ELSE
        BEGIN
            UPDATE Cate_Enterprises
            SET    -- EnterpriseId -- this column value is auto-generated
                   OwnerEnterpriseName     = @OwnerEnterpriseName
                  ,BusinessName            = @BusinessName
                   -- Khong cap nhat cac truong bi khoa: TaxCode, BusinessAddress, StreetName, WardId, WardName, ProvinceId, ProvinceName, TypeBusiness
                   --,TypeBusinessName        = @TypeBusinessName
                  ,LegalRepresentationName = @LegalRepresentationName
                  ,LegalRepresentationPhone = @LegalRepresentationPhone
                  ,LegalRepresentationEmail = @LegalRepresentationEmail
                  ,Website                 = @Website
                  ,Phone                   = @Phone
                  ,Email                   = @Email
                  ,LastModifiedBy          = @SaveBy
                  ,LastModifiedDate        = GETDATE()
            WHERE  EnterpriseId            = @EnterpriseId
        END
        
        INSERT INTO [baocao.sct.cenit.vn.log].dbo.Cate_EnterpriseLogs
               (
                   EnterpriseId
                  ,OwnerEnterpriseName
                  ,BusinessName
                  ,TaxCode
                  ,BusinessAddress
                  ,StreetName
                  ,WardId
                  ,WardName
                  ,ProvinceId
                  ,ProvinceName
                  ,TypeBusiness
                  ,TypeBusinessName
                  ,LegalRepresentationName
                  ,LegalRepresentationPhone
                  ,LegalRepresentationEmail
                  ,Website
                  ,Phone
                  ,Email
                  ,IsActive
                  ,IsDeleted
                  ,ActionType
                  ,Reason
                  ,CreatedBy
                  ,CreatedDate
               )
        SELECT ce.EnterpriseId
              ,ce.OwnerEnterpriseName
              ,ce.BusinessName
              ,ce.TaxCode
              ,ce.BusinessAddress
              ,ce.StreetName
              ,ce.WardId
              ,ce.WardName
              ,ce.ProvinceId
              ,ce.ProvinceName
              ,ce.TypeBusiness
              ,ce.TypeBusinessName
              ,ce.LegalRepresentationName
              ,ce.LegalRepresentationPhone
              ,ce.LegalRepresentationEmail
              ,ce.Website
              ,ce.Phone
              ,ce.Email
              ,ce.IsActive
              ,ce.IsDeleted
              ,@ActionType
              ,@Reason
              ,@SaveBy
              ,GETDATE()
        FROM   Cate_Enterprises AS ce
        WHERE  ce.EnterpriseId = @EnterpriseId
        
        IF NOT EXISTS (
               SELECT 1
               FROM   Cate_BusinessEnterprise AS cbe
               WHERE  cbe.EnterpriseId = @EnterpriseId
           )
        BEGIN
            INSERT INTO Cate_BusinessEnterprise(
                       EnterpriseId
                      ,IndustryIds
                      ,EnterpriseTypeId
                      ,EconomicSectorId
                      ,EnterpriseStatusId
                      ,CreatedBy
                      ,CreatedOn
                   )
                   VALUES
                   (
                       @EnterpriseId
                      ,@IndustryIds
                      ,@EnterpriseTypeId
                      ,@EconomicSectorId
                      ,@EnterpriseStatusId
                      ,@SaveBy
                      ,GETDATE()
                   )
        END
        ELSE
        BEGIN
            UPDATE Cate_BusinessEnterprise
            SET    EnterpriseTypeId       = @EnterpriseTypeId -- Khong cap nhat IndustryIds (truong bi khoa)
                  ,EconomicSectorId       = @EconomicSectorId
                  ,EnterpriseStatusId     = @EnterpriseStatusId
                  ,LastModifiedBy         = @SaveBy
                  ,LastModifiedOn         = GETDATE()
            WHERE  EnterpriseId           = @EnterpriseId
        END
        
        --IF @IndustryIds IS NOT NULL
        --   AND LEN(@IndustryIds)>0
        --BEGIN
        --    INSERT INTO Cate_EnterpriseIndustry(EnterpriseId ,IndustryId)
        --    SELECT @EnterpriseId
        --          ,i.IndustryId
        --    FROM   #Industry AS i
        --    WHERE  NOT EXISTS (
        --               SELECT 1
        --               FROM   Cate_EnterpriseIndustry AS cei
        --               WHERE  cei.EnterpriseId = @EnterpriseId
        --                      AND cei.IndustryId = i.IndustryId
        --           )
        --END
        
        IF @@TRANCOUNT>0
        BEGIN
            COMMIT TRANSACTION
        END
        
        RETURN @EnterpriseId
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT>0
        BEGIN
            ROLLBACK TRANSACTION
        END
        
        INSERT INTO Sys_ProcedureLogs(
                   LogDate
                  ,ProcedureName
                  ,ErrorLine
                  ,ErrorMessage
                  ,AdditionalInfo
               )
        SELECT GETDATE()
              ,ERROR_PROCEDURE()  AS ErrorProcedure
              ,ERROR_LINE()       AS ErrorLine
              ,ERROR_MESSAGE()    AS ErrorMessage
              ,NULL
        
        RETURN-1
    END CATCH
END


GO
