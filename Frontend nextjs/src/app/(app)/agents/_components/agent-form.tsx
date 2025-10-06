"use client";

import { useState } from "react";
import { useAgentsPage } from "../_hooks/use-agents-page.hook";

interface AgentFormProps {
  onCancel: () => void;
}

export function AgentForm({ onCancel }: AgentFormProps) {
  const { handleCreate, creating } = useAgentsPage();

  const [name, setName] = useState("");
  const [description, setDescription] = useState("");

  async function handleSubmit(e: React.FormEvent<HTMLFormElement>) {
    e.preventDefault();

    await handleCreate({
      organization: "",
      module: 0,
      name,
      description,
      typeAgent: 0, 
    });
  }

  return (
    <form onSubmit={handleSubmit} className="space-y-6">
      <div className="space-y-4">
        <div>
          <label className="block text-sm font-medium">Nome</label>
          <input
            type="text"
            value={name}
            onChange={(e) => setName(e.target.value)}
            className="w-full border rounded-lg px-3 py-2 mt-1"
            required
          />
        </div>

        <div>
          <label className="block text-sm font-medium">Descrição</label>
          <textarea
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            className="w-full border rounded-lg px-3 py-2 mt-1"
            rows={3}
          />
        </div>
      </div>

      <button
        type="submit"
        disabled={creating}
        className="w-full px-4 py-3 bg-foreground text-white rounded-lg hover:bg-foreground/90 disabled:opacity-50"
      >
        {creating ? "Criando agente..." : "Criar agente"}
      </button>

      <button
        type="button"
        onClick={onCancel}
        className="w-full px-4 py-2 text-foreground/70 hover:text-foreground transition-colors text-sm"
      >
        Cancelar
      </button>
    </form>
  );
}
