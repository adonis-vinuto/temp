DECLARE @LastKeyValue BIGINT;

-- Obter o último valor da chave primária
SELECT @LastKeyValue = MAX(R_E_C_N_O_) FROM PROTHEUS_HOMOLOGACAO.dbo.SE2010;

INSERT INTO PROTHEUS_HOMOLOGACAO.dbo.FK7010 (
     FK7_FILIAL
    ,FK7_IDDOC
    ,FK7_ALIAS
    ,FK7_CHAVE
    ,FK7_FILTIT
    ,FK7_PREFIX
    ,FK7_NUM
    ,FK7_PARCEL
    ,FK7_TIPO
    ,FK7_CLIFOR
    ,FK7_LOJA
    ,R_E_C_N_O_
)
SELECT 
     EV_FILIAL AS FK7_FILIAL
    ,CAST(EV_NUM AS varchar(32)) AS FK7_IDDOC
    ,'SEV' AS FK7_ALIAS
    ,CAST(EV_PREFIXO + '-' + EV_NUM + '-' + EV_PARCELA + '-' + EV_TIPO AS varchar(100)) AS FK7_CHAVE
    ,EV_FILIAL AS FK7_FILTIT
    ,EV_PREFIXO AS FK7_PREFIX
    ,EV_NUM AS FK7_NUM
    ,EV_PARCELA AS FK7_PARCEL
    ,EV_TIPO AS FK7_TIPO
    ,EV_CLIFOR AS FK7_CLIFOR
    ,EV_LOJA AS FK7_LOJA
    ,@LastKeyValue + ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS R_E_C_N_O_
FROM PROTHEUS_HOMOLOGACAO.dbo.SEV010 AS SEV
WHERE NOT EXISTS (
    SELECT 1
    FROM PROTHEUS_HOMOLOGACAO.dbo.FK7010 AS FK
    WHERE FK.FK7_PREFIX = SEV.EV_PREFIXO
      AND FK.FK7_NUM    = SEV.EV_NUM
      AND FK.FK7_TIPO   = SEV.EV_TIPO
      AND FK.FK7_FILIAL = SEV.EV_FILIAL
      AND FK.FK7_PARCEL = SEV.EV_PARCELA
);

CREATE OR ALTER PROCEDURE dbo.sp_Inserir_FK7010
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @LastKeyValue BIGINT;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Obter o último valor da chave primária
        SELECT @LastKeyValue = ISNULL(MAX(R_E_C_N_O_), 0)
        FROM PROTHEUS_HOMOLOGACAO.dbo.SE2010;

        -- Inserir os novos registros
        INSERT INTO PROTHEUS_HOMOLOGACAO.dbo.FK7010 (
             FK7_FILIAL
            ,FK7_IDDOC
            ,FK7_ALIAS
            ,FK7_CHAVE
            ,FK7_FILTIT
            ,FK7_PREFIX
            ,FK7_NUM
            ,FK7_PARCEL
            ,FK7_TIPO
            ,FK7_CLIFOR
            ,FK7_LOJA
            ,R_E_C_N_O_
        )
        SELECT 
             EV_FILIAL AS FK7_FILIAL
            ,CAST(EV_NUM AS varchar(32)) AS FK7_IDDOC
            ,'SEV' AS FK7_ALIAS
            ,CAST(EV_PREFIXO + '-' + EV_NUM + '-' + EV_PARCELA + '-' + EV_TIPO AS varchar(100)) AS FK7_CHAVE
            ,EV_FILIAL AS FK7_FILTIT
            ,EV_PREFIXO AS FK7_PREFIX
            ,EV_NUM AS FK7_NUM
            ,EV_PARCELA AS FK7_PARCEL
            ,EV_TIPO AS FK7_TIPO
            ,EV_CLIFOR AS FK7_CLIFOR
            ,EV_LOJA AS FK7_LOJA
            ,@LastKeyValue + ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS R_E_C_N_O_
        FROM PROTHEUS_HOMOLOGACAO.dbo.SEV010 AS SEV
        WHERE NOT EXISTS (
            SELECT 1
            FROM PROTHEUS_HOMOLOGACAO.dbo.FK7010 AS FK
            WHERE FK.FK7_PREFIX = SEV.EV_PREFIXO
              AND FK.FK7_NUM    = SEV.EV_NUM
              AND FK.FK7_TIPO   = SEV.EV_TIPO
              AND FK.FK7_FILIAL = SEV.EV_FILIAL
              AND FK.FK7_PARCEL = SEV.EV_PARCELA
        );

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;

        DECLARE @ErrorMessage NVARCHAR(4000),
                @ErrorSeverity INT,
                @ErrorState INT;

        SELECT 
            @ErrorMessage = ERROR_MESSAGE(),
            @ErrorSeverity = ERROR_SEVERITY(),
            @ErrorState = ERROR_STATE();

        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;
GO
