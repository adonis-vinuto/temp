// src/app/(app)/_components/sidebar/module-selector.tsx
"use client";

import { Component } from "lucide-react";
import { cn } from "@/lib/utils";
import {
  DropdownMenu,
  DropdownMenuTrigger,
  DropdownMenuContent,
  DropdownMenuItem,
} from "@/components/ui/dropdown-menu";
import { useModuleStore } from "@/lib/context/use-module-store.context";
import { Module, ModuleLabels } from "@/types/enums/module.enum";
import { IisSidebarOpen } from "@/types/interfaces/sidebar.interface";
import { useRouter } from "next/navigation";

export function ModuleSelector({ isOpen }: IisSidebarOpen) {
  const { module, setModule } = useModuleStore();
  const router = useRouter();

  if (!isOpen) return null;

  return (
    <div className="px-3 pb-3">
      <button
        className={cn(
          "sidebar-outline-button relative flex w-full items-center gap-2 rounded-md px-2 py-2 text-left transition-colors",
          "hover:bg-sidebar-accent/25"
        )}
        onClick={() => router.push('/modules')}
      >
        <span>{ModuleLabels[module]}</span>
        <svg
          className="mo__svg"
          viewBox="0 0 100 100"
          preserveAspectRatio="none"
          aria-hidden="true"
        >
          <rect
            className="mo__rect"
            x="1.5"
            y="1.5"
            width="97"
            height="97"
            rx="4"
            ry="4"
          />
        </svg>
      </button>
    </div>
  );
}
