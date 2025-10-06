// src/types/schemas/pagination.schema.ts
import { z } from "zod";

export const PaginatedSchema = <T extends z.ZodTypeAny>(item: T) =>
  z.object({
    totalPaginas: z.number(),
    totalItens: z.number(),
    indice: z.number(),
    tamanhoPagina: z.number(),
    itens: z.array(item),
  });

export const PaginationParamsSchema = z.object({
  indice: z.number().min(0).default(0),
  tamanhoPagina: z.number().min(1).max(100).default(10),
  ordenarPor: z.string().optional(),
  ordem: z.enum(['asc', 'desc']).default('asc'),
});

export const PaginationMetaSchema = z.object({
  totalPaginas: z.number(),
  totalItens: z.number(),
  indice: z.number(),
  tamanhoPagina: z.number(),
  temProxima: z.boolean().optional(),
  temAnterior: z.boolean().optional(),
});

export type TPaginated<T> = {
  totalPaginas: number;
  totalItens: number;
  indice: number;
  tamanhoPagina: number;
  itens: T[];
};

export type TPaginationParams = z.infer<typeof PaginationParamsSchema>;
export type TPaginationMeta = z.infer<typeof PaginationMetaSchema>;

export function createPaginatedResponse<T>(
  itens: T[],
  meta: TPaginationMeta
): TPaginated<T> {
  return {
    ...meta,
    itens,
  };
}
