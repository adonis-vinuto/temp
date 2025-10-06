"use client";

import {
  AgentSchema,
  Agent,
  AgentDetailSchema,
  AgentDetail,
  // PaginatedAgentsSchema,
  PaginatedAgents,
} from "@/types/schemas/agent.schema";
import { useModuleStore } from "@/lib/context/use-module-store.context";

// const AgentArraySchema = z.array(AgentSchema);

// function normalizeToList(res: unknown): Agent[] {
//   const paginated = PaginatedAgentsSchema.safeParse(res);
//   if (paginated.success) return paginated.data.itens;

//   const arr = AgentArraySchema.safeParse(res);
//   if (arr.success) return arr.data;

//   const one = AgentSchema.safeParse(res);
//   if (one.success) return [one.data];

//   const parsed = PaginatedAgentsSchema.or(AgentArraySchema)
//     .or(AgentSchema)
//     .safeParse(res);

//   if (!parsed.success) {
//     throw parsed.error;
//   }

//   return [];
// }

async function fetchAgentClient<T = unknown>(
  path: string,
  method: "GET" | "POST" | "PUT" | "DELETE" = "GET",
  body?: object | FormData
): Promise<T> {
  if (typeof window === "undefined") {
    throw new Error("fetchAgentClient não deve ser usado no servidor");
  }

  const currentModule = useModuleStore.getState().module;
  const url = `/api/agents${path}${
    path.includes("?") ? "&" : "?"
  }module=${currentModule}`;

  const isForm = body instanceof FormData;

  const res = await fetch(url, {
    method,
    headers: isForm ? undefined : { "Content-Type": "application/json" },
    body: isForm ? body : body ? JSON.stringify(body) : undefined,
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

export async function listAgents(
  pagina: number = 1,
  tamanhoPagina: number = 10
): Promise<PaginatedAgents> {
  return await fetchAgentClient<PaginatedAgents>(
    `?pagina=${pagina}&tamanhoPagina=${tamanhoPagina}`,
    "GET"
  );
}

export async function getAgent(id: string): Promise<AgentDetail> {
  const res = await fetchAgentClient(`/${id}`, "GET");
  return AgentDetailSchema.parse(res);
}

export async function createAgent(payload: Omit<Agent, "id">): Promise<Agent> {
  const res = await fetchAgentClient("", "POST", payload);
  return AgentSchema.parse(res);
}

export async function deleteAgent(id: string): Promise<void> {
  await fetchAgentClient(`/${id}`, "DELETE");
}
