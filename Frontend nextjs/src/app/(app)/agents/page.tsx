"use client";

import { useState } from "react";
import { PageHeader } from "@/components/page-header";
import { useAgentsPage } from "./_hooks/use-agents-page.hook";
import { AgentList } from "./_components/agent-list";
import { AgentForm } from "./_components/agent-form";
import { AgentDetails } from "./_components/agent-details";
import { LoadingState } from "./_components/loading-state";
import { ErrorState } from "./_components/error-state";

export default function AgentsPage() {
  const {
    agents,
    isLoading,
    isError,
    error,
    setPagina,
    isCreating,
    setIsCreating,
  } = useAgentsPage();

  const [selectedAgentId, setSelectedAgentId] = useState<string | null>(null);

  return (
    <div className="p-6 space-y-6">
      <PageHeader
        title="Agentes"
        description="Gerencie e visualize seus agentes"
      />

      <div className="bg-white rounded-xl p-6 grid grid-cols-2 gap-6 border border-blue-600/20 text-card-foreground shadow-sm">
        {/* Coluna da lista */}
        <div className="space-y-6 border-r pr-6 border-foreground/30">
          {isLoading && <LoadingState />}
          {isError && <ErrorState error={error} />}
          {agents && (
            <AgentList
              agents={agents}
              onNew={() => {
                setIsCreating(true);
                setSelectedAgentId(null);
              }}
              onSelectAgent={(id) => {
                setSelectedAgentId(id);
                setIsCreating(false);
              }}
              onPageChange={setPagina}
            />
          )}
          {!isLoading && !isError && !agents && (
            <p className="text-foreground/80">Nenhum agente encontrado.</p>
          )}
        </div>

        {/* Coluna de detalhes ou criação */}
        <div className="pl-6">
          {isCreating ? (
            <div className="space-y-6">
              <div>
                <h3 className="text-xl font-semibold text-foreground mb-2">
                  Criar Novo Agente
                </h3>
                <p className="text-sm text-foreground/60">
                  Preencha as informações abaixo para cadastrar um novo agente.
                </p>
              </div>
              <AgentForm onCancel={() => setIsCreating(false)} />
            </div>
          ) : selectedAgentId ? (
            <AgentDetails
              agentId={selectedAgentId}
              onBack={() => setSelectedAgentId(null)}
            />
          ) : (
            <div>
              <h3 className="text-xl font-semibold text-foreground mb-4">
                Detalhes
              </h3>
              <p className="text-foreground/70">
                Clique em <strong>Novo</strong> para adicionar um agente, ou
                selecione um existente na lista para ver os detalhes.
              </p>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
