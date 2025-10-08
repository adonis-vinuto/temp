USE [hml_vexpenses]
GO

/****** Object:  View [dbo].[vw_integracao]    Script Date: 02/12/2024 02:25:53 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




CREATE VIEW [dbo].[vw_integracao] AS
SELECT
	'MAN' AS "Prefixo",                                	-- Origem: PROTHEUS
	T0.id AS "ID Despesa",								-- Origem: VEXPENSES
	T0.[date] AS "Data Despesa",                    	-- Origem: PROTHEUS
	T0.title AS "N. Titulo",                        	-- Origem: VEXPENSES
	T4.id AS "ID Relatório",							-- Origem: VEXPENSES
	T4.description AS "Descrição Relatório",			-- Origem: VEXPENSES
	01 AS "Parcela",                              		-- Origem: VEXPENSES

	CASE 
        WHEN T0.payment_method_id = 127367 THEN 'FTB'
        WHEN T0.payment_method_id IN (189058, 127365) THEN 'REB'
    END AS "Tipo",                       			-- Origem: VEXPENSES
	
	CASE 
		WHEN T5.iso_code = 'BRL' THEN 'MV_MOEDA1'		-- De-para de Moedas do VExpenses -> Protheus
		WHEN T5.iso_code = 'USD' THEN 'MV_MOEDA2'		-- De-para de Moedas do VExpenses -> Protheus
		WHEN T5.iso_code = 'EUR' THEN 'MV_MOEDA4'		-- De-para de Moedas do VExpenses -> Protheus
	END AS "Moeda",										-- Origem: VEXPENSES
	T0.expense_value AS "VlrTitulo",                	-- Origem: VEXPENSES
	
	CASE 
        WHEN T0.payment_method_id = 127367 THEN 004936
        WHEN T0.payment_method_id IN (189058, 127365) THEN T3.id
    END AS "Fornecedor",               	-- Origem: VEXPENSES
	
	CASE
		WHEN T0.payment_method_id = 127367 THEN 10005	-- De-para Natureza Cartão Corporativo
		WHEN T0.payment_method_id = 189058 THEN 10002	-- De-para Natureza Reembolsável
		WHEN T0.payment_method_id = 127365 THEN 10002	-- De-para Natureza Reembolsável
	END	AS "Natureza",                               	-- Origem: PROTHEUS
	
	-- Captura a data atual
	CAST(GETDATE() AS DATE) AS "Data Emissão",        	-- Origem: PROTHEUS
    
    CASE 
		-- Quando o método de pagamento for cartão corporativo
		-- Verifica se o dia atual < 23, então retorna o dia 23 do mês atual
		-- Se o dia atual >= 23, então retorna o dia 23 do mês seguinte
        WHEN T0.payment_method_id = 127367 THEN 
            CASE 
                WHEN DAY(GETDATE()) < 23 THEN 
                    DATEADD(DAY, 22 - DAY(GETDATE()), GETDATE())
                ELSE 
                    DATEADD(DAY, 22, DATEADD(MONTH, 1, DATEADD(DAY, -DAY(GETDATE()) + 1, GETDATE())))
            END
        WHEN T0.payment_method_id IN (189058, 127365) THEN 
            DATEADD(DAY, 7, GETDATE()) -- Dia Atual + 7
    END AS "Vencimento",

	--'' AS "Vencimento",                             	-- Origem: PROTHEUS
	'' AS "Vencimento Real",                        	-- Origem: PROTHEUS
	
	CASE
		WHEN T0.payment_method_id = 127367 THEN CONCAT('Cartão Corporativo ', T3.name)		-- De-para HISTORICO Cartão Corporativo
		WHEN T0.payment_method_id = 189058 THEN CONCAT('Reembolso ', T3.name)				-- De-para HISTORICO Reembolsável
		WHEN T0.payment_method_id = 127365 THEN CONCAT('Reembolso ', T3.name)				-- De-para HISTORICO Reembolsável
	END	AS "Historico",     -- Origem: PROTHEUS
	
	'' AS "Saldo",                                  	-- Origem: PROTHEUS
	T2.integration_id AS "Centro de Custo"             	-- Origem: VEXPENSES
	,T2.name AS "Nome Centro de Custo"	             	-- Origem: VEXPENSES
	,T6.UnidadeDeNegocio AS "Unidade De Negocio"
	,T3.filial AS "Filial"
FROM
	VEXP_Expenses T0
    left JOIN VEXP_ExpenseTypes T1 ON T0.expense_type_id = T1.id
	left JOIN VEXP_CostCenters T2 ON T0.cost_center_id = T2.id 
	left JOIN VEXP_TeamMembers T3 ON T0.user_id = T3.id 
	left JOIN VEXP_Currency T5 ON T0.original_currency_iso = T5.iso_code
	left JOIN VEXP_Reports T4 ON T0.report_id = T4.id
	left JOIN VEXP_UnidadeDeNegocio T6 ON T2.integration_id = T6.CentroDeCusto AND T3.filial_parcial = T6.CodigoDaEmpresa
;
GO


