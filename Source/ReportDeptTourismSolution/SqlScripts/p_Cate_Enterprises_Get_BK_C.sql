-- Database: [baocao.sct.cenit.vn.cate]
-- p_Cate_Enterprises_Get_BK_C: ban sao cua p_Cate_Enterprises_Get (danh sach doanh nghiep, trang /Cate/Enterprise).
-- Sua loi "Invalid column name 'TypeBusiness'": bang tam #TypeBusinesss co cot TypeBusinessId nhung cau JOIN dung b.TypeBusiness.
-- Mapping: App_Data/Modules/Cate_StoredProcedures.xml -> Cate_Enterprises_Get
-- Can chay script nay tren DB TRUOC khi deploy (danh sach procedure duoc nap 1 lan khi app khoi dong).
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF OBJECT_ID(N'dbo.p_Cate_Enterprises_Get_BK_C', N'P') IS NULL
    EXEC(N'CREATE PROCEDURE [dbo].[p_Cate_Enterprises_Get_BK_C] AS RETURN 0')
GO
ALTER PROCEDURE [dbo].[p_Cate_Enterprises_Get_BK_C]
	@ForUser VARCHAR(500),
	 ---
	@TypeBusinessIds VARCHAR(500),
	@WardIds VARCHAR(500),
	@ProvinceIds VARCHAR(500),
	@IndustryIds VARCHAR(500),
	@EnterpriseTypeIds VARCHAR(500),
	@EconomicSectorIds VARCHAR(500),
	@StatusIds VARCHAR(500),
	 ---
	 --@TypeBusiness INT,
	@Search NVARCHAR(250),
	@Order VARCHAR(3),
	@OrderDir VARCHAR(10),
	@PageIndex INT,
	@PageSize INT
