# models/data_inserter.py
from controllers.DatabaseConnector import DatabaseConnector
from models.DeParaUnidadeNegocioModel import DeParaUnidadeNegocio

class DataInserter:
    def __init__(self, config_file='bd2.config'):
        self.connector = DatabaseConnector(config_file)
        print("✅ Conectado ao BD Destino")

    def record_exists(self, cursor, integra_id):
        """
        Verifica se um registro já existe na ZZ7.
        Retorna o registro completo se existir.
        """
        check_query = """
        SELECT 
            IntegraVEXP_DataDespesa, 
            IntegraVEXP_Titulo, 
            IntegraVEXP_IDRelatorio, 
            IntegraVEXP_DescricaoRelatorio, 
            IntegraVEXP_VlrTitulo, 
            IntegraVEXP_CentroCusto
        FROM ZZ7 WHERE IntegraVEXP_IDDespesa = ?
        """
        cursor.execute(check_query, (integra_id,))
        return cursor.fetchone()

    def fields_are_different(self, db_record, record):
        """
        Compara os campos do banco de dados com os novos valores.
        Retorna True se os campos forem diferentes, False caso contrário.
        """
        return (
            db_record[0] != record.get("Data Despesa") or
            db_record[1] != record.get("N. Titulo") or
            db_record[2] != record.get("ID Relatório") or
            db_record[3] != record.get("Descrição Relatório") or
            db_record[4] != record.get("VlrTitulo") or
            db_record[5] != record.get("Centro de Custo")
        )

    def update_record(self, cursor, record):
        """
        Atualiza um registro existente no banco de dados.
        """
        update_query = """
        UPDATE ZZ7
        SET IntegraVEXP_Prefixo = ?, IntegraVEXP_DataDespesa = ?, IntegraVEXP_Titulo = ?, 
            IntegraVEXP_IDRelatorio = ?, IntegraVEXP_DescricaoRelatorio = ?, IntegraVEXP_Parcela = ?, 
            IntegraVEXP_Tipo = ?, IntegraVEXP_Moeda = ?, IntegraVEXP_VlrTitulo = ?, 
            IntegraVEXP_Fornecedor = ?, IntegraVEXP_Natureza = ?, IntegraVEXP_DataEmissao = ?, 
            IntegraVEXP_DataVencimento = ?, IntegraVEXP_DataVencimentoReal = ?, IntegraVEXP_Historico = ?, 
            IntegraVEXP_Saldo = ?, IntegraVEXP_CentroCusto = ?, IntegraVEXP_NomeCentroCusto = ?, 
            IntegraVEXP_UnidadeDeNegocio = ?
        WHERE IntegraVEXP_IDDespesa = ?
        """
        saldo_value = record.get('Saldo') or None

        fornecedor = record.get("Fornecedor")
        centro_custo = record.get("Centro de Custo")
        unidade_negocio = None

        # Buscar filial e mapear unidade de negócio
        select_query = "SELECT FILIAL, COD_USER FROM VW_USR_VEXP_PROT WHERE COD_USER = ?"
        cursor.execute(select_query, (fornecedor,))
        row = cursor.fetchone()

        if row:
            filial = row[0][:4] if row[0] else None
            depara = DeParaUnidadeNegocio()
            unidade_negocio = depara.get_value_by_centro_custo(centro_custo, filial)

        cursor.execute(update_query, (
            record.get("Prefixo"),
            record.get("Data Despesa"),
            record.get("N. Titulo"),
            record.get("ID Relatório"),
            record.get("Descrição Relatório"),
            record.get("Parcela"),
            record.get("Tipo"),
            record.get("Moeda"),
            record.get("VlrTitulo"),
            fornecedor,
            record.get("Natureza"),
            record.get("Data Emissão"),
            record.get("Vencimento"),
            record.get("Vencimento Real"),
            record.get("Historico"),
            saldo_value,
            centro_custo,
            record.get("Nome Centro de Custo"),
            unidade_negocio,
            record.get("ID Despesa")
        ))
        print(f"🔄 Registro {record.get('ID Despesa')} atualizado com sucesso.")

    def insert_data(self, data):
        """
        Insere ou atualiza dados na tabela ZZ7.
        """
        insert_query = """
        INSERT INTO ZZ7 (
            IntegraVEXP_Prefixo,
            IntegraVEXP_IDDespesa,
            IntegraVEXP_DataDespesa,
            IntegraVEXP_Titulo,
            IntegraVEXP_IDRelatorio,
            IntegraVEXP_DescricaoRelatorio,
            IntegraVEXP_Parcela,
            IntegraVEXP_Tipo,
            IntegraVEXP_Moeda,
            IntegraVEXP_VlrTitulo,
            IntegraVEXP_Fornecedor,
            IntegraVEXP_NomeFornecedor,
            IntegraVEXP_Natureza,
            IntegraVEXP_RateioNatureza,
            IntegraVEXP_DataEmissao,
            IntegraVEXP_DataVencimento,
            IntegraVEXP_DataVencimentoReal,
            IntegraVEXP_Historico,
            IntegraVEXP_Saldo,
            IntegraVEXP_CentroCusto,
            IntegraVEXP_NomeCentroCusto,
            IntegraVEXP_UnidadeDeNegocio,
            IntegraVEXP_Filial,
            IntegraVEXP_StatusRelatorio
        ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
        """
        conn = self.connector.get_connection()
        cursor = conn.cursor()
        try:
            for record in data:
                existing_record = self.record_exists(cursor, record.get("ID Despesa"))

                if existing_record:
                    if self.fields_are_different(existing_record, record):
                        self.update_record(cursor, record)
                    else:
                        print(f"✔️ Registro {record.get('ID Despesa')} já está atualizado.")
                else:
                    saldo_value = record.get('Saldo') or None
                    cursor.execute(insert_query, (
                        record.get("Prefixo"),
                        record.get("ID Despesa"),
                        record.get("Data Despesa"),
                        record.get("N. Titulo"),
                        record.get("ID Relatório"),
                        record.get("Descrição Relatório"),
                        record.get("Parcela"),
                        record.get("Tipo"),
                        record.get("Moeda"),
                        record.get("VlrTitulo"),
                        record.get("Fornecedor"),
                        record.get("Nome Fornecedor"),
                        record.get("Natureza"),
                        record.get("Rateio Natureza"),
                        record.get("Data Emissão"),
                        record.get("Vencimento"),
                        record.get("Vencimento Real"),
                        record.get("Historico"),
                        saldo_value,
                        record.get("Centro de Custo"),
                        record.get("Nome Centro de Custo"),
                        record.get("Unidade De Negocio"),
                        record.get("Filial"),
                        record.get("Status Relatorio")
                    ))
                    print(f"➕ Registro {record.get('ID Despesa')} inserido com sucesso.")

                conn.commit()

            print(f"🎯 Processo concluído: {len(data)} registros processados.")

        except Exception as e:
            conn.rollback()
            print(f"❌ Erro ao inserir dados: {e}")
            raise
        finally:
            cursor.close()
            conn.close()
