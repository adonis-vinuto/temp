from datetime import datetime

class ApportionmentModel:
    def __init__(self, db_connector):
        self.connection = db_connector.get_connection()

    def insert_apportionment(self, apportionment):
        query = """
        INSERT INTO VEXP_Apportionments (
            id, integration_id, expense_id, report_id, company_id, 
            cost_center_id, project_id, value, value_percent, 
            apportionment_on, created_at, updated_at
        )
        VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
        """
        values = (
            apportionment.get('id') or None,
            apportionment.get('integration_id') or None,
            apportionment.get('expense_id') or None,
            apportionment.get('report_id') or None,
            apportionment.get('company_id') or None,
            apportionment.get('cost_center_id') or None,
            apportionment.get('project_id') or None,
            apportionment.get('value') or None,
            apportionment.get('value_percent') or None,
            int(apportionment.get('apportionment_on', 0)),  # padronizado
            apportionment.get('created_at') or datetime.now(),
            apportionment.get('updated_at') or datetime.now()
        )

        cursor = self.connection.cursor()
        try:
            cursor.execute(query, values)
            self.connection.commit()
            print(f"✅ Rateio {apportionment.get('id')} inserido com sucesso!")
        except Exception as e:
            self.connection.rollback()
            print(f"❌ Erro ao inserir Rateio {apportionment.get('id')}: {e}")
        finally:
            cursor.close()

    def get_apportionment_by_id(self, apportionment_id):
        query = "SELECT * FROM VEXP_Apportionments WHERE id = ?"
        cursor = self.connection.cursor()
        try:
            cursor.execute(query, (apportionment_id,))
            result = cursor.fetchone()
            if result:
                return {
                    'id': result[0],
                    'integration_id': result[1],
                    'expense_id': result[2],
                    'report_id': result[3],
                    'company_id': result[4],
                    'cost_center_id': result[5],
                    'project_id': result[6],
                    'value': result[7],
                    'value_percent': result[8],
                    'apportionment_on': result[9],
                    'created_at': result[10],
                    'updated_at': result[11]
                }
            return None
        except Exception as e:
            print(f"❌ Erro ao consultar Rateio {apportionment_id}: {e}")
            return None
        finally:
            cursor.close()

    def update_apportionment(self, apportionment):
        columns_to_update = [
            "integration_id = ?",
            "expense_id = ?",
            "report_id = ?",
            "company_id = ?",
            "cost_center_id = ?",
            "project_id = ?",
            "value = ?",
            "value_percent = ?",
            "apportionment_on = ?",
            "updated_at = ?"
        ]

        values_to_update = [
            apportionment.get('integration_id') or None,
            apportionment.get('expense_id') or None,
            apportionment.get('report_id') or None,
            apportionment.get('company_id') or None,
            apportionment.get('cost_center_id') or None,
            apportionment.get('project_id') or None,
            apportionment.get('value') or None,
            apportionment.get('value_percent') or None,
            int(apportionment.get('apportionment_on', 0)),  # padronizado
            apportionment.get('updated_at') or datetime.now()
        ]

        query = f"""
        UPDATE VEXP_Apportionments
        SET {", ".join(columns_to_update)}
        WHERE id = ?
        """

        cursor = self.connection.cursor()
        try:
            values_to_update.append(apportionment.get('id'))
            cursor.execute(query, tuple(values_to_update))
            self.connection.commit()
            print(f"✅ Rateio {apportionment.get('id')} atualizado com sucesso.")
        except Exception as e:
            self.connection.rollback()
            print(f"❌ Erro ao atualizar Rateio {apportionment.get('id')}: {e}")
        finally:
            cursor.close()
