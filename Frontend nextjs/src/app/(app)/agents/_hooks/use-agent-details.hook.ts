"use client";

import { useQuery } from "@tanstack/react-query";
import { getAgent } from "@/lib/api/agent.client";
import type { AgentDetail } from "@/types/schemas/agent.schema";

export function useAgentDetails(id: string | null) {
  return useQuery<AgentDetail, Error>({
    queryKey: ["agent", id],
    queryFn: () => {
      if (!id) throw new Error("ID inválido");
      return getAgent(id);
    },
    enabled: !!id,
  });
}
