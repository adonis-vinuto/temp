import cv2
import numpy as np
import pypdfium2 as pdfium
import pytesseract
from PIL import Image
import re
import io
from typing import List, Dict, Optional

class AdvancedOcrProcessor:
    """OCR simplificado com apenas remoção de azul e grayscale"""
    
    def __init__(self, dpi: int = 300, language: str = 'por'):
        """
        Inicializa o processador OCR simplificado
        
        Args:
            dpi: Resolução para conversão PDF->imagem (padrão: 300)
            language: Idioma do Tesseract (padrão: 'por' para português)
        """
        self.scale = 3.0  # Mesmo scale do simple_ocr.py
        self.language = language
        self.tesseract_config = '--oem 3 --psm 6 -l por'
    
    def intelligent_blue_removal(self, image_rgb: np.ndarray) -> np.ndarray:
        """Remoção AGRESSIVA de assinatura azul/roxa/vermelha preservando texto preto"""
        
        print("    🧠 Remoção agressiva de assinatura colorida...")
        
        # Converte para HSV
        hsv = cv2.cvtColor(image_rgb, cv2.COLOR_RGB2HSV)
        
        # Range azul amplo
        lower_blue = np.array([100, 50, 50])
        upper_blue = np.array([130, 255, 255])
        blue_mask = cv2.inRange(hsv, lower_blue, upper_blue)
        
        # Range vermelho/roxo (dividido por causa do wrap do HSV)
        # Vermelho baixo (0-10)
        lower_red1 = np.array([0, 50, 50])
        upper_red1 = np.array([10, 255, 255])
        red_mask1 = cv2.inRange(hsv, lower_red1, upper_red1)
        
        # Vermelho alto (170-180) e roxo
        lower_red2 = np.array([160, 50, 50])
        upper_red2 = np.array([180, 255, 255])
        red_mask2 = cv2.inRange(hsv, lower_red2, upper_red2)
        
        # Combina todas as máscaras de cores
        color_mask = cv2.bitwise_or(blue_mask, red_mask1)
        color_mask = cv2.bitwise_or(color_mask, red_mask2)
        
        # Detecta texto preto (mais conservador para preservar texto)
        gray = cv2.cvtColor(image_rgb, cv2.COLOR_RGB2GRAY)
        text_mask = gray < 80  # Apenas pixels bem escuros (texto preto)
        
        # Remove cores EXCETO onde há texto muito escuro
        safe_color_mask = color_mask & (~text_mask)
        
        # Dilata a máscara para remover cores ao redor do texto também
        kernel_dilate = np.ones((3,3), np.uint8)
        safe_color_mask = cv2.dilate(safe_color_mask, kernel_dilate, iterations=1)
        
        result = image_rgb.copy()
        result[safe_color_mask > 0] = [255, 255, 255]  # Remove cores -> branco
        
        # Estatísticas como no simple_ocr.py
        colors_removed = np.sum(safe_color_mask > 0)
        blue_removed = np.sum(blue_mask > 0)
        red_removed = np.sum(cv2.bitwise_or(red_mask1, red_mask2) > 0)
        
        print(f"    🔵 {blue_removed:,} pixels azuis detectados")
        print(f"    🔴 {red_removed:,} pixels vermelhos detectados") 
        print(f"    🎨 {colors_removed:,} pixels coloridos removidos")
        
        return result
    
    def clean_text_output(self, text: str) -> str:
        """Limpa o texto removendo linhas com muito ruído"""
        
        lines = text.strip().split('\n')
        clean_lines = []
        
        for line in lines:
            clean_line = line.strip()
            if len(clean_line) < 2:
                continue
            
            # Remove linhas com muitos caracteres especiais consecutivos
            if re.search(r'[^A-Za-zÀ-ÿ0-9\s.,:/()-]{5,}', clean_line):
                continue
            
            # Remove linhas que são majoritariamente ruído
            valid_chars = len(re.findall(r'[A-Za-zÀ-ÿ0-9\s.,:/()-]', clean_line))
            if valid_chars < len(clean_line) * 0.6:  # Menos de 60% de caracteres válidos
                continue
            
            clean_lines.append(clean_line)
        
        return '\n'.join(clean_lines)
    
    def process_simple_ocr(self, image_rgb: np.ndarray) -> np.ndarray:
        """Pipeline SIMPLIFICADO: apenas remoção azul + grayscale"""
        
        print("🎯 Processamento SIMPLIFICADO - Apenas o essencial")
        print("=" * 55)
        
        # 1. Remove cores de forma inteligente
        blue_removed = self.intelligent_blue_removal(image_rgb)
        
        # 2. Converte para grayscale
        gray = cv2.cvtColor(blue_removed, cv2.COLOR_RGB2GRAY)
        
        return gray
    
    def run_ocr(self, image_gray: np.ndarray) -> dict:
        """Executa OCR na imagem processada"""
        
        print("\n🔤 Executando OCR...")
        
        try:
            pil_img = Image.fromarray(image_gray.astype(np.uint8), mode='L')
            raw_text = pytesseract.image_to_string(pil_img, config=self.tesseract_config)
            clean_text = self.clean_text_output(raw_text)
            
            # Métricas como no simple_ocr.py
            char_count = len(clean_text.strip())
            print(f"    ✓ Caracteres: {char_count}")
            
            return {
                'raw_text': raw_text,
                'clean_text': clean_text,
                'char_count': char_count
            }
            
        except Exception as e:
            print(f"    ❌ Erro no OCR: {str(e)}")
            return {
                'raw_text': '',
                'clean_text': '',
                'char_count': 0,
                'error': str(e)
            }
    
    def extract_text_from_pdf_bytes(self, pdf_bytes: bytes) -> List[Dict]:
        """
        Extrai texto de um PDF a partir de bytes usando processamento simplificado
        
        Args:
            pdf_bytes: Bytes do arquivo PDF
            
        Returns:
            Lista das páginas com texto extraído (dicionários)
        """
        try:
            # Converte PDF para imagens usando pypdfium2
            pdf = pdfium.PdfDocument(io.BytesIO(pdf_bytes))
            n_pages = len(pdf)
            
            result_pages = []
            
            for i in range(n_pages):
                try:
                    # Renderiza página como imagem (usando mesmo scale do simple_ocr.py)
                    page = pdf[i]
                    rendered = page.render(scale=self.scale)
                    if rendered is None:
                        result_pages.append({
                            'page': i + 1,
                            'text': "",
                            'raw_text': "",
                            'char_count': 0,
                            'error': "Falha na renderização da página"
                        })
                        continue
                    
                    # Converte para numpy array para processamento
                    image_rgb = rendered.to_numpy()
                    
                    # Aplica processamento simplificado
                    processed_image = self.process_simple_ocr(image_rgb)
                    
                    # Executa OCR
                    ocr_result = self.run_ocr(processed_image)
                    
                    result_pages.append({
                        'page': i + 1,
                        'text': ocr_result['clean_text'],
                        'raw_text': ocr_result['raw_text'],
                        'char_count': ocr_result['char_count'],
                        'error': ocr_result.get('error', None)
                    })

                except Exception as page_error:
                    result_pages.append({
                        'page': i + 1,
                        'text': "",
                        'raw_text': "",
                        'char_count': 0,
                        'error': str(page_error)
                    })

            return result_pages

        except Exception as e:
            raise Exception(f"Falha na extração de texto: {str(e)}")
    
    def extract_text_from_pdf_file(self, file_path: str) -> List[Dict]:
        """
        Extrai texto de um arquivo PDF
        
        Args:
            file_path: Caminho para o arquivo PDF
            
        Returns:
            Lista de resultados das páginas processadas (dicionários)
        """
        try:
            with open(file_path, 'rb') as pdf_file:
                pdf_bytes = pdf_file.read()
            return self.extract_text_from_pdf_bytes(pdf_bytes)
        except FileNotFoundError:
            raise Exception(f"Arquivo não encontrado: {file_path}")
        except Exception as e:
            raise Exception(f"Erro ao ler arquivo {file_path}: {str(e)}")
    
    def extract_text_simple(self, pdf_bytes: bytes) -> str:
        """
        Extrai texto simples (todas as páginas concatenadas)
        
        Args:
            pdf_bytes: Bytes do arquivo PDF
            
        Returns:
            Texto de todas as páginas concatenado
        """
        pages = self.extract_text_from_pdf_bytes(pdf_bytes)
        return "\n\n--- PAGE BREAK ---\n\n".join([page['text'] for page in pages])


# Factory function para facilitar o uso
def create_advanced_pdf_extractor() -> AdvancedOcrProcessor:
    """
    Cria um extrator PDF simplificado com configurações otimizadas
    
    Returns:
        Instância configurada do SimpleOCRProcessor
    """
    return AdvancedOcrProcessor()