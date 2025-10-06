"use client";

import type { SessionHistory } from "@/types/schemas/chat.schema";
import { Button } from "@/components/ui/button";
import { cn } from "@/lib/utils";
import { Plus } from "lucide-react";

interface ChatSessionListProps {
  sessions: SessionHistory[];
  currentId: string | null;
  onSelect: (id: string) => void;
  onNew: () => void;
}

export function ChatSessionList({
  sessions,
  currentId,
  onSelect,
  onNew,
}: ChatSessionListProps) {
  return (
    <div className="flex h-full flex-col">
      <div className="p-2">
        <Button className="w-full" onClick={onNew}>
          <Plus /> Nova sessão
        </Button>
      </div>
      <div className="flex-1 overflow-y-auto space-y-1 p-2">
        {sessions.map((s) => (
          <div
            key={s.sessionId}
            onClick={() => onSelect(s.sessionId)}
            className={cn(
              "cursor-pointer rounded-md p-2 text-sm hover:bg-sidebar-accent hover:text-sidebar-accent-foreground",
              currentId === s.sessionId &&
                "bg-sidebar-accent text-sidebar-accent-foreground"
            )}
          >
            <div className="font-medium truncate">{s.title}</div>
            <div className="text-xs opacity-60">
              {new Date(s.lastSendDate).toLocaleString()}
            </div>
          </div>
        ))}
        {sessions.length === 0 && (
          <p className="p-2 text-sm text-foreground/70">
            Nenhuma sessão ainda.
          </p>
        )}
      </div>
    </div>
  );
}
