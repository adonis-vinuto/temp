from datetime import datetime

class ApprovalFlowsModel:
    def __init__(self, db_connector):
        self.connection = db_connector.get_connection()

    def insert_approval_flows(self, approval_flow):
        query = """
        INSERT INTO VEXP_ApprovalFlows(
            id, company_id, description, external_id, 
            steps_operator, steps_entrance_value, steps_order
        )
        VALUES (?, ?, ?, ?, ?, ?, ?)
        """
        values = (
            approval_flow.get('id') or None,
            approval_flow.get('company_id') or None,
            approval_flow.get('description') or None,
            approval_flow.get('external_id') or None,
            approval_flow.get('steps_operator') or None,
            approval_flow.get('steps_entrance_value') or None,
            approval_flow.get('steps_order') or None
        )

        cursor = self.connection.cursor()
        try:
            cursor.execute(query, values)
            self.connection.commit()
            # print(f"✅ Fluxo de Aprovação {approval_flow.get('id')} inserido com sucesso!")
        except Exception as e:
            self.connection.rollback()
            print(f"❌ Erro ao inserir Fluxo de Aprovação {approval_flow.get('id')}: {e}")
        finally:
            cursor.close()

    def get_approval_flow_by_id(self, approval_flow_id):
        """
        Busca um fluxo de aprovação pelo ID.
        """
        query = "SELECT * FROM VEXP_ApprovalFlows WHERE id = ?"
        cursor = self.connection.cursor()

        try:
            cursor.execute(query, (approval_flow_id,))
            result = cursor.fetchone()

            if result:
                return {
                    'id': result[0],
                    'company_id': result[1],
                    'description': result[2],
                    'external_id': result[3],
                    'steps_operator': result[4],
                    'steps_entrance_value': result[5],
                    'steps_order': result[6]
                }
            return None
        except Exception as e:
            print(f"❌ Erro ao consultar Fluxo de Aprovação {approval_flow_id}: {e}")
            return None
        finally:
            cursor.close()

    def update_approval_flows(self, approval_flow):
        """
        Atualiza um fluxo de aprovação existente no BD, com base no ID.
        """
        columns_to_update = [
            "company_id = ?",
            "description = ?",
            "external_id = ?",
            "steps_operator = ?",
            "steps_entrance_value = ?",
            "steps_order = ?"
        ]

        values_to_update = [
            approval_flow.get('company_id') or None,
            approval_flow.get('description') or None,
            approval_flow.get('external_id') or None,
            approval_flow.get('steps_operator') or None,
            approval_flow.get('steps_entrance_value') or None,
            approval_flow.get('steps_order') or None
        ]

        query = f"""
        UPDATE VEXP_ApprovalFlows
        SET {", ".join(columns_to_update)}
        WHERE id = ?
        """

        cursor = self.connection.cursor()
        try:
            values_to_update.append(approval_flow.get('id'))
            cursor.execute(query, tuple(values_to_update))
            self.connection.commit()
            # print(f"✅ Fluxo de Aprovação {approval_flow.get('id')} atualizado com sucesso.")
        except Exception as e:
            self.connection.rollback()
            print(f"❌ Erro ao atualizar Fluxo de Aprovação {approval_flow.get('id')}: {e}")
        finally:
            cursor.close()
