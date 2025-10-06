// src\lib\api\file.client.ts
"use client";

import { z } from "zod";
import {
  FileSchema,
  FileResponse,
  UploadResultSchema,
  UploadResult,
} from "@/types/schemas/file.schema";
import { PaginatedSchema, TPaginated } from "@/types/schemas/pagination.schema";
import { useModuleStore } from "@/lib/context/use-module-store.context";

const PaginatedFileSchema = PaginatedSchema(FileSchema);
const FileArraySchema = z.array(FileSchema);

function normalizeToList(res: unknown): FileResponse[] {
  const paginated = PaginatedFileSchema.safeParse(res);
  if (paginated.success) return paginated.data.itens;

  const arr = FileArraySchema.safeParse(res);
  if (arr.success) return arr.data;

  const one = FileSchema.safeParse(res);
  if (one.success) return [one.data];

  const parsed = PaginatedFileSchema.or(FileArraySchema)
    .or(FileSchema)
    .safeParse(res);
  if (!parsed.success) {
    throw parsed.error;
  }
  return [];
}

async function fetchFileClient<T = unknown>(
  path: string,
  method: "GET" | "POST" | "PUT" | "DELETE" = "GET",
  body?: object | FormData
): Promise<T> {
  if (typeof window === "undefined") {
    throw new Error("fetchFileClient não deve ser usado no servidor");
  }

  const currentModule = useModuleStore.getState().module;
  const url = `/api/file${path}${
    path.includes("?") ? "&" : "?"
  }module=${currentModule}`;
  const isForm = body instanceof FormData;

  const res = await fetch(url, {
    method,
    headers: isForm ? undefined : { "Content-Type": "application/json" },
    body: isForm ? body : body ? JSON.stringify(body) : undefined,
  });

  if (!res.ok) {
    console.log("fetchFileClient error:", res);
    let error: unknown;
    try {
      error = await res.json();
    } catch {
      error = { error: `HTTP ${res.status}` };
    }

    if (typeof error === "object" && error !== null && "error" in error) {
      throw new Error((error as { error: string }).error);
    }

    throw new Error(`Erro inesperado (HTTP ${res.status})`);
  }
  return (await res.json()) as T;
}

export async function listFiles(
  pagina: number = 1,
  tamanhoPagina: number = 10
): Promise<TPaginated<FileResponse>> {
  return await fetchFileClient<TPaginated<FileResponse>>(
    `?pagina=${pagina}&tamanhoPagina=${tamanhoPagina}`,
    "GET"
  );
}

export async function getFile(id: string): Promise<FileResponse> {
  const res = await fetchFileClient(`/${id}`, "GET");
  const list = normalizeToList(res);
  const first = list[0];
  if (!first) throw new Error("Arquivo não encontrado");
  return first;
}

export async function createFile(file: File): Promise<UploadResult> {
  const formData = new FormData();
  formData.append("file", file);

  const res = await fetchFileClient("", "POST", formData);
  return UploadResultSchema.parse(res);
}

export async function deleteFile(id: string): Promise<void> {
  await fetchFileClient(`/${id}`, "DELETE");
}

export async function attachFileToAgent(
  id: string,
  payload: { idAgent: string }
) {
  await fetchFileClient(`/${id}/attach`, "PUT", payload);
}
