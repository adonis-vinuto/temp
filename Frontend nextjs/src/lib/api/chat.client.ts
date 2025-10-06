"use client";

import {
  ChatHistoryItemSchema,
  ChatHistoryItem,
  SessionHistorySchema,
  SessionHistory,
  FirstMessageRequest,
  FirstMessageResponseSchema,
  FirstMessageResponse,
  ContinueMessageRequest,
  ContinueMessageResponseSchema,
  ContinueMessageResponse,
} from "@/types/schemas/chat.schema";
import { useModuleStore } from "@/lib/context/use-module-store.context";

async function fetchChatClient<T = unknown>(
  path: string,
  method: "GET" | "POST" = "GET",
  body?: object,
  moduleInPath = false
): Promise<T> {
  if (typeof window === "undefined") {
    throw new Error("fetchChatClient não deve ser usado no servidor");
  }

  const currentModule = useModuleStore.getState().module;

  if (!currentModule) {
    throw new Error("Módulo não definido no useModuleStore");
  }

  const url = moduleInPath
    ? `/api/${currentModule}/chat${path}`
    : `/api/chat${path}${
        path.includes("?") ? "&" : "?"
      }module=${currentModule}`;

  const res = await fetch(url, {
    method,
    headers: { "Content-Type": "application/json" },
    body: body ? JSON.stringify(body) : undefined,
  });

  if (!res.ok) {
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

export async function getChatHistory(
  sessionId: string
): Promise<ChatHistoryItem[]> {
  const res = await fetchChatClient(`/history/${sessionId}`, "GET");
  return ChatHistoryItemSchema.array().parse(res);
}

export async function getSessionHistory(
  idAgent: string,
  idUser: string
): Promise<SessionHistory[]> {
  if (typeof window === "undefined") {
    throw new Error("getSessionHistory não deve ser usado no servidor");
  }

  const currentModule = useModuleStore.getState().module;

  if (!currentModule) {
    throw new Error("Módulo não definido no useModuleStore");
  }

  try {
    const res = await fetch(
      `/api/${currentModule}/session-history/${idAgent}/${idUser}`,
      {
        method: "GET",
        headers: { "Content-Type": "application/json" },
      }
    );

    if (!res.ok) {
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

    const data = (await res.json()) as unknown;
    return SessionHistorySchema.array().parse(data);
  } catch (err) {
    if (err instanceof Error && err.message.includes("não encontrado")) {
      return [];
    }
    throw err;
  }
}

export async function sendFirstMessage(
  idAgent: string,
  payload: FirstMessageRequest
): Promise<FirstMessageResponse> {
  const res = await fetchChatClient(
    `/first-message/${idAgent}`,
    "POST",
    payload
  );
  return FirstMessageResponseSchema.parse(res);
}

export async function sendMessage(
  idAgent: string,
  idSession: string,
  payload: ContinueMessageRequest
): Promise<ContinueMessageResponse> {
  const res = await fetchChatClient(
    `/${idAgent}/${idSession}`,
    "POST",
    payload,
    false // << agora o module vai na querystring, igual first-message
  );
  return ContinueMessageResponseSchema.parse(res);
}
