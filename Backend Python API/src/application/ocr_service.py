from PIL import Image, ImageEnhance, ImageFilter
import pytesseract
from typing import Union
import pypdfium2 as pdfium


def preprocess_image(image: Image.Image) -> Image.Image:
    """
    Pré-processa a imagem para melhorar a qualidade do OCR.
    Aumenta contraste, nitidez e reduz ruído.
    """
    # Converte para RGB se necessário
    if image.mode != 'RGB':
        image = image.convert('RGB')
    
    # Aumenta o contraste para melhorar legibilidade
    enhancer = ImageEnhance.Contrast(image)
    image = enhancer.enhance(1.5)
    
    # Aumenta a nitidez para caracteres mais definidos
    enhancer = ImageEnhance.Sharpness(image)
    image = enhancer.enhance(1.3)
    
    # Aplica filtro mediano para reduzir ruído mantendo bordas
    image = image.filter(ImageFilter.MedianFilter(size=3))
    
    return image


def ocr_service(image: Union[Image.Image, "pdfium.PdfPage"]) -> str:
    """
    Executa OCR otimizado para português com suporte a caracteres especiais (ç, ã, õ, etc).
    Se receber uma PdfPage, converte para PIL automaticamente em alta resolução.
    Retorna o texto extraído.
    """
    # Se for uma PdfPage do pypdfium2, converte para PIL com alta resolução
    # Scale 5.0 para máxima qualidade (360 DPI)
    if hasattr(image, 'render'):
        pil_image = image.render(scale=5.0).to_pil()

    else:
        pil_image = image
    
    # Pré-processa a imagem para melhor qualidade
    pil_image = preprocess_image(pil_image)
    
    # Configurações otimizadas do Tesseract
    # --oem 1: LSTM neural net (melhor para caracteres especiais)
    # --psm 1: Automatic page segmentation with OSD (Orientation and Script Detection)
    # -c tessedit_char_whitelist: Lista explícita de caracteres permitidos
    
    # Configuração base sem whitelist (evita problemas com aspas)
    # A whitelist causa problemas de parsing no Windows
    config = "--oem 1 --psm 1 -c preserve_interword_spaces=1"
    
    # Estratégia 1: Tenta português primeiro (se disponível)
    try:
        print("[INFO] Tentando OCR com português...")

        text = pytesseract.image_to_string(pil_image, lang='por', config=config).strip()

        if text and len(text) > 10:
            print(f"[INFO] ✓ OCR bem-sucedido com português")

            return text
        
    except pytesseract.pytesseract.TesseractError:
        print("[AVISO] Português não disponível, tentando alternativa...")
    
    # Estratégia 2: Tenta português+inglês combinados
    try:
        print("[INFO] Tentando OCR com por+eng...")

        text = pytesseract.image_to_string(pil_image, lang='por+eng', config=config).strip()

        if text and len(text) > 10:
            print(f"[INFO] ✓ OCR bem-sucedido com por+eng")

            return text
        
    except pytesseract.pytesseract.TesseractError:
        print("[AVISO] por+eng não disponível...")
    
    # Estratégia 3: Usa inglês com whitelist de caracteres portugueses
    # Isso permite que o motor reconheça acentos mesmo treinado em inglês
    try:
        print("[INFO] Tentando OCR com inglês + whitelist portuguesa...")

        text = pytesseract.image_to_string(pil_image, lang='eng', config=config).strip()

        if text and len(text) > 10:
            print(f"[INFO] ✓ OCR com inglês (whitelist portuguesa)")
            return text
        
    except Exception as e:
        print(f"[ERRO] Falha no OCR com inglês: {e}")
    
    # Estratégia 4: Último recurso - sem restrições
    try:
        print("[INFO] Tentando OCR sem restrições...")

        simple_config = "--oem 1 --psm 1"

        text = pytesseract.image_to_string(pil_image, config=simple_config).strip()

        if text and len(text) > 10:
            print(f"[INFO] ✓ OCR sem restrições")
            return text
            
    except Exception as e:
        print(f"[ERRO] Falha total no OCR: {e}")
        
    return ""