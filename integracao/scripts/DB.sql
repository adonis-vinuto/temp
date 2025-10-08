/*
-- Endpoint Membros de Equipe (Team Members)
*/
CREATE TABLE VEXP_TeamMembers (
	id INTEGER NOT NULL PRIMARY KEY,            -- ID do membro da equipe
	integration_id VARCHAR(20),                 -- ID de integração externa
	external_id VARCHAR(100),                   -- ID externo do membro da equipe
	company_id VARCHAR(100),                         -- ID da empresa associada
	role_id VARCHAR(100),                            -- ID do papel/função do membro
	approval_flow_id VARCHAR(100),                   -- ID do fluxo de aprovação associado
	expense_limit_policy_id NUMERIC(18,2),      -- Limite de despesas para o membro
	user_type VARCHAR(50),                      -- Tipo de usuário (funcionário, prestador, etc.)
	name VARCHAR(200),                          -- Nome completo do membro
	email VARCHAR(200),                         -- Email do membro
	cpf_cnpj VARCHAR(14),                       -- CPF ou CNPJ (com letras) do membro
	phone1 VARCHAR(25),                         -- Telefone principal do membro (com DDI e DDD)
	phone2 VARCHAR(25),                         -- Telefone secundário do membro (com DDI e DDD)
	birth_date DATE,                            -- Data de nascimento do membro
	bank VARCHAR(100),                          -- Nome do banco associado
	agency VARCHAR(20),                         -- Número da agência bancária
	account VARCHAR(20),                        -- Número da conta bancária
	confirmed BIT,                              -- Indica se o membro foi confirmado
	active BIT,                                  -- Indica se o membro está ativo
	filial VARCHAR(100),                        -- Filial
	filial_parcial VARCHAR(100),                -- Filial parcial
);



/*
-- Endpoint Centro de Custos (Cost Centers)
*/
CREATE TABLE VEXP_CostCenters (
	id INTEGER NOT NULL PRIMARY KEY,             -- ID do centro de custo
	integration_id INTEGER,                      -- ID de integração externa
	name VARCHAR(200),                           -- Nome do centro de custo
	company_group_id INTEGER,                    -- ID do grupo da empresa
	costcenter_on BIT                            -- Indica se o centro de custo está ativo
);



/*
-- Endpoint Centro de Custos e Projetos (Projects)
*/
CREATE TABLE VEXP_Projects (
	id INTEGER NOT NULL PRIMARY KEY,             -- ID do projeto
	name VARCHAR(200),                           -- Nome do projeto
	company_name VARCHAR(200),                   -- Nome da empresa associada ao projeto
	cnpj VARCHAR(14),                            -- CNPJ da empresa do projeto
	address VARCHAR(300),                        -- Endereço da empresa do projeto
	neighborhood VARCHAR(300),                   -- Bairro da empresa do projeto
	city VARCHAR(300),                           -- Cidade da empresa do projeto
	state_uf VARCHAR(2),                         -- Estado (UF) da empresa do projeto
	zip_code VARCHAR(9),                         -- CEP da empresa do projeto
	phone1 VARCHAR(14),                          -- Telefone principal da empresa (com DDI e DDD)
	phone2 VARCHAR(14),                          -- Telefone secundário da empresa (com DDI e DDD)
	project_on BIT                               -- Indica se o projeto está ativo
);



/*
-- Endpoint Fluxos de Aprovação (Approval Flows)
*/
CREATE TABLE VEXP_ApprovalFlows (
	id INTEGER NOT NULL PRIMARY KEY,             -- ID do fluxo de aprovação
	company_id INTEGER,                          -- ID da empresa associada
	description VARCHAR(200),                    -- Descrição do fluxo de aprovação
	external_id VARCHAR(100),                    -- ID externo do fluxo de aprovação
	steps_operator VARCHAR(200),                 -- Operador do passo de aprovação
	steps_entrance_value INTEGER,                -- Valor de entrada do passo
	steps_order INTEGER                         -- Ordem do passo de aprovação
);

