from datetime import datetime

class CurrenciesModel:
    def __init__(self, db_connector):
        self.connection = db_connector.get_connection()

    def truncate_currencies(self):
        """
        Remove todos os registros da tabela de moedas.
        """
        query = "DELETE FROM VEXP_Currency"  # use TRUNCATE TABLE se seu DB suportar
        cursor = self.connection.cursor()
        try:
            cursor.execute(query)
            self.connection.commit()
            #print("✅ Tabela VEXP_Currency truncada com sucesso.")
        except Exception as e:
            self.connection.rollback()
            print(f"❌ Erro ao truncar a tabela VEXP_Currency: {e}")
        finally:
            cursor.close()

    def insert_currencies(self, currency):
        query = """
        INSERT INTO VEXP_Currency(
            iso_code, priority, name, symbol, subunit, subunit_to_unit, 
            symbol_first, html_entity, decimal_mark, thousands_separator, iso_numeric
        )
        VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
        """
        values = (
            currency.get('iso_code') or None,
            currency.get('priority') or None,
            currency.get('name') or None,
            currency.get('symbol') or None,
            currency.get('subunit') or None,
            currency.get('subunit_to_unit') or None,
            int(currency.get('symbol_first', 0)),
            currency.get('html_entity') or None,
            currency.get('decimal_mark') or None,
            currency.get('thousands_separator') or None,
            currency.get('iso_numeric') or None
        )

        cursor = self.connection.cursor()
        try:
            cursor.execute(query, values)
            self.connection.commit()
            # print(f"✅ Moeda {currency.get('iso_code')} inserida com sucesso!")
        except Exception as e:
            self.connection.rollback()
            print(f"❌ Erro ao inserir Moeda {currency.get('iso_code')}: {e}")
        finally:
            cursor.close()

    def get_currency_by_iso(self, iso_code):
        """
        Retorna a moeda pelo código ISO ou None se não encontrada.
        """
        query = "SELECT * FROM VEXP_Currency WHERE iso_code = ?"
        cursor = self.connection.cursor()
        try:
            cursor.execute(query, (iso_code,))
            result = cursor.fetchone()
            if result:
                return {
                    'iso_code': result[0],
                    'priority': result[1],
                    'name': result[2],
                    'symbol': result[3],
                    'subunit': result[4],
                    'subunit_to_unit': result[5],
                    'symbol_first': result[6],
                    'html_entity': result[7],
                    'decimal_mark': result[8],
                    'thousands_separator': result[9],
                    'iso_numeric': result[10]
                }
            return None
        except Exception as e:
            print(f"❌ Erro ao consultar Moeda {iso_code}: {e}")
            return None
        finally:
            cursor.close()

    def update_currency(self, currency):
        """
        Atualiza uma moeda existente no BD, com base no ISO (PK).
        Não altera iso_code.
        """
        columns_to_update = [
            "priority = ?",
            "name = ?",
            "symbol = ?",
            "subunit = ?",
            "subunit_to_unit = ?",
            "symbol_first = ?",
            "html_entity = ?",
            "decimal_mark = ?",
            "thousands_separator = ?",
            "iso_numeric = ?"
        ]

        values_to_update = [
            currency.get('priority') or None,
            currency.get('name') or None,
            currency.get('symbol') or None,
            currency.get('subunit') or None,
            currency.get('subunit_to_unit') or None,
            int(currency.get('symbol_first', 0)),
            currency.get('html_entity') or None,
            currency.get('decimal_mark') or None,
            currency.get('thousands_separator') or None,
            currency.get('iso_numeric') or None
        ]

        query = f"""
        UPDATE VEXP_Currency
        SET {", ".join(columns_to_update)}
        WHERE iso_code = ?
        """

        cursor = self.connection.cursor()
        try:
            values_to_update.append(currency.get('iso_code'))
            cursor.execute(query, tuple(values_to_update))
            self.connection.commit()
            # print(f"✅ Moeda {currency.get('iso_code')} atualizada com sucesso.")
        except Exception as e:
            self.connection.rollback()
            print(f"❌ Erro ao atualizar Moeda {currency.get('iso_code')}: {e}")
        finally:
            cursor.close()
