// src\lib\api\data-config.client.ts
"use client";

import { z } from "zod";
import { DataConfigSchema, DataConfig } from "@/types/schemas/data-config.schema";
import { PaginatedSchema } from "@/types/schemas/pagination.schema";

const PaginatedDataConfigSchema = PaginatedSchema(DataConfigSchema);
const DataConfigArraySchema = z.array(DataConfigSchema);

function normalizeToList(res: unknown): DataConfig[] {
  const paginated = PaginatedDataConfigSchema.safeParse(res);
  if (paginated.success) return paginated.data.itens;

  const arr = DataConfigArraySchema.safeParse(res);
  if (arr.success) return arr.data;

  const one = DataConfigSchema.safeParse(res);
  if (one.success) return [one.data];

  const parsed = PaginatedDataConfigSchema
    .or(DataConfigArraySchema)
    .or(DataConfigSchema)
    .safeParse(res);

  if (!parsed.success) {
    throw parsed.error;
  }
  return [];
}

export async function fetchDataConfigClient(
  endpoint: string,
  method: "GET" | "POST" | "PUT" = "GET",
  body?: Partial<DataConfig>
): Promise<unknown> {
  if (typeof window === "undefined") {
    throw new Error("fetchDataConfigClient não deve ser usado no servidor");
  }

  const url =
    method === "PUT"
      ? `/api/data-config/${endpoint.split("/").pop()}`
      : "/api/data-config";

  const response = await fetch(url, {
    method,
    headers: { "Content-Type": "application/json" },
    body: body ? JSON.stringify(body) : undefined,
  });

  if (!response.ok) {
    const error = await response.json();
    throw new Error(error.error || `HTTP ${response.status}`);
  }

  return response.json();
}

export async function getDataConfig(): Promise<DataConfig | null> {
  const res = await fetchDataConfigClient("/api/data-config", "GET");
  const list = normalizeToList(res);
  return list[0] ?? null;
}

export async function createDataConfig(payload: Partial<DataConfig>): Promise<DataConfig> {
  const res = await fetchDataConfigClient("/api/data-config", "POST", payload);
  const list = normalizeToList(res);
  const first = list[0];
  if (!first) throw new Error("Resposta inválida ao criar configuração");
  return first;
}

export async function updateDataConfig(id: string, payload: Partial<DataConfig>): Promise<DataConfig> {
  const res = await fetchDataConfigClient(`/api/data-config/${id}`, "PUT", payload);
  const list = normalizeToList(res);
  const first = list[0];
  if (!first) throw new Error("Resposta inválida ao atualizar configuração");
  return first;
}
