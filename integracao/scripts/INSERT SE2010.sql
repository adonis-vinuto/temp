DECLARE @LastKeyValue INT;

-- Obter o último valor da chave primária
SELECT @LastKeyValue = MAX(R_E_C_N_O_) FROM PROTHEUS_TESTE.dbo.SE2010;

INSERT INTO PROTHEUS_TESTE.dbo.SE2010 (
    R_E_C_N_O_,  -- Inclua a coluna da chave primária
    E2_FILIAL, 
    E2_PREFIXO, 
    E2_NUM, 
    E2_PARCELA, 
    E2_TIPO, 
    E2_NATUREZ, 
    E2_PORTADO, 
    E2_FORNECE, 
    E2_LOJA, 
    E2_NOMFOR, 
    E2_EMISSAO, 
    E2_VENCTO, 
    E2_VENCREA, 
    E2_VALOR, 
    E2_ISS, 
    E2_IRRF, 
    E2_HIST, 
    E2_SALDO,
    E2_ITEMCTA,
    E2_CCUSTO,
    E2_MULTNAT
)
SELECT
    @LastKeyValue + ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS R_E_C_N_O_,  
    IntegraVEXP_Filial AS E2_FILIAL,
    IntegraVEXP_Prefixo AS E2_PREFIXO,  
    IntegraVEXP_IDRelatorio AS E2_NUM,  
    ISNULL(MAX(TRY_CAST(IntegraVEXP_Parcela AS INT)), 1) AS E2_PARCELA,  -- Rever
    IntegraVEXP_Tipo AS E2_TIPO,
    ISNULL(IntegraVEXP_Natureza, 'VAZIO') AS E2_NATUREZ,  
    '001' AS E2_PORTADO,
    IntegraVEXP_Fornecedor AS E2_FORNECE,  
    '01' AS E2_LOJA,  
    LEFT(IntegraVEXP_NomeFornecedor, 20) AS E2_NOMFOR,  
    FORMAT(MIN(IntegraVEXP_DataEmissao), 'yyyyMMdd') AS E2_EMISSAO,  
    FORMAT(MAX(IntegraVEXP_DataVencimento), 'yyyyMMdd') AS E2_VENCTO,  
    FORMAT(MAX(IntegraVEXP_DataVencimentoReal), 'yyyyMMdd') AS E2_VENCREA,  
    ISNULL(SUM(IntegraVEXP_VlrTitulo), 0) AS E2_VALOR,  
    0 AS E2_ISS,  
    0 AS E2_IRRF,  
    ISNULL(LEFT(IntegraVEXP_Historico, 40), 'N/A') AS E2_HIST,  
    ISNULL(SUM(IntegraVEXP_Saldo), 0) AS E2_SALDO,
    ISNULL(IntegraVEXP_UnidadeDeNegocio, 'N/A') AS E2_ITENCTA,
    ISNULL(MAX(IntegraVEXP_CentroCusto), 'N/A') AS E2_CCUSTO,
    1 as E2_MULTNAT
FROM PROTHEUS_TESTE.dbo.ZZ7 AS ZZ7
WHERE NOT EXISTS (
    SELECT 1
    FROM PROTHEUS_TESTE.dbo.SE2010 AS SE
    WHERE SE.E2_PREFIXO = ZZ7.IntegraVEXP_Prefixo
      AND SE.E2_NUM = ZZ7.IntegraVEXP_IDRelatorio
      AND SE.E2_TIPO = ZZ7.IntegraVEXP_Tipo
)
AND ZZ7.IntegraVEXP_Filial != '000000000'
AND ZZ7.IntegraVEXP_Parcela IS NOT NULL
AND (ZZ7.IntegraVEXP_StatusRelatorio = 'APROVADO' OR ZZ7.IntegraVEXP_StatusRelatorio = 'ENVIADO')
GROUP BY 
    IntegraVEXP_IDRelatorio,
    IntegraVEXP_Filial,
    IntegraVEXP_Prefixo,
    IntegraVEXP_Parcela,
    IntegraVEXP_Tipo,
    IntegraVEXP_Natureza,
    IntegraVEXP_Fornecedor,
    IntegraVEXP_NomeFornecedor,
    IntegraVEXP_Historico,
    IntegraVEXP_UnidadeDeNegocio
ORDER BY IntegraVEXP_IDRelatorio;


