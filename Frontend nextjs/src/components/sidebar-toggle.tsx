// src\components\sidebar-toggle.tsx
"use client";

import { useEffect } from "react";
import { ChevronLeft } from "lucide-react";
import { Button } from "@/components/ui/button";
import { useSidebar } from "@/components/ui/sidebar";
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from "@/components/ui/tooltip";
import { cn } from "@/lib/utils";

export function SidebarToggle() {
  const { open, toggleSidebar } = useSidebar();
  
  useEffect(() => {
    const onKey = (e: KeyboardEvent) => {
      const isCmd = e.metaKey && !e.ctrlKey;
      const isCtrl = e.ctrlKey && !e.metaKey;
      if ((isCtrl || isCmd) && e.key.toLowerCase() === "b") {
        e.preventDefault();
        toggleSidebar();
      }
    };
    window.addEventListener("keydown", onKey);
    return () => window.removeEventListener("keydown", onKey);
  }, [toggleSidebar]);

  return (
    <TooltipProvider delayDuration={0}>
      <Tooltip>
        <TooltipTrigger asChild>
          <Button
            type="button"
            variant="ghost"
            size="icon"
            aria-label={open ? "Recolher menu" : "Expandir menu"}
            aria-expanded={open}
            onClick={toggleSidebar}
            className={cn(
              "absolute -right-3 top-1/2 -translate-y-1/2 z-40",
              "h-9 w-9 rounded-full text-card-foreground  bg-background shadow-sm",
              "transition-colors"
            )}
          >
            <ChevronLeft
              className={cn(
                "h-12 w-12 text-card-foreground transition-transform duration-200",
                !open && "rotate-180"
              )}
            />
          </Button>
        </TooltipTrigger>
        <TooltipContent side="right" className="text-xs ">
          {open ? "Recolher (Ctrl/Cmd + B)" : "Expandir (Ctrl/Cmd + B)"}
        </TooltipContent>
      </Tooltip>
    </TooltipProvider>
  );
}
