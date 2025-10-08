from datetime import datetime

class ExpenseTypesModel:
    def __init__(self, db_connector):
        self.connection = db_connector.get_connection()

    def insert_expense_type(self, expense_type):
        query = """
        INSERT INTO VEXP_ExpenseTypes (
            id, integration_id, description, expensetype_on, created_at, updated_at
        )
        VALUES (?, ?, ?, ?, ?, ?)
        """
        values = (
            expense_type.get('id') or None,
            expense_type.get('integration_id') or None,
            expense_type.get('description') or None,
            int(expense_type.get('expensetype_on', 0)),  # garante BIT no SQLServer
            expense_type.get('created_at') or datetime.now(),
            expense_type.get('updated_at') or datetime.now()
        )

        cursor = self.connection.cursor()
        try:
            cursor.execute(query, values)
            self.connection.commit()
            # print(f"✅ Tipo de Despesa {expense_type.get('id')} inserido com sucesso!")
        except Exception as e:
            self.connection.rollback()
            print(f"❌ Erro ao inserir Tipo de Despesa {expense_type.get('id')}: {e}")
        finally:
            cursor.close()

    def get_last_updated_date(self):
        query = """
        SET DATEFORMAT ymd;
        SELECT MAX(CAST(updated_at AS DATETIME)) AS MaxUpdatedAt 
        FROM VEXP_ExpenseTypes;
        """
        cursor = self.connection.cursor()
        try:
            cursor.execute(query)
            result = cursor.fetchone()
            if result and result[0]:
                return result[0].strftime('%Y-%m-%d')
            return None
        except Exception as e:
            print(f"❌ Erro ao consultar a última data de atualização: {e}")
            return None
        finally:
            cursor.close()

    def get_expense_type_by_id(self, expense_type_id):
        query = "SELECT * FROM VEXP_ExpenseTypes WHERE id = ?"
        cursor = self.connection.cursor()
        try:
            cursor.execute(query, (expense_type_id,))
            result = cursor.fetchone()
            if result:
                return {
                    'id': result[0],
                    'integration_id': result[1],
                    'description': result[2],
                    'expensetype_on': result[3],
                    'created_at': result[4],
                    'updated_at': result[5]
                }
            return None
        except Exception as e:
            print(f"❌ Erro ao consultar Tipo de Despesa {expense_type_id}: {e}")
            return None
        finally:
            cursor.close()

    def update_expense_type(self, expense_type):
        columns_to_update = [
            "integration_id = ?",
            "description = ?",
            "expensetype_on = ?",
            "created_at = ?",
            "updated_at = ?"
        ]

        values_to_update = [
            expense_type.get('integration_id') or None,
            expense_type.get('description') or None,
            int(expense_type.get('expensetype_on', 0)),
            expense_type.get('created_at') or datetime.now(),
            datetime.now()  # força updated_at para agora
        ]

        query = f"""
        UPDATE VEXP_ExpenseTypes
        SET {", ".join(columns_to_update)}
        WHERE id = ?
        """

        cursor = self.connection.cursor()
        try:
            values_to_update.append(expense_type.get('id'))
            cursor.execute(query, tuple(values_to_update))
            self.connection.commit()
            # print(f"✅ Tipo de Despesa {expense_type.get('id')} atualizado com sucesso!")
        except Exception as e:
            self.connection.rollback()
            print(f"❌ Erro ao atualizar Tipo de Despesa {expense_type.get('id')}: {e}")
        finally:
            cursor.close()
