"use client";

import type { Agent } from "@/types/schemas/agent.schema";
import type { TPaginated } from "@/types/schemas/pagination.schema";

interface AgentListProps {
  agents: TPaginated<Agent>;
  onNew: () => void;
  onPageChange: (page: number) => void;
  onSelectAgent?: (id: string) => void;
}

export function AgentList({ agents, onNew, onPageChange, onSelectAgent }: AgentListProps) {
  return (
    <div className="space-y-6">
      <div className="grid grid-cols-2 md:grid-cols-2 lg:grid-cols-3 gap-4">
        {/* Card de Novo */}
        <div
          className="bg-sidebar hover:bg-sidebar/80 text-white rounded-lg shadow hover:shadow-lg transition flex flex-col items-center justify-center p-4 h-32 cursor-pointer"
          onClick={onNew}
        >
          <span className="text-2xl font-bold">+</span>
          <p className="mt-2 text-sm font-semibold">Novo</p>
        </div>

        {/* Lista de Agentes */}
        {agents.itens.map((agent) => (
          <div
            key={agent.id}
            className="bg-sidebar hover:bg-sidebar/80 border border-blue-300/20 shadow-md text-white rounded-lg hover:shadow-lg transition flex flex-col items-center justify-center p-4 h-32 cursor-pointer"
            onClick={() => onSelectAgent?.(agent.id)}
          >
            <div className="text-lg font-bold truncate">{agent.name}</div>
            <p className="text-sm opacity-80">{agent.organization}</p>
            <span className="text-xs opacity-60">Módulo {agent.module}</span>
          </div>
        ))}
      </div>

      {/* Paginação */}
      <div className="flex justify-between items-center pt-4">
        <button
          className="px-3 py-1 bg-foreground hover:bg-foreground/80 text-white rounded disabled:opacity-50"
          disabled={agents.indice <= 1}
          onClick={() => onPageChange(Math.max(1, agents.indice - 1))}
        >
          Anterior
        </button>
        <span className="text-sm text-foreground/80">
          Página {agents.indice} de {agents.totalPaginas}
        </span>
        <button
          className="px-3 py-1 bg-foreground hover:bg-foreground/80 text-white rounded disabled:opacity-50"
          disabled={agents.indice >= agents.totalPaginas}
          onClick={() => onPageChange(agents.indice + 1)}
        >
          Próxima
        </button>
      </div>
    </div>
  );
}
