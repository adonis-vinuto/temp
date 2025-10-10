USE [PROTHEUS_HOMOLOGACAO]
GO

/****** Object:  View [dbo].[vw_integracao]    Script Date: 10/9/2025 1:22:19 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[vw_integracao] AS
SELECT
	'MAN' AS "Prefixo",                                	-- Origem: PROTHEUS
	VEXP_Expenses.id AS "ID Despesa",								-- Origem: VEXPENSES
	VEXP_Expenses.[date] AS "Data Despesa",                    	-- Origem: PROTHEUS
	VEXP_Expenses.title AS "N. Titulo",                        	-- Origem: VEXPENSES
	VEXP_Reports.id AS "ID Relatório",							-- Origem: VEXPENSES
	VEXP_Reports.description AS "Descrição Relatório",			-- Origem: VEXPENSES
	01 AS "Parcela",                              		-- Origem: VEXPENSES

	CASE 
        WHEN VEXP_Expenses.payment_method_id = 127367 THEN 'FTB'
        WHEN VEXP_Expenses.payment_method_id IN (189058, 127365) THEN 'REB'
    END AS "Tipo",                       			-- Origem: VEXPENSES
	
	CASE 
		WHEN VEXP_Currency.iso_code = 'BRL' THEN 'MV_MOEDA1'		-- De-para de Moedas do VExpenses -> Protheus
		WHEN VEXP_Currency.iso_code = 'USD' THEN 'MV_MOEDA2'		-- De-para de Moedas do VExpenses -> Protheus
		WHEN VEXP_Currency.iso_code = 'EUR' THEN 'MV_MOEDA4'		-- De-para de Moedas do VExpenses -> Protheus
	END AS "Moeda",										-- Origem: VEXPENSES

	VEXP_Expenses.expense_value AS "VlrTitulo",                	-- Origem: VEXPENSES
	
	CASE 
        WHEN VEXP_Expenses.payment_method_id = 127367 THEN 004936
        WHEN VEXP_Expenses.payment_method_id IN (189058, 127365) THEN VEXP_TeamMembers.id
    END AS "Fornecedor", 
	
	CASE 
        WHEN VEXP_Expenses.payment_method_id = 127367 THEN 'ITAUCARD S.A'
        WHEN VEXP_Expenses.payment_method_id IN (189058, 127365) THEN VEXP_TeamMembers.name
    END AS "Nome_Fornecedor",               	-- Origem: VEXPENSES
	
	CASE
		WHEN VEXP_Expenses.payment_method_id = 127367 THEN 10005	-- De-para Natureza Cartão Corporativo
		WHEN VEXP_Expenses.payment_method_id = 189058 THEN 10002	-- De-para Natureza Reembolsável
		WHEN VEXP_Expenses.payment_method_id = 127365 THEN 10002	-- De-para Natureza Reembolsável
	END	AS "Natureza",                               	-- Origem: PROTHEUS
	
	-- Captura a data atual
	CAST(GETDATE() AS DATE) AS "Data Emissão",        	-- Origem: PROTHEUS
    
    CASE
    -- Cartão corporativo (127367): vence no dia 23 com base no mês da approval_date
    WHEN VEXP_Expenses.payment_method_id = 127367 THEN
        CASE 
            WHEN DAY(VEXP_Reports.approval_date) < 23 THEN
                DATEFROMPARTS(YEAR(VEXP_Reports.approval_date), MONTH(VEXP_Reports.approval_date), 23)
            ELSE
                DATEADD(MONTH, 1, DATEFROMPARTS(YEAR(VEXP_Reports.approval_date), MONTH(VEXP_Reports.approval_date), 23))
        END

    -- Reembolsável (189058, 127365): quarta-feira da semana seguinte (seg–qui), 
    -- e quarta de duas semanas seguintes se for sexta; sáb/dom tratamos como duas semanas também
    WHEN VEXP_Expenses.payment_method_id IN (189058, 127365) THEN
        DATEADD(
            DAY,
            CASE 
                -- weekdayMon1 = ((DATEDIFF(DAY,0,approval_date) % 7) + 1), onde 1=seg ... 7=dom
                WHEN ((DATEDIFF(DAY, 0, VEXP_Reports.approval_date) % 7) + 1) BETWEEN 1 AND 4  -- seg..qui
                    THEN 10 - ((DATEDIFF(DAY, 0, VEXP_Reports.approval_date) % 7) + 1)        -- próxima quarta da semana seguinte (mín. 7 dias)
                WHEN ((DATEDIFF(DAY, 0, VEXP_Reports.approval_date) % 7) + 1) = 5             -- sexta
                    THEN 12                                                                    -- quarta de duas semanas seguintes
                WHEN ((DATEDIFF(DAY, 0, VEXP_Reports.approval_date) % 7) + 1) = 6             -- sábado
                    THEN 11                                                                    -- quarta de duas semanas seguintes
                WHEN ((DATEDIFF(DAY, 0, VEXP_Reports.approval_date) % 7) + 1) = 7             -- domingo
                    THEN 10                                                                    -- quarta de duas semanas seguintes
            END,
            CAST(VEXP_Reports.approval_date AS date)
        )
	END AS "Vencimento",

	--'' AS "Vencimento",                             	-- Origem: PROTHEUS
	'' AS "Vencimento Real",                        	-- Origem: PROTHEUS
	
	CASE
		WHEN VEXP_Expenses.payment_method_id = 127367 THEN CONCAT('Cartão Corporativo ', VEXP_TeamMembers.name)		-- De-para HISTORICO Cartão Corporativo
		WHEN VEXP_Expenses.payment_method_id = 189058 THEN CONCAT('Reembolso ', VEXP_TeamMembers.name)				-- De-para HISTORICO Reembolsável
		WHEN VEXP_Expenses.payment_method_id = 127365 THEN CONCAT('Reembolso ', VEXP_TeamMembers.name)				-- De-para HISTORICO Reembolsável
	END	AS "Historico",     -- Origem: PROTHEUS
	
	'' AS "Saldo",                                  	-- Origem: PROTHEUS
	VEXP_CostCenters.integration_id AS "Centro de Custo"             	-- Origem: VEXPENSES
	,VEXP_CostCenters.name AS "Nome Centro de Custo"	             	-- Origem: VEXPENSES
	
	,VEXP_TeamMembers.filial AS "Filial"
FROM
	VEXP_Expenses
    left JOIN VEXP_ExpenseTypes ON VEXP_Expenses.expense_type_id = VEXP_ExpenseTypes.id
	left JOIN VEXP_CostCenters ON VEXP_Expenses.cost_center_id = VEXP_CostCenters.id 
	left JOIN VEXP_TeamMembers ON VEXP_Expenses.user_id = VEXP_TeamMembers.id 
	left JOIN VEXP_Currency ON VEXP_Expenses.original_currency_iso = VEXP_Currency.iso_code
	left JOIN VEXP_Reports ON VEXP_Expenses.report_id = VEXP_Reports.id
;
GO