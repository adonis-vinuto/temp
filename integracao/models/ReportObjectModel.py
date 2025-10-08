class ReportObjectModel:
    def __init__(self, db_connector):
        self.connection = db_connector.get_connection()

    def get_report_by_id(self, report_id):
        """
        Obtém um relatório com o ID fornecido a partir da tabela VEXP_Reports.
        """
        query = "SELECT * FROM VEXP_Reports WHERE id = ?"
        cursor = self.connection.cursor()

        try:
            cursor.execute(query, (report_id,))
            result = cursor.fetchone()

            if result:
                return {
                    'id': result[0],
                    'external_id': result[1],
                    'user_id': result[2],
                    'device_id': result[3],
                    'description': result[4],
                    'status': result[5],
                    'approval_stage_id': result[6],
                    'approval_user_id': result[7],
                    'approval_date': result[8],
                    'paying_company_id': result[9],
                    'payment_date': result[10],
                    'payment_method_id': result[11],
                    'observation': result[12],
                    'report_on': result[13],
                    'justification': result[14],
                    'pdf_link': result[15],
                    'excel_link': result[16],
                    'created_at': result[17],
                    'updated_at': result[18]
                }
            return None

        except Exception as e:
            print(f"Erro ao consultar relatório por ID: {e}")
            return None

        finally:
            cursor.close()
