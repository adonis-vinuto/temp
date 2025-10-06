"use client";

import { useAgentDetails } from "../_hooks/use-agent-details.hook";
import { LoadingState } from "./loading-state";
import { ErrorState } from "./error-state";
import { Button } from "@/components/ui/button";
import { TrashIcon, MessageCircle } from "lucide-react";
import { deleteAgent } from "@/lib/api/agent.client";
import { toast } from "sonner";
import { useRouter } from "next/navigation";

interface AgentDetailsProps {
  agentId: string | null;
  onBack?: () => void;
  onDeleted?: () => void;
}

export function AgentDetails({ agentId, onBack, onDeleted }: AgentDetailsProps) {
  const { data: agentData, isLoading, isError, error } = useAgentDetails(agentId ?? "");
  const router = useRouter();

  if (!agentId) {
    return <div className="text-foreground/70">Selecione um agente para ver os detalhes.</div>;
  }

  if (isLoading) return <LoadingState />;
  if (isError) return <ErrorState error={error} />;
  if (!agentData) return null;

  const handleDelete = async () => {
    if (!agentData?.id) return;
    try {
      await deleteAgent(agentData.id);
      toast.success("Agente excluído com sucesso!");
      onDeleted?.();
      onBack?.();
    } catch (err) {
      console.error(err);
      toast.error("Erro ao excluir o agente.");
    }
  };

  return (
    <div className="space-y-4">
      <h3 className="text-xl font-semibold text-foreground">Detalhes do Agente</h3>

      <div className="space-y-2">
        <p><span className="font-medium">Nome:</span> {agentData.name}</p>
        <p><span className="font-medium">Organização:</span> {agentData.organization}</p>
        <p><span className="font-medium">Descrição:</span> {agentData.description ?? "N/A"}</p>
        {/* Removidos: Módulo e Tipo */}
      </div>

      <div className="flex gap-2">
        <Button
          variant="default"
          size="sm"
          className="w-32"
          onClick={() => router.push(`/agents/${agentData.id}/chat`)}
        >
          <MessageCircle size={16} className="mr-2" />
          Ir para Chat
        </Button>

        <Button
          variant="destructive"
          size="sm"
          className="w-28"
          onClick={handleDelete}
        >
          <TrashIcon size={16} className="mr-2" />
          Excluir
        </Button>
      </div>

      {onBack && (
        <button
          onClick={onBack}
          className="px-4 py-2 text-sm bg-gray-100 rounded hover:bg-gray-200"
        >
          Voltar
        </button>
      )}
    </div>
  );
}
