DECLARE @LastKeyValue INT;

-- Obter o último valor da chave primária
SELECT @LastKeyValue = MAX(R_E_C_N_O_) FROM PROTHEUS_TESTE.dbo.SEV010;

INSERT INTO PROTHEUS_TESTE.dbo.SEV010(
	R_E_C_N_O_, EV_FILIAL, EV_PREFIXO, EV_NUM, EV_PARCELA, EV_CLIFOR, EV_LOJA, EV_TIPO, EV_VALOR, EV_NATUREZ
	)
SELECT 
    @LastKeyValue + ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS R_E_C_N_O_,  -- Incrementa a chave primária
    IntegraVEXP_Filial AS EV_FILIAL
    ,IntegraVEXP_Prefixo AS EV_PREFIXO  
    ,IntegraVEXP_IDRelatorio AS EV_NUM  
    ,CAST(IntegraVEXP_Parcela AS varchar(2)) AS EV_PARCELA  
    ,'' AS EV_CLIFOR
    ,'01' AS EV_LOJA 
    ,IntegraVEXP_Tipo AS EV_TIPO
    ,ISNULL(IntegraVEXP_VlrTitulo, 0) AS EV_VALOR  
    ,ISNULL(IntegraVEXP_RateioNatureza, '0') AS EV_NATUREZ   
FROM PROTHEUS_TESTE.dbo.ZZ7 AS ZZ7
WHERE NOT EXISTS (
    SELECT 1
    FROM PROTHEUS_TESTE.dbo.SEV010 AS SE
    WHERE SE.EV_PREFIXO = ZZ7.IntegraVEXP_Prefixo
      AND SE.EV_NUM = ZZ7.IntegraVEXP_IDRelatorio
      AND SE.EV_TIPO = ZZ7.IntegraVEXP_Tipo
) 
AND ZZ7.IntegraVEXP_Filial != '000000000'
AND (ZZ7.IntegraVEXP_StatusRelatorio = 'APROVADO' OR ZZ7.IntegraVEXP_StatusRelatorio = 'ENVIADO');


CREATE OR ALTER PROCEDURE dbo.sp_Inserir_SEV010
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @LastKeyValue INT;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Obter o último valor da chave primária
        SELECT @LastKeyValue = ISNULL(MAX(R_E_C_N_O_), 0)
        FROM PROTHEUS_TESTE.dbo.SEV010;

        -- Inserir novos registros
        INSERT INTO PROTHEUS_TESTE.dbo.SEV010 (
            R_E_C_N_O_, 
            EV_FILIAL, 
            EV_PREFIXO, 
            EV_NUM, 
            EV_PARCELA, 
            EV_CLIFOR, 
            EV_LOJA, 
            EV_TIPO, 
            EV_VALOR, 
            EV_NATUREZ
        )
        SELECT 
            @LastKeyValue + ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS R_E_C_N_O_,  
            ZZ7.IntegraVEXP_Filial AS EV_FILIAL,
            ZZ7.IntegraVEXP_Prefixo AS EV_PREFIXO,  
            ZZ7.IntegraVEXP_IDRelatorio AS EV_NUM,  
            CAST(ZZ7.IntegraVEXP_Parcela AS varchar(2)) AS EV_PARCELA,  
            '' AS EV_CLIFOR,
            '01' AS EV_LOJA, 
            ZZ7.IntegraVEXP_Tipo AS EV_TIPO,
            ISNULL(ZZ7.IntegraVEXP_VlrTitulo, 0) AS EV_VALOR,  
            ISNULL(ZZ7.IntegraVEXP_RateioNatureza, '0') AS EV_NATUREZ
        FROM PROTHEUS_TESTE.dbo.ZZ7 AS ZZ7
        WHERE NOT EXISTS (
            SELECT 1
            FROM PROTHEUS_TESTE.dbo.SEV010 AS SE
            WHERE SE.EV_PREFIXO = ZZ7.IntegraVEXP_Prefixo
              AND SE.EV_NUM = ZZ7.IntegraVEXP_IDRelatorio
              AND SE.EV_TIPO = ZZ7.IntegraVEXP_Tipo
        )
        AND ZZ7.IntegraVEXP_Filial != '000000000'
        AND (ZZ7.IntegraVEXP_StatusRelatorio = 'APROVADO' OR ZZ7.IntegraVEXP_StatusRelatorio = 'ENVIADO');

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
