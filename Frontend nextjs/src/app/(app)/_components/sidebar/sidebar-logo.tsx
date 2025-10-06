// src/app/(app)/_components/sidebar/sidebar-logo.tsx
"use client";

import Image from "next/image";
import { cn } from "@/lib/utils";
import { IisSidebarOpen } from "@/types/interfaces/sidebar.interface";

export function SidebarLogo({ isOpen }: IisSidebarOpen) {
  return (
    <div
      className={cn(
        "px-3 py-4",
        isOpen ? "flex items-center gap-3" : "flex items-center justify-center"
      )}
    >
      <div className={cn("shrink-0", !isOpen && "flex justify-center")}>
        <Image
          src="/images/logo.png"
          alt="Komvos"
          width={isOpen ? 50 : 30}
          height={isOpen ? 20 : 30}
          priority
          className={cn("rounded-md", !isOpen && "rounded-lg")}
        />
      </div>

      {isOpen && (
        <div className="flex flex-col overflow-hidden">
          <span className="text-2xl font-bold text-sidebar-foreground uppercase first-letter:text-4xl">
            KOMVOS
          </span>
          <span className="text-xl text-sidebar-foreground/80 uppercase first-letter:text-2xl">
            MIND
          </span>
        </div>
      )}
    </div>
  );
}
