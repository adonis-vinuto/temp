// src/components/config-required-modal.tsx
'use client';

import { useRouter } from 'next/navigation';
import { AlertCircle, Database, Cloud } from 'lucide-react';
import { Button } from '@/components/ui/button';

export function ConfigRequiredModal() {
  const router = useRouter();

  const handleConfigure = () => {
    router.push('/settings');
  };

  return (
    <>
      <div 
        className="fixed inset-0 z-50 bg-black/80 backdrop-blur-sm"
        aria-hidden="true"
      />
      <div className="fixed inset-0 z-50 flex items-center justify-center p-4">
        <div className="relative max-w-lg w-full bg-background border rounded-lg shadow-lg">
          <div className="p-6 space-y-4">
            <div className="flex items-center gap-3">
              <div className="flex h-10 w-10 items-center justify-center rounded-full bg-orange-100 dark:bg-orange-900/20">
                <AlertCircle className="h-5 w-5 text-orange-600 dark:text-orange-400" />
              </div>
              <div>
                <h2 className="text-lg font-semibold">
                  Configuração Inicial Necessária
                </h2>
                <p className="text-sm text-muted-foreground">
                  Configure o banco de dados para começar
                </p>
              </div>
            </div>
            <div className="space-y-2">
              <p className="text-sm text-muted-foreground">
                Para utilizar o sistema, é necessário configurar a conexão com o banco de dados SQL.
              </p>
              <p className="text-sm text-muted-foreground">
                Esta é uma configuração única e essencial para o funcionamento da aplicação.
              </p>
            </div>
            <div className="space-y-3">
              <div className="rounded-md bg-orange-50 dark:bg-orange-900/10 border border-orange-200 dark:border-orange-900/30 p-3 space-y-1">
                <div className="flex items-center gap-2 text-sm font-medium text-orange-800 dark:text-orange-300">
                  <Database className="h-4 w-4" />
                  <span>Obrigatório</span>
                </div>
                <ul className="text-sm space-y-1 ml-6 text-orange-700 dark:text-orange-400">
                  <li className="flex items-center gap-2">
                    <span className="h-1.5 w-1.5 rounded-full bg-orange-500" />
                    Servidor SQL (host e porta)
                  </li>
                  <li className="flex items-center gap-2">
                    <span className="h-1.5 w-1.5 rounded-full bg-orange-500" />
                    Credenciais de acesso (usuário e senha)
                  </li>
                  <li className="flex items-center gap-2">
                    <span className="h-1.5 w-1.5 rounded-full bg-orange-500" />
                    Nome do banco de dados
                  </li>
                </ul>
              </div>
              <div className="rounded-md bg-muted/50 p-3 space-y-1">
                <div className="flex items-center gap-2 text-sm font-medium text-muted-foreground">
                  <Cloud className="h-4 w-4" />
                  <span>Opcional</span>
                </div>
                <ul className="text-sm space-y-1 ml-6 text-muted-foreground">
                  <li className="flex items-center gap-2">
                    <span className="h-1.5 w-1.5 rounded-full bg-gray-400" />
                    Blob Storage (para upload de arquivos)
                  </li>
                  <li className="flex items-center gap-2">
                    <span className="h-1.5 w-1.5 rounded-full bg-gray-400" />
                    Container de armazenamento
                  </li>
                </ul>
                <p className="text-xs text-muted-foreground ml-6 mt-1">
                  Pode ser configurado posteriormente
                </p>
              </div>
            </div>
            <div className="flex justify-end gap-3 pt-2">
              <Button 
                onClick={handleConfigure}
                className="min-w-[150px] bg-foreground text-white hover:bg-foreground/80"
              >
                Configurar Agora
              </Button>
            </div>
            <div className="text-xs text-center text-muted-foreground pt-2 border-t">
              A configuração leva apenas alguns minutos
            </div>
          </div>
        </div>
      </div>
    </>
  );
}