CREATE TABLE VEXP_ApprovalFlows_Groups (
	id_approval INTEGER NOT NULL,                -- ID do fluxo de aprovação
	groups_operator VARCHAR(200),                -- Operador do grupo de aprovação
	groups_approvers VARCHAR(200)                -- Aprovadores do grupo
);



/*
-- Endpoint Relatórios (Reports)
*/
CREATE TABLE VEXP_Reports (
	id INTEGER NOT NULL PRIMARY KEY,             -- ID do relatório
	external_id VARCHAR(100),                    -- ID externo do relatório
	user_id INTEGER,                             -- ID do usuário que submeteu o relatório
	device_id INTEGER,                           -- ID do dispositivo usado para criar o relatório
	description VARCHAR(500),                    -- Descrição do relatório
	status VARCHAR(50),                          -- Status do relatório
	approval_stage_id INTEGER,                   -- ID da fase de aprovação
	approval_user_id INTEGER,                    -- ID do usuário que aprovou o relatório
	approval_date VARCHAR(100),                  -- Data de aprovação
	paying_company_id INTEGER,                   -- ID da empresa pagadora
	payment_date VARCHAR(100),                   -- Data de pagamento
	payment_method_id INTEGER,                   -- ID do método de pagamento
	observation VARCHAR(2000),                   -- Observações gerais
	report_on BIT,                               -- Indica se o relatório está ativo
	justification VARCHAR(2000),                 -- Justificativa do relatório
	pdf_link VARCHAR(300),                       -- Link para o PDF do relatório
	excel_link VARCHAR(300),                     -- Link para o arquivo Excel do relatório
	created_at VARCHAR(100),                     -- Data de criação do relatório
	updated_at VARCHAR(100),                     -- Data da última atualização do relatório
);



/*
-- Endpoint Tipos de Despesas (Expense Types)
*/
CREATE TABLE VEXP_ExpenseTypes (
	id INTEGER NOT NULL PRIMARY KEY,             -- ID do tipo de despesa
	integration_id VARCHAR(500),                      -- ID de integração externa
	description VARCHAR(500),                    -- Descrição do tipo de despesa
	expensetype_on BIT,                                      -- Indica se o tipo de despesa está ativo
	created_at NVARCHAR(100),                         -- Data de criação do tipo de despesa
	updated_at NVARCHAR(100)                          -- Data da última atualização
);



/*
-- Endpoint de Moedas (Currency)
*/
CREATE TABLE VEXP_Currency (
    iso_code VARCHAR(3) NOT NULL PRIMARY KEY,    -- Código ISO da moeda (Ex: USD, EUR)
    priority INTEGER,                            -- Prioridade de uso da moeda
    name VARCHAR(100),                           -- Nome da moeda (Ex: Dólar, Euro)
    symbol VARCHAR(10),                          -- Símbolo da moeda (Ex: $, €)
    subunit VARCHAR(50),                         -- Nome da subunidade da moeda (Ex: Centavo)
    subunit_to_unit NUMERIC(18,2),               -- Fator de conversão da subunidade para a unidade
    symbol_first BIT,                            -- Indica se o símbolo vem antes do valor (0 = não, 1 = sim)
    html_entity VARCHAR(10),                     -- Representação HTML do símbolo da moeda
    decimal_mark VARCHAR(1),                     -- Símbolo usado para separar decimais (Ex: ".", ",")
    thousands_separator VARCHAR(1),              -- Símbolo usado para separar milhares (Ex: ".", ",")
    iso_numeric INTEGER                          -- Código numérico ISO da moeda (Ex: 840 para USD)
);



