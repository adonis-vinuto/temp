// src/lib/utils/error-parser.ts
const FIELD_TRANSLATIONS: Record<string, string> = {
  'BlobConnectionString': 'String de Conexão do Blob',
  'BlobContainerName': 'Nome do Container',
  'sqlHost': 'Host SQL',
  'sqlPort': 'Porta SQL',
  'sqlUser': 'Usuário SQL',
  'sqlPassword': 'Senha SQL',
  'sqlDatabase': 'Banco de Dados SQL',
  
  'name': 'Nome',
  'email': 'E-mail',
  'password': 'Senha',
  'phone': 'Telefone',
  'description': 'Descrição',
  'date': 'Data',
  'startDate': 'Data de Início',
  'endDate': 'Data de Término',
};

const HTTP_STATUS_MESSAGES: Record<string, string> = {
  '400': 'Requisição inválida. Verifique os dados enviados.',
  '401': 'Não autorizado. Faça login novamente.',
  '403': 'Sem permissão para realizar esta ação.',
  '404': 'Recurso não encontrado.',
  '409': 'Conflito. Este registro já existe.',
  '422': 'Dados inválidos. Verifique os campos do formulário.',
  '429': 'Muitas requisições. Aguarde um momento.',
  '500': 'Erro interno do servidor. Tente novamente.',
  '502': 'Erro de comunicação com o servidor.',
  '503': 'Serviço temporariamente indisponível.',
  '504': 'Tempo de resposta excedido. Tente novamente.',
};

interface ValidationError {
  field: string;
  message: string;
}

function extractValidationErrors(message: string): ValidationError[] {
  const errors: ValidationError[] = [];
  
  const errorsMatch = message.match(/"errors":\s*\{([^}]+)\}/);
  if (errorsMatch) {
    try {
      const errorString = errorsMatch[1];
      const fieldMatches = errorString.matchAll(/"([^"]+)":\s*\[?"([^"\]]+)"\]?/g);
      
      for (const match of fieldMatches) {
        const field = match[1];
        const msg = match[2];
        errors.push({
          field: FIELD_TRANSLATIONS[field] || field,
          message: msg
        });
      }
    } catch {
    }
  }
  
  if (errors.length === 0) {
    const titleMatch = message.match(/"title":\s*"([^"]+)"/);
    if (titleMatch) {
      return [{ field: 'Erro', message: titleMatch[1] }];
    }
  }
  
  return errors;
}

export function parseErrorMessage(error: unknown): string {
  if (!(error instanceof Error)) {
    return 'Erro desconhecido ao processar a requisição.';
  }
  
  const message = error.message;
  
  const httpMatch = message.match(/HTTP (\d+)/);
  if (httpMatch) {
    const statusCode = httpMatch[1];
    
    if (statusCode === '400' || statusCode === '422') {
      const validationErrors = extractValidationErrors(message);
      
      if (validationErrors.length > 0) {
        if (validationErrors.length === 1) {
          return validationErrors[0].message;
        }
        
        const errorList = validationErrors
          .map(err => `${err.field}: ${err.message}`)
          .join('; ');
        
        return `Erros de validação: ${errorList}`;
      }
    }
    
    return HTTP_STATUS_MESSAGES[statusCode] || `Erro do servidor (HTTP ${statusCode})`;
  }
  
  if (message.includes('Failed to fetch') || message.includes('NetworkError')) {
    return 'Erro de conexão. Verifique sua internet e tente novamente.';
  }
  
  if (message.includes('timeout') || message.includes('Timeout')) {
    return 'Tempo de resposta excedido. Tente novamente.';
  }
  
  return message;
}

export function getFieldErrorMessage(field: string, errors: ValidationError[]): string | undefined {
  const error = errors.find(err => err.field === field);
  return error?.message;
}

export function getToastMessage(error: unknown): {
  title: string;
  description?: string;
  duration?: number;
} {
  const message = parseErrorMessage(error);
  
  if (message.includes('validação') || message.includes('Verifique')) {
    return {
      title: 'Erro de Validação',
      description: message,
      duration: 5000,
    };
  }
  
  if (message.includes('autorizado') || message.includes('login')) {
    return {
      title: 'Sessão Expirada',
      description: message,
      duration: 4000,
    };
  }
  
  if (message.includes('servidor') || message.includes('HTTP 5')) {
    return {
      title: 'Erro no Servidor',
      description: message,
      duration: 4000,
    };
  }
  
  return {
    title: 'Erro',
    description: message,
    duration: 3000,
  };
}
