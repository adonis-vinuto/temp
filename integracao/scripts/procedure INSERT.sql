CREATE OR ALTER PROCEDURE dbo.sp_Inserir_Protheus
    @TabelaDestino VARCHAR(20)  -- FK7010 | SE2010 | SEV010
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @LastKeyValue INT;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF @TabelaDestino = 'FK7010'
        BEGIN
            -- Último valor da PK
            SELECT @LastKeyValue = ISNULL(MAX(R_E_C_N_O_), 0)
            FROM PROTHEUS_TESTE.dbo.SE2010;

            INSERT INTO PROTHEUS_HOMOLOGACAO.dbo.FK7010 (
                 FK7_FILIAL, FK7_IDDOC, FK7_ALIAS, FK7_CHAVE,
                 FK7_FILTIT, FK7_PREFIX, FK7_NUM, FK7_PARCEL,
                 FK7_TIPO, FK7_CLIFOR, FK7_LOJA, R_E_C_N_O_
            )
            SELECT 
                 EV_FILIAL,
                 CAST(EV_NUM AS VARCHAR(32)),
                 'SEV',
                 CAST(EV_PREFIXO + '-' + EV_NUM + '-' + EV_PARCELA + '-' + EV_TIPO AS VARCHAR(100)),
                 EV_FILIAL,
                 EV_PREFIXO,
                 EV_NUM,
                 EV_PARCELA,
                 EV_TIPO,
                 EV_CLIFOR,
                 EV_LOJA,
                 @LastKeyValue + ROW_NUMBER() OVER (ORDER BY (SELECT NULL))
            FROM PROTHEUS_TESTE.dbo.SEV010 SEV
            WHERE NOT EXISTS (
                SELECT 1
                FROM PROTHEUS_TESTE.dbo.FK7010 FK
                WHERE FK.FK7_PREFIX = SEV.EV_PREFIXO
                  AND FK.FK7_NUM    = SEV.EV_NUM
                  AND FK.FK7_TIPO   = SEV.EV_TIPO
                  AND FK.FK7_FILIAL = SEV.EV_FILIAL
                  AND FK.FK7_PARCEL = SEV.EV_PARCELA
            );
        END

        ELSE IF @TabelaDestino = 'SE2010'
        BEGIN
            SELECT @LastKeyValue = ISNULL(MAX(R_E_C_N_O_), 0)
            FROM PROTHEUS_TESTE.dbo.SE2010;

            INSERT INTO PROTHEUS_TESTE.dbo.SE2010 (
                R_E_C_N_O_, E2_FILIAL, E2_PREFIXO, E2_NUM, E2_PARCELA,
                E2_TIPO, E2_NATUREZ, E2_PORTADO, E2_FORNECE, E2_LOJA,
                E2_NOMFOR, E2_EMISSAO, E2_VENCTO, E2_VENCREA, E2_VALOR,
                E2_ISS, E2_IRRF, E2_HIST, E2_SALDO, E2_ITEMCTA,
                E2_CCUSTO, E2_MULTNAT
            )
            SELECT
                @LastKeyValue + ROW_NUMBER() OVER (ORDER BY (SELECT NULL)),
                ZZ7.IntegraVEXP_Filial,
                ZZ7.IntegraVEXP_Prefixo,
                ZZ7.IntegraVEXP_IDRelatorio,
                ISNULL(MAX(TRY_CAST(ZZ7.IntegraVEXP_Parcela AS INT)), 1),
                ZZ7.IntegraVEXP_Tipo,
                ISNULL(ZZ7.IntegraVEXP_Natureza, 'VAZIO'),
                '001',
                ZZ7.IntegraVEXP_Fornecedor,
                '01',
                LEFT(ZZ7.IntegraVEXP_NomeFornecedor, 20),
                FORMAT(MIN(ZZ7.IntegraVEXP_DataEmissao), 'yyyyMMdd'),
                FORMAT(MAX(ZZ7.IntegraVEXP_DataVencimento), 'yyyyMMdd'),
                FORMAT(MAX(ZZ7.IntegraVEXP_DataVencimentoReal), 'yyyyMMdd'),
                ISNULL(SUM(ZZ7.IntegraVEXP_VlrTitulo), 0),
                0,
                0,
                ISNULL(LEFT(ZZ7.IntegraVEXP_Historico, 40), 'N/A'),
                ISNULL(SUM(ZZ7.IntegraVEXP_Saldo), 0),
                ISNULL(ZZ7.IntegraVEXP_UnidadeDeNegocio, 'N/A'),
                ISNULL(MAX(ZZ7.IntegraVEXP_CentroCusto), 'N/A'),
                1
            FROM PROTHEUS_TESTE.dbo.ZZ7 ZZ7
            WHERE NOT EXISTS (
                SELECT 1
                FROM PROTHEUS_TESTE.dbo.SE2010 SE
                WHERE SE.E2_PREFIXO = ZZ7.IntegraVEXP_Prefixo
                  AND SE.E2_NUM = ZZ7.IntegraVEXP_IDRelatorio
                  AND SE.E2_TIPO = ZZ7.IntegraVEXP_Tipo
            )
            AND ZZ7.IntegraVEXP_Filial != '000000000'
            AND ZZ7.IntegraVEXP_Parcela IS NOT NULL
            AND ZZ7.IntegraVEXP_StatusRelatorio IN ('APROVADO','ENVIADO')
            GROUP BY 
                ZZ7.IntegraVEXP_IDRelatorio, ZZ7.IntegraVEXP_Filial,
                ZZ7.IntegraVEXP_Prefixo, ZZ7.IntegraVEXP_Parcela,
                ZZ7.IntegraVEXP_Tipo, ZZ7.IntegraVEXP_Natureza,
                ZZ7.IntegraVEXP_Fornecedor, ZZ7.IntegraVEXP_NomeFornecedor,
                ZZ7.IntegraVEXP_Historico, ZZ7.IntegraVEXP_UnidadeDeNegocio
            ORDER BY ZZ7.IntegraVEXP_IDRelatorio;
        END

        ELSE IF @TabelaDestino = 'SEV010'
        BEGIN
            SELECT @LastKeyValue = ISNULL(MAX(R_E_C_N_O_), 0)
            FROM PROTHEUS_TESTE.dbo.SEV010;

            INSERT INTO PROTHEUS_TESTE.dbo.SEV010 (
                R_E_C_N_O_, EV_FILIAL, EV_PREFIXO, EV_NUM, EV_PARCELA,
                EV_CLIFOR, EV_LOJA, EV_TIPO, EV_VALOR, EV_NATUREZ
            )
            SELECT 
                @LastKeyValue + ROW_NUMBER() OVER (ORDER BY (SELECT NULL)),
                ZZ7.IntegraVEXP_Filial,
                ZZ7.IntegraVEXP_Prefixo,
                ZZ7.IntegraVEXP_IDRelatorio,
                CAST(ZZ7.IntegraVEXP_Parcela AS VARCHAR(2)),
                '',
                '01',
                ZZ7.IntegraVEXP_Tipo,
                ISNULL(ZZ7.IntegraVEXP_VlrTitulo, 0),
                ISNULL(ZZ7.IntegraVEXP_RateioNatureza, '0')
            FROM PROTHEUS_TESTE.dbo.ZZ7 ZZ7
            WHERE NOT EXISTS (
                SELECT 1
                FROM PROTHEUS_TESTE.dbo.SEV010 SE
                WHERE SE.EV_PREFIXO = ZZ7.IntegraVEXP_Prefixo
                  AND SE.EV_NUM = ZZ7.IntegraVEXP_IDRelatorio
                  AND SE.EV_TIPO = ZZ7.IntegraVEXP_Tipo
            )
            AND ZZ7.IntegraVEXP_Filial != '000000000'
            AND ZZ7.IntegraVEXP_StatusRelatorio IN ('APROVADO','ENVIADO');
        END

        ELSE
        BEGIN
            RAISERROR('TabelaDestino inválida. Use FK7010, SE2010 ou SEV010.', 16, 1);
        END

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
