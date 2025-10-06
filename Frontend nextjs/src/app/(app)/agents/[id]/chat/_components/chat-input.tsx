"use client";

import { Button } from "@/components/ui/button";

interface ChatInputProps {
  value: string;
  onChange: (v: string) => void;
  onSend: () => void;
  loading: boolean;
}

export function ChatInput({ value, onChange, onSend, loading }: ChatInputProps) {
  return (
    <form
      onSubmit={(e) => {
        e.preventDefault();
        onSend();
      }}
      className="flex gap-2 border-t p-4"
    >
      <input
        type="text"
        value={value}
        onChange={(e) => onChange(e.target.value)}
        placeholder="Digite sua mensagem..."
        className="flex-1 rounded-lg border px-3 py-2"
        disabled={loading}
      />
      <Button type="submit" disabled={loading || !value.trim()}> 
        {loading ? "Enviando..." : "Enviar"}
      </Button>
    </form>
  );
}

