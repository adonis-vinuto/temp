"use client";

import { useState } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import {
  getSessionHistory,
  getChatHistory,
  sendFirstMessage,
  sendMessage,
} from "@/lib/api/chat.client";
import type {
  ChatHistoryItem,
  SessionHistory,
} from "@/types/schemas/chat.schema";

export function useAgentChat(
  agentId: string | undefined,
  userId: string | undefined
) {
  const queryClient = useQueryClient();
  const [sessionId, setSessionId] = useState<string | null>(null);

  const { data: sessions = [] } = useQuery<SessionHistory[]>({
    queryKey: ["session-history", agentId, userId],
    queryFn: () => getSessionHistory(agentId!, userId!),
    enabled: !!agentId && !!userId,
  });

  const { data: messages = [] } = useQuery<ChatHistoryItem[]>({
    queryKey: ["chat-history", agentId, sessionId],
    queryFn: () => getChatHistory(sessionId!),
    enabled: !!agentId && !!sessionId,
  });

  const [pendingMessages, setPendingMessages] = useState<ChatHistoryItem[]>([]);

  const { mutateAsync: sendMutation, isPending: loading } = useMutation({
    mutationFn: async (content: string) => {
      const now = new Date().toISOString();
      if (!sessionId) {
        const res = await sendFirstMessage(agentId!, { message: content });
        return { type: "first" as const, res, content, now };
      }
      const res = await sendMessage(agentId!, sessionId, { message: content });
      return { type: "continue" as const, res, content, now };
    },
    onSuccess: ({ type, res, content, now }) => {
      // Limpa mensagens pendentes
      setPendingMessages([]);

      if (type === "first") {
        setSessionId(res.sessionId);
        queryClient.setQueryData<SessionHistory[]>(
          ["session-history", agentId, userId],
          (old = []) => [
            {
              sessionId: res.sessionId,
              lastSendDate: now,
              totalInteractions: "1",
              title: "Novo Chat",
            },
            ...old,
          ]
        );
        queryClient.setQueryData<ChatHistoryItem[]>(
          ["chat-history", agentId, res.sessionId],
          [
            {
              role: 0,
              content,
              sendDate: now,
              usage: { totalTokens: 0, totalTime: 0 },
            },
            {
              role: 1,
              content: res.messageResponse,
              sendDate: now,
              usage: { totalTokens: 0, totalTime: 0 },
            },
          ]
        );
      } else {
        queryClient.setQueryData<ChatHistoryItem[]>(
          ["chat-history", agentId, sessionId],
          (old = []) => [
            ...(old ?? []).filter((msg) => msg.content !== "_Typing..._"), // remove placeholder
            {
              role: 0,
              content,
              sendDate: now,
              usage: { totalTokens: 0, totalTime: 0 },
            },
            {
              role: 1,
              content: res.messageResponse,
              sendDate: now,
              usage: { totalTokens: 0, totalTime: 0 },
            },
          ]
        );
      }
    },
  });

  function selectSession(id: string) {
    setSessionId(id);
  }

  function startNewSession() {
    setSessionId(null);
    queryClient.removeQueries({ queryKey: ["chat-history", agentId] });
  }

  async function send(content: string) {
    if (!agentId || !content.trim()) return;

    const now = new Date().toISOString();

    // Adiciona mensagem do usuário + placeholder da IA no estado local
    const optimisticUserMessage: ChatHistoryItem = {
      role: 0,
      content,
      sendDate: now,
      usage: { totalTokens: 0, totalTime: 0 },
    };

    const optimisticBotMessage: ChatHistoryItem = {
      role: 1,
      content: "_Typing..._", // pode ser uma animação também
      sendDate: now,
      usage: { totalTokens: 0, totalTime: 0 },
    };

    setPendingMessages((prev) => [
      ...prev,
      optimisticUserMessage,
      optimisticBotMessage,
    ]);

    await sendMutation(content);
  }

  return {
    sessions,
    messages,
    pendingMessages,
    sessionId,
    selectSession,
    startNewSession,
    send,
    loading,
  };
}
