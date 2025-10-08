class TeamMemberModel:
    def __init__(self, db_connector,db_connector2):
        self.connection = db_connector.get_connection()
        self.connection2 = db_connector2.get_connection()

    def insert_team_member(self, member):
        # Busca filial
        filial_Campo = "000000000"
        filial_parcial_campo = "0000"

        if member.get('integration_id') and str(member['integration_id']).strip():
            queryFilial = "SELECT FILIAL FROM VW_USR_VEXP_PROT WHERE COD_USER = ?"
            cursorFilial = self.connection2.cursor()
            try:
                cursorFilial.execute(queryFilial, (member['integration_id'],))
                result = cursorFilial.fetchone()
                if result and result[0]:
                    filial_Campo = result[0]
                    filial_parcial_campo = filial_Campo[:4]
            except Exception as e:
                print(f"Erro ao buscar filial: {e}")
            finally:
                cursorFilial.close()

        query = """
        INSERT INTO VEXP_TeamMembers (
            id, integration_id, external_id, company_id, role_id, 
            approval_flow_id, expense_limit_policy_id, user_type, name, 
            email, cpf_cnpj, phone1, phone2, birth_date, bank, agency, 
            account, confirmed, active, filial, filial_parcial
        )
        VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
        """

        values = (
            member.get('id') or None,
            member.get('integration_id') or None,
            member.get('external_id') or None,
            member.get('company_id') or None,
            member.get('role_id') or None,
            member.get('approval_flow_id') or None,
            member.get('expense_limit_policy_id') or None,
            member.get('user_type') or None,
            member.get('name') or None,
            member.get('email') or None,
            member.get('cpf_cnpj') or None,
            member.get('phone1') or None,
            member.get('phone2') or None,
            member.get('birth_date') or None,
            member.get('bank') or None,
            member.get('agency') or None,
            member.get('account') or None,
            member.get('confirmed', 0),
            member.get('active', 1),
            filial_Campo,
            filial_parcial_campo,
        )

        cursor = self.connection.cursor()
        try:
            cursor.execute(query, values)
            self.connection.commit()
        except Exception as e:
            self.connection.rollback()
            print(f"Erro ao inserir membro da equipe: {e}")
        finally:
            cursor.close()

    def get_team_member_by_id(self, team_member_id):
        """
        Verifica se o membro de equipe com o ID fornecido já existe no BD.
        Retorna o membro encontrado ou None se não houver correspondência.
        """
        query = "SELECT * FROM VEXP_TeamMembers WHERE id = ?"   # Marcador de posição
        cursor = self.connection.cursor()

        try:
            cursor.execute(query, (team_member_id,))
            result = cursor.fetchone()

            if result:
                # Retornar o membro de equipe encontrado
                return{
                    'id': result[0]                             # Assumindo que ID seja o primeiro campo da tabela
                }
            return None
        
        except Exception as e:
            print(f"Erro ao consultar Membro de Equipe por ID: {e}")
            return None
        
        finally:
            cursor.close()

    
    def update_team_member(self, team_member):
        # Busca filial
        filial = None
        filial_parcial = None
        if team_member.get('integration_id'):
            queryFilial = "SELECT FILIAL FROM VW_USR_VEXP_PROT WHERE COD_USER = ?"
            cursorFilial = self.connection2.cursor()
            try:
                cursorFilial.execute(queryFilial, (team_member['integration_id'],))
                result = cursorFilial.fetchone()
                if result and result[0]:
                    filial = result[0]
                    filial_parcial = filial[:4]
            except Exception as e:
                print(f"Erro ao buscar filial: {e}")
            finally:
                cursorFilial.close()

        query = """
        UPDATE VEXP_TeamMembers
        SET integration_id = ?, external_id = ?, company_id = ?, role_id = ?, 
            approval_flow_id = ?, expense_limit_policy_id = ?, user_type = ?, 
            name = ?, email = ?, cpf_cnpj = ?, phone1 = ?, phone2 = ?, birth_date = ?, 
            bank = ?, agency = ?, account = ?, confirmed = ?, active = ?, filial = ?, filial_parcial = ?
        WHERE id = ?
        """

        values = (
            team_member.get('integration_id') or None,
            team_member.get('external_id') or None,
            team_member.get('company_id') or None,
            team_member.get('role_id') or None,
            team_member.get('approval_flow_id') or None,
            team_member.get('expense_limit_policy_id') or None,
            team_member.get('user_type') or None,
            team_member.get('name') or None,
            team_member.get('email') or None,
            team_member.get('cpf_cnpj') or None,
            team_member.get('phone1') or None,
            team_member.get('phone2') or None,
            team_member.get('birth_date') or None,
            team_member.get('bank') or None,
            team_member.get('agency') or None,
            team_member.get('account') or None,
            team_member.get('confirmed', 0),
            team_member.get('active', 1),
            filial,
            filial_parcial,
            team_member.get('id')  # usado no WHERE
        )

        cursor = self.connection.cursor()
        try:
            cursor.execute(query, values)
            self.connection.commit()
        except Exception as e:
            print(f"Erro ao atualizar Membro de Equipe com ID {team_member.get('id')}: {e}")
            self.connection.rollback()
        finally:
            cursor.close()
