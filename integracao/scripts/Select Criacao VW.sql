SELECT
	'man' AS "Prefixo",                                	-- Origem: PROTHEUS
	T0.id AS "ID Despesa",								-- Origem: VEXPENSES
	T0.[date] AS "Data Despesa",                    	-- Origem: PROTHEUS
	T0.title AS "N. Titulo",                        	-- Origem: VEXPENSES
	T4.id AS "ID Relatório",							-- Origem: VEXPENSES
	T4.description AS "Descrição Relatório",			-- Origem: VEXPENSES
	01 AS "Parcela",                              		-- Origem: VEXPENSES
	--T1.description AS "Tipo",                       	-- Origem: VEXPENSES
	'FTBREB' AS "Tipo",                       			-- Origem: VEXPENSES
	
	CASE 
		WHEN T5.iso_code = 'BRL' THEN 'MV_MOEDA1'		-- De-para de Moedas do VExpenses -> Protheus
		WHEN T5.iso_code = 'USD' THEN 'MV_MOEDA2'		-- De-para de Moedas do VExpenses -> Protheus
		WHEN T5.iso_code = 'EUR' THEN 'MV_MOEDA4'		-- De-para de Moedas do VExpenses -> Protheus
	END AS "Moeda",										-- Origem: VEXPENSES
	T0.expense_value AS "VlrTitulo",                	-- Origem: VEXPENSES
	T3.name AS "Fornecedor",                        	-- Origem: VEXPENSES
	
	CASE
		WHEN T0.payment_method_id = 127367 THEN 10005	-- De-para Natureza Cartão Corporativo
		WHEN T0.payment_method_id = 189058 THEN 10002	-- De-para Natureza Reembolsável
		WHEN T0.payment_method_id = 127365 THEN 10002	-- De-para Natureza Reembolsável
	END	AS "Natureza",                               	-- Origem: PROTHEUS
	
	'' AS "Data Emissão",                           	-- Origem: PROTHEUS
	'' AS "Vencimento",                             	-- Origem: PROTHEUS
	'' AS "Vencimento Real",                        	-- Origem: PROTHEUS
	
	CASE
		WHEN T0.payment_method_id = 127367 THEN CONCAT('PRESTAR CONTAS CC - ', T3.name)		-- De-para HISTORICO Cartão Corporativo
		WHEN T0.payment_method_id = 189058 THEN CONCAT('REEMBOLSO - ', T3.name)				-- De-para HISTORICO Reembolsável
		WHEN T0.payment_method_id = 127365 THEN CONCAT('REEMBOLSO - ', T3.name)				-- De-para HISTORICO Reembolsável
	END	AS "Historico",     -- Origem: PROTHEUS
	
	'' AS "Saldo",                                  	-- Origem: PROTHEUS
	T2.integration_id AS "Centro de Custo"             	-- Origem: VEXPENSES
	,T2.name AS "Nome Centro de Custo"	             	-- Origem: VEXPENSES
FROM
	VEXP_Expenses T0
	JOIN VEXP_ExpenseTypes T1 ON T0.expense_type_id = T1.id
	JOIN VEXP_CostCenters T2 ON T0.paying_company_id = T2.id 
	JOIN VEXP_TeamMembers T3 ON T0.user_id = T3.id 
	JOIN VEXP_Currency T5 ON T0.original_currency_iso = T5.iso_code
	JOIN VEXP_Reports T4 ON T0.report_id = T4.id
WHERE
	T0.user_id = 352114
;