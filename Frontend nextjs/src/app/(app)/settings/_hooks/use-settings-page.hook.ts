// src\app\(app)\settings\_hooks\use-settings-page.hook.ts
import { useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { toast } from 'sonner';
import { useConfig } from '@/lib/providers/config-provider';
import { getToastMessage } from '@/lib/utils/error-parser.utils';
import { DataConfig, DataConfigCreateSchema, DataConfigCreate } from '@/types/schemas/data-config.schema';
import {
  createDataConfig,
  getDataConfig,
  updateDataConfig,
} from '@/lib/api/data-config.client';

export type FormData = DataConfigCreate;

export function useSettingsPage() {
  const router = useRouter();
  const qc = useQueryClient();
  const { refetch: refetchGlobalConfig } = useConfig();

  const { data, isLoading, isError, error } = useQuery({
    queryKey: ['data-config'],
    queryFn: () => getDataConfig(),
    staleTime: 0,
    gcTime: 5 * 60 * 1000,
    retry: 1,
  });

  const existing: DataConfig | null = data ?? null;

  const form = useForm<FormData>({
    resolver: zodResolver(DataConfigCreateSchema),
    defaultValues: {
      sqlHost: '',
      sqlPort: '',
      sqlUser: '',
      sqlPassword: '',
      sqlDatabase: '',
      blobConnectionString: '',
      blobContainerName: '',
    },
    mode: 'onBlur',
  });

  useEffect(() => {
    if (!existing) return;
    form.reset({
      sqlHost: existing.sqlHost ?? '',
      sqlPort: existing.sqlPort ?? '',
      sqlUser: existing.sqlUser ?? '',
      sqlPassword: existing.sqlPassword ?? '',
      sqlDatabase: existing.sqlDatabase ?? '',
      blobConnectionString: existing.blobConnectionString ?? '',
      blobContainerName: existing.blobContainerName ?? '',
    });
  }, [existing, form]);

  const { mutate: saveCreate, isPending: creating } = useMutation({
    mutationFn: (payload: FormData) => createDataConfig(payload),
    onSuccess: async () => {
      toast.success('Configuração criada com sucesso! Redirecionando...');
      await qc.invalidateQueries({ queryKey: ['data-config'] });
      await refetchGlobalConfig();
      
      setTimeout(() => {
        router.push('/dashboard');
      }, 1500);
    },
    onError: (error: unknown) => {
      const { title, description, duration } = getToastMessage(error);
      toast.error(description || title, { duration });
    },
  });

  const { mutate: saveUpdate, isPending: updating } = useMutation({
    mutationFn: (payload: FormData) => {
      if (!existing) throw new Error('Sem ID para editar.');
      return updateDataConfig(existing.id, payload);
    },
    onSuccess: async () => {
      toast.success('Configuração atualizada com sucesso!');
      await qc.invalidateQueries({ queryKey: ['data-config'] });
      await refetchGlobalConfig();
    },
    onError: (error: unknown) => {
      const { title, description, duration } = getToastMessage(error);
      toast.error(description || title, { duration });
    },
  });

  const onSubmit = (values: FormData) => {
    if (existing) {
      saveUpdate(values);
    } else {
      saveCreate(values);
    }
  };

  return {
    form,
    isLoading,
    isError,
    error,
    existing,
    isPending: creating || updating,
    onSubmit,
  };
}