/*
-- Endpoint Despesas (Expenses)
*/
CREATE TABLE VEXP_Expenses (
	id INTEGER NOT NULL PRIMARY KEY,              -- ID da despesa
	user_id INTEGER,                              -- ID do usuário que submeteu a despesa
	report_id INTEGER,                            -- ID do relatório relacionado (chave estrangeira)
	device_id INTEGER,                            -- ID do dispositivo usado para criar a despesa
	integration_id INTEGER,                       -- ID de integração externa
	external_id VARCHAR(100),                     -- ID externo da despesa
	expense_type_id INTEGER,                      -- ID do tipo de despesa (chave estrangeira)
	payment_method_id INTEGER,                    -- Método de pagamento
	paying_company_id INTEGER,                    -- ID da empresa pagadora
	route_id INTEGER,                             -- ID da rota, se aplicável
	receipt_url VARCHAR(500),                     -- URL do recibo
	date VARCHAR(100),                            -- Data da despesa
	expense_value NUMERIC(18,2),                  -- Valor da despesa
	title VARCHAR(500),                           -- Título da despesa
	expense_validate VARCHAR(500),                -- Validação da despesa
	observation VARCHAR(3000),                    -- Observações da despesa
	rejected INTEGER,                             -- Indica se a despesa foi rejeitada (0 ou 1)
	expense_on BIT,                               -- Indica se a despesa está ativa
	reimbursable BIT,                             -- Indica se a despesa é reembolsável
	mileage NUMERIC(18,2),                        -- Quilometragem, se aplicável
	mileage_value NUMERIC(18,2),                  -- Valor da quilometragem
	original_currency_iso VARCHAR(3),             -- ISO da moeda original (chave estrangeira)
	exchange_rate NUMERIC(18,6),                  -- Taxa de câmbio
	converted_value NUMERIC(18,2),                -- Valor convertido
	converted_currency_iso VARCHAR(3),            -- ISO da moeda convertida
	created_at VARCHAR(100),                      -- Data de criação da despesa
	updated_at VARCHAR(100),                      -- Data da última atualização
	cost_center_id INTEGER                        -- ID do centro de custo relacionado (chave estrangeira)
	--FOREIGN KEY (report_id) REFERENCES VEXP_Reports(id),               -- FK para relatório
	--FOREIGN KEY (expense_type_id) REFERENCES VEXP_ExpenseTypes(id),     -- FK para tipo de despesa
	--FOREIGN KEY (original_currency_iso) REFERENCES VEXP_Currency(iso_code) -- FK para moeda original
);

/*
-- Endpoint Rateios (Apportionments)
*/
CREATE TABLE VEXP_Apportionments (
	id INTEGER NOT NULL PRIMARY KEY,             -- ID do rateio
	integration_id INTEGER,                      -- ID de integração externa
	expense_id INTEGER,                          -- ID da despesa relacionada (chave estrangeira)
	report_id INTEGER,                           -- ID do relatório relacionado (chave estrangeira)
	company_id INTEGER,                          -- ID da empresa envolvida no rateio
	cost_center_id INTEGER,                      -- ID do centro de custo relacionado (chave estrangeira)
	project_id INTEGER,                          -- ID do projeto relacionado (chave estrangeira)
	value NUMERIC(18,2),                         -- Valor do rateio
	value_percent NUMERIC(18,2),                 -- Percentual do rateio
	apportionment_on BIT,                        -- Indica se o rateio está ativo
	created_at DATETIME,                         -- Data de criação do rateio
	updated_at DATETIME                          -- Data da última atualização
	--FOREIGN KEY (expense_id) REFERENCES VEXP_Expenses(id)              -- FK para despesa
	--FOREIGN KEY (report_id) REFERENCES VEXP_Reports(id),                -- FK para relatório
	--FOREIGN KEY (cost_center_id) REFERENCES VEXP_CostCenters(id),       -- FK para centro de custo
	--FOREIGN KEY (project_id) REFERENCES VEXP_Projects(id)               -- FK para projeto
);



/*
-- Endpoint Adiantamentos (Advances)
*/
CREATE TABLE VEXP_Advances (
	id INTEGER NOT NULL PRIMARY KEY,              -- ID do adiantamento
	integration_id INTEGER,                       -- ID de integração externa
	report_id INTEGER,                            -- ID do relatório relacionado (chave estrangeira)
	user_id INTEGER,                              -- ID do usuário que solicitou o adiantamento
	value NUMERIC(18,2),                          -- Valor do adiantamento
	advance_on BIT,                               -- Indica se o adiantamento está ativo
	observation VARCHAR(2000),                    -- Observações gerais
	created_at DATETIME,                          -- Data de criação do adiantamento
	updated_at DATETIME                           -- Data da última atualização
	--FOREIGN KEY (report_id) REFERENCES VEXP_Reports(id) -- FK para relatório
);