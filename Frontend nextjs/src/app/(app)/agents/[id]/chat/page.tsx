"use client";

import { useState } from "react";
import { useParams } from "next/navigation";
import { useSession } from "next-auth/react";
import { ChatSessionList } from "./_components/chat-session-list";
import { ChatMessageList } from "./_components/chat-message-list";
import { ChatInput } from "./_components/chat-input";
import { useAgentChat } from "./_hooks/use-agent-chat.hook";
import { useAgentDetails } from "../../_hooks/use-agent-details.hook";
import { FileList } from "./_components/file-list";
import { Button } from "@/components/ui/button";
import { ChevronsLeft, ChevronsRight, File, MessageCircle } from "lucide-react";

export default function AgentChatPage() {
  const params = useParams<{ id: string }>();
  const agentId = params?.id;
  const { data: session } = useSession();
  const userId = session?.user?.id;

  const {
    sessions,
    messages,
    pendingMessages,
    sessionId,
    selectSession,
    startNewSession,
    send,
    loading,
  } = useAgentChat(agentId, userId);

  const { data: agentData, refetch } = useAgentDetails(agentId);
  const [value, setValue] = useState("");
  const [showFilesAside, setShowFilesAside] = useState(true);
  const [showSessionsAside, setShowSessionsAside] = useState(true);

  function handleSend() {
    send(value);
    setValue("");
  }

  return (
    <div className="flex h-full relative">
      {/* Aside de sessões */}
      <aside
        id="sessions-aside"
        role="complementary"
        className={`h-full border-r flex flex-col relative transition-all duration-200 ease-in-out ${showSessionsAside ? "w-64" : "w-[50px]"}`}
      >
        {/* Cabeçalho com título/botão */}
        <div className="p-2 flex items-center justify-between min-h-[48px]">
          {showSessionsAside ? (
            <>
              <h2 className="font-semibold">Sessões</h2>
              <Button
                variant="ghost"
                size="icon"
                aria-label="Fechar painel de sessões"
                aria-controls="sessions-aside"
                aria-expanded="true"
                onClick={() => setShowSessionsAside(false)}
                className="h-8 w-8"
              >
                <ChevronsLeft className="h-6 w-6" />
              </Button>
            </>
          ) : (
            <Button
              variant="ghost"
              size="icon"
              aria-label="Abrir painel de sessões"
              aria-controls="sessions-aside"
              aria-expanded="false"
              onClick={() => setShowSessionsAside(true)}
              className="h-8 w-8 mx-auto"
            >
              <ChevronsRight className="h-6 w-6" />
            </Button>
          )}
        </div>

        {/* Conteúdo (lista) — escondido quando fechado */}
        <div
          id="sessions-panel"
          className={showSessionsAside ? "flex-1 h-full overflow-y-auto" : "hidden"}
          aria-hidden={!showSessionsAside}
        >
          <ChatSessionList
            sessions={sessions}
            currentId={sessionId}
            onSelect={selectSession}
            onNew={startNewSession}
          />
        </div>

        {/* Ícone de mensagens na parte inferior - só aparece quando fechado */}
        {!showSessionsAside && (
          <div className="mt-auto p-2 flex items-center justify-center">
            <MessageCircle className="h-6 w-6 mb-4" aria-hidden="true" />
          </div>
        )}
      </aside>

      {/* Conteúdo principal */}
      <div className="flex flex-1 flex-col">
        <ChatMessageList
          messages={messages}
          pendingMessages={pendingMessages}
          loading={loading}
        />
        <ChatInput
          value={value}
          onChange={setValue}
          onSend={handleSend}
          loading={loading}
        />
      </div>

      {/* Aside de arquivos */}
      <aside
        id="files-aside"
        role="complementary"
        className={`h-full border-l flex flex-col relative transition-all duration-200 ease-in-out ${showFilesAside ? "w-64" : "w-[50px]"}`}
      >
        {/* Cabeçalho com título/botão */}
        <div className="p-2 flex items-center justify-between min-h-[48px]">
          {showFilesAside ? (
            <>
              <h2 className="font-semibold">Arquivos</h2>
              <Button
                variant="ghost"
                size="icon"
                aria-label="Fechar painel de arquivos"
                aria-controls="files-aside"
                aria-expanded="true"
                onClick={() => setShowFilesAside(false)}
                className="h-8 w-8"
              >
                <ChevronsRight className="h-6 w-6" />
              </Button>
            </>
          ) : (
            <Button
              variant="ghost"
              size="icon"
              aria-label="Abrir painel de arquivos"
              aria-controls="files-aside"
              aria-expanded="false"
              onClick={() => setShowFilesAside(true)}
              className="h-8 w-8 mx-auto"
            >
              <ChevronsLeft className="h-6 w-6" />
            </Button>
          )}
        </div>

        {/* Conteúdo (lista) — escondido quando fechado */}
        <div
          id="files-panel"
          className={showFilesAside ? "flex-1 h-full overflow-y-auto" : "hidden"}
          aria-hidden={!showFilesAside}
        >
          <FileList
            files={agentData?.files}
            agentId={agentId}
            onFilesChanged={refetch}
          />
        </div>

        {/* Ícone de arquivo na parte inferior - só aparece quando fechado */}
        {!showFilesAside && (
          <div className="mt-auto p-2 flex items-center justify-center">
            <File  className="h-6 w-6 mb-4" aria-hidden="true" />
          </div>
        )}
      </aside>
    </div>
  );
}