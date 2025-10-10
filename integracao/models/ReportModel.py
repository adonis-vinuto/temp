class ReportModel:
    def __init__(self, db_connector):
        self.connection = db_connector.get_connection()

    def insert_report(self, report):
        query = """
        INSERT INTO VEXP_Reports (id, external_id, user_id, device_id, description, status, 
                                  approval_stage_id, approval_user_id, approval_date, paying_company_id, 
                                  payment_date, payment_method_id, observation, report_on, 
                                  justification, pdf_link, excel_link, created_at, updated_at)
        VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
        """
        values = (
            report['id'],
            report['external_id'],
            report['user_id'],
            report['device_id'],
            report['description'],
            report['status'],
            report['approval_stage_id'],
            report['approval_user_id'],
            parse_datetime_or_none(report['approval_date']),
            report['paying_company_id'],
            report['payment_date'],
            report['payment_method_id'],
            report['observation'],
            report['on'],
            report['justification'],
            report['pdf_link'],
            report['excel_link'],
            report['created_at'],
            report['updated_at']
        )

        #print(f"Cursor do INSERT")
        cursor = self.connection.cursor()
        try:
            cursor.execute(query, values)
            self.connection.commit()
            #print(f"Relatório {report['id']} inserido com sucesso!")
        except Exception as e:
            self.connection.rollback()
            print(f"Erro ao inserir relatório {report['id']}: {e}")
        finally:
            cursor.close()


    def get_last_updated_date(self):
        """
        Consulta a última data registrada na coluna updated_ate da tabela de despesas.
        Retorna a última data ou None se não houver registros.
        """
        query = "SET DATEFORMAT ymd; SELECT MAX(CAST(updated_at AS DATETIME)) AS MaxUpdatedAt FROM VEXP_Reports;"
        cursor = self.connection.cursor()

        try:
            cursor.execute(query)
            result = cursor.fetchone()

            if result and result[0]:
                return result[0].strftime('%Y-%m-%d')  # Retornar a data no formato YYYY-MM-DD
            return None
        
        except Exception as e:
            print(f"Erro ao consultar a última data de atualização: {e}")
            return None

        finally:
            cursor.close()


    def get_report_by_id(self, report_id):
        """
        Verifica se o relatório com o ID fornecido jé existe no banco de dados.
        Retorna o relatório encontrado ou None se não houver correspondência.
        """
        query = "SELECT * FROM VEXP_Reports WHERE id = ?"   #Correção do marcador de posição
        #print(f"Cursor do SELECT")
        cursor = self.connection.cursor()

        try:
            cursor.execute(query, (report_id,))
            result = cursor.fetchone()

            if result:
                #Retornar o relatório encontrado
                return{
                    'id': result[0],        # Assumindo que o ID é o primeiro campo
                    'updated_at': result[-1]        # Assumindo que updated_at é o último campo
                }
            return None

        except Exception as e:
            print(f"Erro ao consultar relatório por ID: {e}")
            return None

        finally:
            cursor.close()

    
    def update_report(self, report):
        """
        Atualiza um relatório existente no banco de dados com base no ID.
        """
        # Monta a query dinamicamente
        columns_to_update = [
            "id = ?",
	        "external_id = ?",
	        "user_id = ?",
	        "device_id = ?",
	        "description = ?",
	        "status = ?",
	        "approval_stage_id = ?",
	        "approval_user_id = ?",
	        "approval_date = ?",
	        "paying_company_id = ?",
	        "payment_date = ?",
	        "payment_method_id = ?",
	        "observation = ?",
	        "report_on = ?",
	        "justification = ?",
	        "pdf_link = ?",
	        "excel_link = ?",
	        "created_at = ?",
	        "updated_at = ?"
        ]

        values_to_update = [
            report.get('id'),                           # ID do relatório
	        report.get('external_id'),                  # ID externo do relatório
	        report.get('user_id'),                      # ID do usuário que submeteu o relatório
	        report.get('device_id'),                    # ID do dispositivo usado para criar o relatório
	        report.get('description'),                  # Descrição do relatório
	        report.get('status'),                       # Status do relatório
	        report.get('approval_stage_id'),            # ID da fase de aprovação
	        report.get('approval_user_id'),             # ID do usuário que aprovou o relatório
	        parse_datetime_or_none(report['approval_date']),                    # Data de aprovação
	        report.get('paying_company_id'),            # ID da empresa pagadora
	        report['payment_date'],                     # Data de pagamento
	        report.get('payment_method_id'),            # ID do método de pagamento
	        report.get('observation'),                  # Observações gerais
	        report.get('report_on'),                    # Indica se o relatório está ativo
	        report.get('justification'),                # Justificativa do relatório
	        report.get('pdf_link'),                     # Link para o PDF do relatório
	        report.get('excel_link'),                   # Link para o arquivo Excel do relatório
	        report.get('created_at'),                   # Data de criação do relatório
	        report.get('updated_at'),                   # Data da última atualização do relatório
        ]

        query = f"""
        UPDATE VEXP_Reports
        SET {",".join(columns_to_update)}
        WHERE id = ?
        """

        #print(f"Cursor do UPDATE")
        cursor = self.connection.cursor()

        try:
            # Adiciona ID do relatório no final dos valores
            values_to_update.append(report['id'])

            # Executar o update com os valores do relatório
            cursor.execute(query, tuple(values_to_update))
            self.connection.commit()
            #print(f"Relatório com ID {report['id']} atualizado com sucesso.")
        
        except Exception as e:
            print(f"Erro ao atualizar Relatório com ID {report['id']}: {e}")
            self.connection.rollback()

        finally:
            cursor.close()

from datetime import datetime

def parse_datetime_or_none(value):
    """
    Converte uma string datetime no formato 'YYYY-MM-DD HH:MM:SS' para objeto datetime.
    Retorna None se o valor for None ou string vazia.
    """
    if value in (None, "", "null"):
        return None
    try:
        return datetime.strptime(value, "%Y-%m-%d %H:%M:%S")
    except ValueError:
        # Caso venha só a data sem hora
        try:
            return datetime.strptime(value, "%Y-%m-%d")
        except Exception:
            return None