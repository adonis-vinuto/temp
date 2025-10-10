from datetime import datetime
from models.ApportionmentModel import ApportionmentModel
from models.CostCenterModel import CostCentersModel

class ExpenseModel:
    def __init__(self, db_connector):
        self.connection = db_connector.get_connection()
        self.apportionment_model = ApportionmentModel(db_connector)
        self.cost_centers_model = CostCentersModel(db_connector)
        
    def insert_expense(self, expense):
        # Determina o centro de custo
        cost_center_id = self._get_cost_center_id(expense)

        query = """
        INSERT INTO VEXP_Expenses (
            id, user_id, report_id, device_id, integration_id, external_id, 
            expense_type_id, payment_method_id, paying_company_id, route_id, 
            receipt_url, date, expense_value, title, expense_validate, 
            observation, rejected, expense_on, reimbursable, mileage, 
            mileage_value, original_currency_iso, exchange_rate, 
            converted_value, converted_currency_iso, created_at, updated_at, 
            cost_center_id
        )
        VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
        """
        values = (
            expense.get('id') or None,
            expense.get('user_id') or None,
            expense.get('report_id') or None,  # corrigido
            expense.get('device_id') or None,  
            expense.get('integration_id') or None,
            expense.get('external_id') or None,
            expense.get('expense_type_id') or None,
            expense.get('payment_method_id') or None,
            expense.get('paying_company_id') or None,
            expense.get('route_id') or None,
            expense.get('receipt_url') or None,
            expense.get('date') or None,
            expense.get('value') or None,
            expense.get('title') or None,  
            expense.get('expense_validate') or None,  
            expense.get('observation') or None,  
            int(expense.get('rejected', 0)),
            int(expense.get('expense_on', 0)),   # corrigido
            int(expense.get('reimbursable', 0)),
            expense.get('mileage') or None,
            expense.get('mileage_value') or None,
            expense.get('original_currency_iso') or None,
            expense.get('exchange_rate') or None,
            expense.get('converted_value') or None,
            expense.get('converted_currency_iso') or None,
            expense.get('created_at') or datetime.now(),
            expense.get('updated_at') or datetime.now(),
            cost_center_id
        )

        cursor = self.connection.cursor()
        try:
            cursor.execute(query, values)
            self.connection.commit()

            # Inserir rateios
            apportionments = expense.get('apportionment', [])
            for apportionment in apportionments:
                apportionment['expense_id'] = expense.get('id')
                self.apportionment_model.insert_apportionment(apportionment)

        except Exception as e:
            self.connection.rollback()
            print(f"❌ Erro ao inserir despesa {expense.get('id')}: {e}")
        finally:
            cursor.close()

    def _get_cost_center_id(self, expense):
        cost_center_id = expense.get('cost_center_id')
        if cost_center_id:
            cost_center = self.cost_centers_model.get_cost_center_by_id(cost_center_id)
            if cost_center:
                return cost_center['id']
            else:
                print(f"⚠️ Centro de Custo {cost_center_id} não encontrado.")
                return None
        return None

    def get_last_updated_date(self):
        query = "SET DATEFORMAT ymd; SELECT MAX(CAST(updated_at AS DATETIME)) AS MaxUpdatedAt FROM VEXP_Expenses;"
        cursor = self.connection.cursor()
        try:
            cursor.execute(query)
            result = cursor.fetchone()
            if result and result[0]:
                return result[0].strftime('%Y-%m-%d')
            return None
        except Exception as e:
            print(f"❌ Erro ao consultar última data de atualização: {e}")
            return None
        finally:
            cursor.close()

    def get_expense_by_id(self, expense_id):
        query = "SELECT * FROM VEXP_Expenses WHERE id = ?"
        cursor = self.connection.cursor()
        try:
            cursor.execute(query, (expense_id,))
            result = cursor.fetchone()
            if result:
                return {
                    'id': result[0],
                    'updated_at': result[-2]
                }
            return None
        except Exception as e:
            print(f"❌ Erro ao consultar despesa {expense_id}: {e}")
            return None
        finally:
            cursor.close()

    def update_expense(self, expense):
        columns = {
            "user_id": expense.get('user_id'),
            "report_id": expense.get('report_id'),
            "device_id": expense.get('device_id'),
            "integration_id": expense.get('integration_id'),
            "external_id": expense.get('external_id'),
            "expense_type_id": expense.get('expense_type_id'),
            "payment_method_id": expense.get('payment_method_id'),
            "paying_company_id": expense.get('paying_company_id'),
            "route_id": expense.get('route_id'),
            "receipt_url": expense.get('receipt_url'),
            "date": expense.get('date'),
            "expense_value": expense.get('value'),
            "title": expense.get('title'),
            "expense_validate": expense.get('expense_validate'),
            "observation": expense.get('observation'),
            "rejected": int(expense.get('rejected', 0)),
            "expense_on": int(expense.get('expense_on', 0)),
            "reimbursable": int(expense.get('reimbursable', 0)),
            "mileage": expense.get('mileage'),
            "mileage_value": expense.get('mileage_value'),
            "original_currency_iso": expense.get('original_currency_iso'),
            "exchange_rate": expense.get('exchange_rate'),
            "converted_value": expense.get('converted_value'),
            "converted_currency_iso": expense.get('converted_currency_iso'),
            "updated_at": datetime.now(),
            "cost_center_id": expense.get('cost_center_id')
        }

        # Mantém só os campos não nulos
        columns_to_update = {k: v for k, v in columns.items() if v is not None}

        set_clause = ", ".join([f"{column} = ?" for column in columns_to_update.keys()])
        query = f"UPDATE VEXP_Expenses SET {set_clause} WHERE id = ?"

        values_to_update = list(columns_to_update.values())
        values_to_update.append(expense.get('id'))

        cursor = self.connection.cursor()
        try:
            cursor.execute(query, tuple(values_to_update))
            self.connection.commit()

            # Atualizar ou inserir os rateios
            apportionments = expense.get('apportionment', [])
            for apportionment in apportionments:
                existing_apportionment = self.apportionment_model.get_apportionment_by_id(apportionment.get('id'))
                if existing_apportionment:
                    self.apportionment_model.update_apportionment(apportionment)
                else:
                    apportionment['expense_id'] = expense.get('id')
                    self.apportionment_model.insert_apportionment(apportionment)

        except Exception as e:
            self.connection.rollback()
            print(f"❌ Erro ao atualizar despesa {expense.get('id')}: {e}")
        finally:
            cursor.close()

    def get_all_expenses_ids(self):
        query = "SELECT id FROM VEXP_Expenses WHERE cost_center_id IS NULL"
        cursor = self.connection.cursor()
        try:
            cursor.execute(query)
            result = cursor.fetchall()
            return [row[0] for row in result]
        except Exception as e:
            print(f"❌ Erro ao buscar IDs de despesas: {e}")
            return []
        finally:
            cursor.close()
