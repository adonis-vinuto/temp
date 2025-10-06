// src/app/(app)/settings/page.tsx
'use client';

import { Card } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { PageHeader } from '@/components/page-header';
import { FormField } from '@/components/form-field';
import { useSettingsPage } from './_hooks/use-settings-page.hook';
import { LoadingState } from './_components/loading-state';
import { ErrorState } from './_components/error-state';

export default function SettingsPage() {
  const {
    form,
    isLoading,
    isError,
    error,
    existing,
    isPending,
    onSubmit
  } = useSettingsPage();

  if (isLoading) {
    return <LoadingState />;
  }

  return (
    <div className="p-6 space-y-6">
      <PageHeader
        title="Configurações"
        description={existing 
          ? 'Gerencie as configurações do sistema.'
          : 'Configure os dados necessários para habilitar o sistema.'}
        badge={existing && (
          <div className="flex items-center gap-2 text-sm text-green-600 dark:text-green-400">
            <span className="h-2 w-2 rounded-full bg-green-600 dark:bg-green-400 animate-pulse" />
            Sistema configurado
          </div>
        )}
      />

      <form
        onSubmit={form.handleSubmit(onSubmit)}
        className="grid grid-cols-1 md:grid-cols-2 gap-6"
      >
        <Card className="p-4 space-y-4 rounded-2xl border border-blue-600/20 bg-card text-card-foreground shadow-sm">
          <h2 className="text-xl font-semibold text-foreground">Banco de Dados (SQL) <span className='text-red-600'>*</span></h2>

          <FormField
            label="SQL Host"
            required
            placeholder="ex: db.mycorp.com"
            register={form.register('sqlHost')}
            error={form.formState.errors.sqlHost?.message}
          />

          <FormField
            label="SQL Port"
            required
            placeholder="ex: 1433"
            register={form.register('sqlPort')}
            error={form.formState.errors.sqlPort?.message}
          />

          <FormField
            label="SQL User"
            required
            placeholder="ex: admin"
            register={form.register('sqlUser')}
            error={form.formState.errors.sqlUser?.message}
          />

          <FormField
            label="SQL Password"
            required
            type="password"
            placeholder="••••••••"
            register={form.register('sqlPassword')}
            error={form.formState.errors.sqlPassword?.message}
          />

          <FormField
            label="SQL Database"
            required
            placeholder="ex: myapp_db"
            register={form.register('sqlDatabase')}
            error={form.formState.errors.sqlDatabase?.message}
          />

          <div className="text-xs text-foreground/80 pt-2">
            <span className='text-red-600'>*</span> Campos obrigatórios
          </div>
        </Card>

        <Card className="p-4 space-y-4 rounded-2xl border border-blue-600/20 bg-card text-card-foreground shadow-sm">
          <h2 className="text-lg font-semibold text-foreground">
            Armazenamento (Blob) 
            <span className="text-xs font-normal text-muted-foreground ml-2 text-foreground/60">
              Opcional
            </span>
          </h2>

          <FormField
            label="Blob Connection String"
            optional
            placeholder="ex: DefaultEndpointsProtocol=https;..."
            register={form.register('blobConnectionString')}
            error={form.formState.errors.blobConnectionString?.message}
          />

          <FormField
            label="Blob Container Name"
            optional
            placeholder="ex: uploads"
            register={form.register('blobContainerName')}
            error={form.formState.errors.blobContainerName?.message}
          />

          <div className="text-xs text-muted-foreground space-y-1 pt-4">
            <p className="text-xs text-foreground/80">
              As configurações de Blob Storage são opcionais mas necessárias
              para funcionalidades de upload de arquivos.
            </p>
            <p className="text-xs text-foreground/80">
              Você pode configurar estas opções posteriormente se necessário.
            </p>
          </div>
        </Card>

        <div className="md:col-span-2 flex justify-end">
          <Button
            type="submit"
            disabled={isPending}
            className="min-w-[150px] bg-foreground text-white hover:bg-foreground/80 "
          >
            {existing
              ? isPending
                ? 'Salvando...'
                : 'Salvar alterações'
              : isPending
              ? 'Criando...'
              : 'Criar Configuração'}
          </Button>
        </div>
      </form>

      {isError && <ErrorState error={error} />}
    </div>
  );
}