CREATE OR ALTER PROCEDURE dbo.sp_Inserir_SE2010
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @LastKeyValue INT;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Obter o último valor da chave primária
        SELECT @LastKeyValue = ISNULL(MAX(R_E_C_N_O_), 0)
        FROM PROTHEUS_TESTE.dbo.SE2010;

        -- Inserir novos registros
        INSERT INTO PROTHEUS_TESTE.dbo.SE2010 (
            R_E_C_N_O_,
            E2_FILIAL, 
            E2_PREFIXO, 
            E2_NUM, 
            E2_PARCELA, 
            E2_TIPO, 
            E2_NATUREZ, 
            E2_PORTADO, 
            E2_FORNECE, 
            E2_LOJA, 
            E2_NOMFOR, 
            E2_EMISSAO, 
            E2_VENCTO, 
            E2_VENCREA, 
            E2_VALOR, 
            E2_ISS, 
            E2_IRRF, 
            E2_HIST, 
            E2_SALDO,
            E2_ITEMCTA,
            E2_CCUSTO,
            E2_MULTNAT
        )
        SELECT
            @LastKeyValue + ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS R_E_C_N_O_,  
            ZZ7.IntegraVEXP_Filial AS E2_FILIAL,
            ZZ7.IntegraVEXP_Prefixo AS E2_PREFIXO,  
            ZZ7.IntegraVEXP_IDRelatorio AS E2_NUM,  
            ISNULL(MAX(TRY_CAST(ZZ7.IntegraVEXP_Parcela AS INT)), 1) AS E2_PARCELA,  
            ZZ7.IntegraVEXP_Tipo AS E2_TIPO,
            ISNULL(ZZ7.IntegraVEXP_Natureza, 'VAZIO') AS E2_NATUREZ,  
            '001' AS E2_PORTADO,
            ZZ7.IntegraVEXP_Fornecedor AS E2_FORNECE,  
            '01' AS E2_LOJA,  
            LEFT(ZZ7.IntegraVEXP_NomeFornecedor, 20) AS E2_NOMFOR,  
            FORMAT(MIN(ZZ7.IntegraVEXP_DataEmissao), 'yyyyMMdd') AS E2_EMISSAO,  
            FORMAT(MAX(ZZ7.IntegraVEXP_DataVencimento), 'yyyyMMdd') AS E2_VENCTO,  
            FORMAT(MAX(ZZ7.IntegraVEXP_DataVencimentoReal), 'yyyyMMdd') AS E2_VENCREA,  
            ISNULL(SUM(ZZ7.IntegraVEXP_VlrTitulo), 0) AS E2_VALOR,  
            0 AS E2_ISS,  
            0 AS E2_IRRF,  
            ISNULL(LEFT(ZZ7.IntegraVEXP_Historico, 40), 'N/A') AS E2_HIST,  
            ISNULL(SUM(ZZ7.IntegraVEXP_Saldo), 0) AS E2_SALDO,
            ISNULL(ZZ7.IntegraVEXP_UnidadeDeNegocio, 'N/A') AS E2_ITEMCTA,
            ISNULL(MAX(ZZ7.IntegraVEXP_CentroCusto), 'N/A') AS E2_CCUSTO,
            1 AS E2_MULTNAT
        FROM PROTHEUS_TESTE.dbo.ZZ7 AS ZZ7
        WHERE NOT EXISTS (
            SELECT 1
            FROM PROTHEUS_TESTE.dbo.SE2010 AS SE
            WHERE SE.E2_PREFIXO = ZZ7.IntegraVEXP_Prefixo
              AND SE.E2_NUM = ZZ7.IntegraVEXP_IDRelatorio
              AND SE.E2_TIPO = ZZ7.IntegraVEXP_Tipo
        )
        AND ZZ7.IntegraVEXP_Filial != '000000000'
        AND ZZ7.IntegraVEXP_Parcela IS NOT NULL
        AND (ZZ7.IntegraVEXP_StatusRelatorio = 'APROVADO' OR ZZ7.IntegraVEXP_StatusRelatorio = 'ENVIADO')
        GROUP BY 
            ZZ7.IntegraVEXP_IDRelatorio,
            ZZ7.IntegraVEXP_Filial,
            ZZ7.IntegraVEXP_Prefixo,
            ZZ7.IntegraVEXP_Parcela,
            ZZ7.IntegraVEXP_Tipo,
            ZZ7.IntegraVEXP_Natureza,
            ZZ7.IntegraVEXP_Fornecedor,
            ZZ7.IntegraVEXP_NomeFornecedor,
            ZZ7.IntegraVEXP_Historico,
            ZZ7.IntegraVEXP_UnidadeDeNegocio
        ORDER BY ZZ7.IntegraVEXP_IDRelatorio;

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
