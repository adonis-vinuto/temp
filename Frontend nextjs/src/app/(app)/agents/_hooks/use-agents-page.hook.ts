"use client";

import { useState } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { listAgents, createAgent } from "@/lib/api/agent.client";
import type { Agent } from "@/types/schemas/agent.schema";
import type { TPaginated } from "@/types/schemas/pagination.schema";
import { toast } from "sonner";

export function useAgentsPage() {
  const queryClient = useQueryClient();
  const [pagina, setPagina] = useState(1);
  const [tamanhoPagina] = useState(10);
  const [isCreating, setIsCreating] = useState(false);

  const {
    data: agents,
    isLoading,
    isError,
    error,
  } = useQuery<TPaginated<Agent>, Error>({
    queryKey: ["agents", pagina, tamanhoPagina],
    queryFn: () => listAgents(pagina, tamanhoPagina),
  });

  const { mutateAsync: createAgentMutation, isPending: creating } = useMutation({
    mutationFn: async (payload: Omit<Agent, "id">) => {
      return await createAgent(payload);
    },
    onSuccess: () => {
      toast.success("Agente criado com sucesso!");
      queryClient.invalidateQueries({ queryKey: ["agents"] });
      setIsCreating(false);
    },
    onError: (err: unknown) => {
      toast.error(
        err instanceof Error ? err.message : "Não foi possível criar o agente."
      );
    },
  });

  async function handleCreate(payload: Omit<Agent, "id">) {
    await createAgentMutation(payload);
  }

  return {
    agents,
    isLoading,
    isError,
    error,
    pagina,
    tamanhoPagina,
    setPagina,
    isCreating,
    setIsCreating,
    creating,
    handleCreate,
  };
}
