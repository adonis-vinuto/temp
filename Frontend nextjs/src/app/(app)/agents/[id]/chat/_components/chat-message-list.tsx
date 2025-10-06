"use client";

import React from "react";
import ReactMarkdown, {
  type Components,
  type ExtraProps,
} from "react-markdown";
import remarkGfm from "remark-gfm";
import rehypeRaw from "rehype-raw";
import type { ChatHistoryItem } from "@/types/schemas/chat.schema";
import Image from "next/image";
import { UserAvatar } from "../../../../../../components/user-avatar";
import { useSession } from "next-auth/react";

interface ChatMessageListProps {
  messages: ChatHistoryItem[];
  pendingMessages: ChatHistoryItem[];
  loading: boolean;
}

// Tipagem explícita para o componente de código
type CodeProps = React.ComponentPropsWithoutRef<"code"> &
  ExtraProps & { inline?: boolean };

export function ChatMessageList({
  messages,
  pendingMessages,
}: ChatMessageListProps) {
  const { data: session } = useSession();
  const allMessages = [...messages, ...pendingMessages];

  const endOfMessagesRef = React.useRef<HTMLDivElement>(null);

  React.useEffect(() => {
    endOfMessagesRef.current?.scrollIntoView({ behavior: "smooth" });
  }, [messages, pendingMessages]);

  const components: Components = {
    code({ inline, className, children, ...props }: CodeProps) {
      if (inline) {
        return (
          <code className="bg-muted px-1 rounded" {...props}>
            {children}
          </code>
        );
      }
      return (
        <pre className="bg-black/80 text-white p-3 rounded-md overflow-auto">
          <code className={className} {...props}>
            {children}
          </code>
        </pre>
      );
    },
  };

  return (
    <div className="h-full p-8 flex flex-col gap-4 overflow-y-auto">
      {allMessages.length === 0 && (
        <p className="text-foreground/70">Nenhuma mensagem ainda.</p>
      )}
      {allMessages.map((msg, idx) =>
        msg.role === 1 ? (
          <div key={idx} className="flex items-start gap-2 mb-3 max-w-[80%]">
            <Image
              src="/images/logo.png"
              alt="Komvos"
              width={30}
              height={30}
              priority
              className="rounded-sm p-0.5 bg-black/70"
            />

            <div className="bg-accent/10 p-4 rounded-lg rounded-tl-none">
              {msg.content === "_Typing..._" ? (
                <span className="animate-pulse text-sm italic text-muted">
                  Digitando...
                </span>
              ) : (
                <ReactMarkdown
                  remarkPlugins={[remarkGfm]}
                  rehypePlugins={[rehypeRaw]}
                  components={components}
                >
                  {msg.content}
                </ReactMarkdown>
              )}
              <span className="text-xs opacity-60">
                {new Date(msg.sendDate).toLocaleTimeString()}
              </span>
            </div>
          </div>
        ) : (
          <div
            key={idx}
            className="flex items-start gap-2 mb-3 justify-end self-end max-w-[80%]"
          >
            <div className="bg-accent/30 p-4 rounded-lg rounded-tr-none">
              <ReactMarkdown
                remarkPlugins={[remarkGfm]}
                rehypePlugins={[rehypeRaw]}
                components={components}
              >
                {msg.content}
              </ReactMarkdown>
              <span className="text-xs opacity-60">
                {new Date(msg.sendDate).toLocaleTimeString()}
              </span>
            </div>

            <UserAvatar
              name={session?.user?.name || "Usuário"}
              image={session?.user?.image || null}
              size={36}
              bgClassName="bg-accent/30"
            />
          </div>
        )
      )}
      <div ref={endOfMessagesRef} />
    </div>
  );
}