AS
BEGIN
    SET NOCOUNT ON
    -- _BK_C: ban sao cua p_Cate_Enterprises_Get, sua loi "Invalid column name TypeBusiness" (bang tam #TypeBusinesss co cot TypeBusinessId)
    SET @Order = ISNULL(@Order ,'0')
    SET @OrderDir = UPPER(ISNULL(@OrderDir ,'ASC'))
    SET @PageIndex = ISNULL(@PageIndex ,0)
    SET @PageSize = ISNULL(@PageSize ,10)
    
    IF OBJECT_ID('tempdb..#TypeBusinesss') IS NOT NULL
        DROP TABLE #TypeBusinesss;
    
    SELECT CAST(fs.Name AS INT) AS TypeBusinessId
    INTO   #TypeBusinesss
    FROM   dbo.fnSplit(@TypeBusinessIds ,',') AS fs
    
    IF OBJECT_ID('tempdb..#Wards') IS NOT NULL
        DROP TABLE #Wards;
    
    SELECT CAST(fs.Name AS INT)        AS WardId
    INTO   #Wards
    FROM   dbo.fnSplit(@WardIds ,',')  AS fs
    
    IF OBJECT_ID('tempdb..#Provinces') IS NOT NULL
        DROP TABLE #Provinces;
    
    SELECT CAST(fs.Name AS INT) AS ProvinceId
    INTO   #Provinces
    FROM   dbo.fnSplit(@ProvinceIds ,',') AS fs
    
    IF OBJECT_ID('tempdb..#Industry') IS NOT NULL
        DROP TABLE #Industry;
    
    SELECT CAST(fs.Name AS INT) AS IndustryId
    INTO   #Industry
    FROM   dbo.fnSplit(@IndustryIds ,',') AS fs
    
    IF OBJECT_ID('tempdb..#EnterpriseType') IS NOT NULL
        DROP TABLE #EnterpriseType;
    
    SELECT CAST(fs.Name AS INT) AS EnterpriseTypeId
    INTO   #EnterpriseType
    FROM   dbo.fnSplit(@EnterpriseTypeIds ,',') AS fs
    
    IF OBJECT_ID('tempdb..#EconomicSector') IS NOT NULL
        DROP TABLE #EconomicSector;
    
    SELECT CAST(fs.Name AS INT) AS EconomicSectorId
    INTO   #EconomicSector
    FROM   dbo.fnSplit(@EconomicSectorIds ,',') AS fs
    
    IF OBJECT_ID('tempdb..#Status') IS NOT NULL
        DROP TABLE #Status;
    
    SELECT CAST(fs.Name AS INT)          AS StatusId
    INTO   #Status
    FROM   dbo.fnSplit(@StatusIds ,',')  AS fs
    
    ;WITH T AS(
        SELECT ROW_NUMBER() OVER(
                   ORDER BY 
                   --ASC
                   CASE 
                        WHEN @Order=N'0'
                             AND @OrderDir='ASC' THEN am.BusinessName
                       END ASC
                  ,CASE 
                        WHEN @Order=N'1'
                             AND @OrderDir='ASC' THEN am.OwnerEnterpriseName
                       END ASC
                  ,CASE 
                        WHEN @Order=N'2'
                             AND @OrderDir='ASC' THEN am.BusinessName
                       END ASC
                  ,CASE 
                        WHEN @Order=N'3'
                             AND @OrderDir='ASC' THEN am.TaxCode
                       END ASC
                  ,CASE 
                        WHEN @Order=N'4'
                             AND @OrderDir='ASC' THEN am.WardName
                       END ASC
                  ,CASE 
                        WHEN @Order=N'5'
                             AND @OrderDir='ASC' THEN am.ProvinceName
                       END ASC --DESC
                  ,CASE 
                        WHEN @Order=N'0'
                             AND @OrderDir='DESC' THEN am.BusinessName
                       END DESC
                  ,CASE 
                        WHEN @Order=N'1'
                             AND @OrderDir='DESC' THEN am.OwnerEnterpriseName
                       END DESC
                  ,CASE 
                        WHEN @Order=N'2'
                             AND @OrderDir='DESC' THEN am.BusinessName
                       END DESC
                  ,CASE 
                        WHEN @Order=N'3'
                             AND @OrderDir='DESC' THEN am.TaxCode
                       END DESC
                  ,CASE 
                        WHEN @Order=N'4'
                             AND @OrderDir='DESC' THEN am.WardName
                       END DESC
                  ,CASE 
                        WHEN @Order=N'5'
                             AND @OrderDir='DESC' THEN am.ProvinceName
                       END DESC
               )                 AS RowIndex
              ,am.EnterpriseId
              ,am.OwnerEnterpriseName
              ,am.BusinessName
              ,am.TaxCode
              ,am.BusinessAddress
              ,am.StreetName
              ,am.WardId
              ,am.WardName
               --,am.TypeBusiness
               --,am.TypeBusinessName
               --,cbe.IndustryId
               --,cbi.IndustryName  AS IndustryName
              ,cbe.EnterpriseTypeId
              ,cet.[Name]        AS EnterpriseTypeName
              ,cbe.EconomicSectorId
              ,cs.[Name]         AS EconomicSectorName
              ,cbe.EnterpriseStatusId
              ,ces.[Name]        AS EnterpriseStatusName
              ,am.LegalRepresentationName
              ,am.LegalRepresentationPhone
              ,am.LegalRepresentationEmail
              ,am.Website
              ,am.Phone
              ,am.Email
              ,am.IsActive
              ,am.IsDeleted
              ,am.CreatedBy
              ,am.CreatedDate
              ,'<ul class="list-unstyled">'+REPLACE(
                   REPLACE(
                       REPLACE(
                           STUFF(
                               (
                                   SELECT ','+DocName
                                   FROM   (
                                              SELECT 
                                                     '<li><a name="DownloadRefDoc" href="#" data-href="/Doc/RefDoc?fileId=' 
                                                    +(LOWER(CAST(kud.FileId AS NVARCHAR(500))))
                                                    +'"><i class="fa fa-paperclip"></i> '+(kud.[FileName]+kud.FileExt) 
                                                    +
                                                     '</a></li>' AS DocName
                                              FROM   Cate_Docs AS kud
                                              WHERE  kud.ObjectId = am.EnterpriseId
                                                     AND kud.TypeObject = 'Cate_Enterprises'
                                                     AND kud.IsDeleted = 0
                                          ) AS M FOR XML PATH('')
                               )
                              ,1
                              ,1
                              ,''
                           )
                          ,'&lt;'
                          ,'<'
                       )
                      ,'&gt;'
                      ,'>'
                   )
                  ,','
                  ,''
               )+'</ul>'         AS ListPathCertificateFiles
        FROM   Cate_Enterprises  AS am
               LEFT JOIN Cate_BusinessEnterprise AS cbe
                    ON  cbe.EnterpriseId = am.EnterpriseId
                        --LEFT JOIN Cate_BusinessIndustry AS cbi
                        --     ON  cbi.IndustryId = cbe.IndustryId
                        
               LEFT JOIN Cate_EnterpriseType AS cet
                    ON  cet.EnterpriseTypeId = cbe.EnterpriseTypeId
               LEFT JOIN Cate_EnterpriseStatus AS ces
                    ON  ces.EnterpriseStatusId = cbe.EnterpriseStatusId
               LEFT JOIN Cate_EconomicSector AS cs
                    ON  cs.EconomicSectorId = cbe.EconomicSectorId
               LEFT JOIN Cate_EnterprisePermissions AS cep
                    ON  cep.EnterpriseId = am.EnterpriseId
                        AND (
                                @ForUser IS NULL
                                OR (
                                       (
                                           CASE 
                                                WHEN cep.ForUser LIKE '%_@__%.__%' THEN LEFT(cep.ForUser ,CHARINDEX('@' ,cep.ForUser)- 1)
                                                ELSE cep.ForUser
                                           END
                                       )=(
                                           CASE 
                                                WHEN @ForUser LIKE '%_@__%.__%' THEN LEFT(@ForUser ,CHARINDEX('@' ,@ForUser)- 1)
                                                ELSE @ForUser
                                           END
                                       )
                                   )
                            )
        WHERE  am.IsApproved = 1
               AND (
                       @Search IS NULL
                       OR am.OwnerEnterpriseName LIKE N'%'+@Search+'%'
                       OR am.BusinessName LIKE N'%'+@Search+'%'
                       OR am.TaxCode LIKE N'%'+@Search+'%'
                       OR am.BusinessAddress LIKE N'%'+@Search+'%'
                       OR am.StreetName LIKE N'%'+@Search+'%'
                       OR am.WardName LIKE N'%'+@Search+'%'
                          --OR am.TypeBusinessName LIKE N'%'+@Search+'%'
                          --OR cbi.IndustryName LIKE N'%'+@Search+'%'
                       OR cet.[Name] LIKE N'%'+@Search+'%'
                       OR cs.[Name] LIKE N'%'+@Search+'%'
                       OR ces.[Name] LIKE N'%'+@Search+'%'
                       OR am.LegalRepresentationName LIKE N'%'+@Search+'%'
                       OR am.LegalRepresentationPhone LIKE N'%'+@Search+'%'
                       OR am.LegalRepresentationEmail LIKE N'%'+@Search+'%'
                       OR am.Website LIKE N'%'+@Search+'%'
                       OR am.Phone LIKE N'%'+@Search+'%'
                       OR am.Email LIKE N'%'+@Search+'%'
                   )
               AND (
                       @TypeBusinessIds IS NULL
                       OR EXISTS (
                              SELECT 1
                              FROM   dbo.fnSplit(am.TypeBusiness ,',') A
                                     INNER JOIN #TypeBusinesss B
                                          ON  CAST(A.Name AS INT) = b.TypeBusinessId
                          )
                          --OR am.TypeBusiness IN (SELECT TypeBusinessId FROM   #TypeBusinesss)
                   )
               AND (
                       @WardIds IS NULL
                       OR am.WardId IN (SELECT WardId
                                        FROM   #Wards)
                   )
               AND (
                       @ProvinceIds IS NULL
                       OR am.ProvinceId IN (SELECT ProvinceId
                                            FROM   #Provinces)
                   )
               AND (
                       @IndustryIds IS NULL
                       OR EXISTS (
                              SELECT 1
                              FROM   dbo.fnSplit(cbe.IndustryIds ,',') A
                                     INNER JOIN #Industry B
                                          ON  CAST(A.Name AS INT) = b.IndustryId
                          )
                          
                          --OR cbe.IndustryId IN (SELECT IndustryId FROM   #Industry)
                   )
               AND (
                       @EnterpriseTypeIds IS NULL
                       OR cbe.EnterpriseTypeId IN (SELECT EnterpriseTypeId
                                                   FROM   #EnterpriseType)
                   )
               AND (
                       @EconomicSectorIds IS NULL
                       OR cbe.EconomicSectorId IN (SELECT EconomicSectorId
                                                   FROM   #EconomicSector)
                   )
               AND (
                       @StatusIds IS NULL
                       OR cbe.EnterpriseStatusId IN (SELECT StatusId
                                                     FROM   #Status)
                   )
               AND am.IsDeleted = 0
    )
    
    
    -- search and return records
    SELECT T.*
          ,(
               SELECT COUNT(RowIndex)
               FROM   T
           ) AS TotalRow
    FROM   T
    WHERE  (
               @PageSize>0
               AND T.RowIndex BETWEEN @PageIndex+1 AND @PageIndex+@PageSize
           )
           OR @PageSize<=0
END









GO
