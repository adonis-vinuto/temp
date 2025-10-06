// src/app/(app)/_components/app-sidebar.tsx
"use client";

import {
  Sidebar,
  SidebarContent,
  SidebarFooter,
  SidebarGroup,
  SidebarGroupContent,
  SidebarHeader,
  useSidebar,
} from "@/components/ui/sidebar";
import { TooltipProvider } from "@/components/ui/tooltip";
import {
  Box,
  UsersRound,
  FileText,
  BookOpen,
  Cable,
} from "lucide-react";
import { SidebarToggle } from "@/components/sidebar-toggle";
import { SidebarLogo } from "./sidebar/sidebar-logo";
import { ModuleSelector } from "./sidebar/module-selector";
import { SidebarNav } from "./sidebar/sidebar-nav";
import { UserMenu } from "./sidebar/user-menu";

const menuItems = [
  { title: "Dashboard", url: "/dashboard", icon: Box },
  { title: "Agentes", url: "/agents", icon: UsersRound },
  { title: "Arquivos", url: "/files", icon: FileText },
  { title: "Conhecimento", url: "/knowledge", icon: BookOpen },
  { title: "Integrações", url: "/integrations", icon: Cable },
];

export function AppSidebar() {
  const { open } = useSidebar();

  return (
    <TooltipProvider delayDuration={0}>
      <Sidebar collapsible="icon" className="relative">
        <SidebarToggle />
        <div className="pointer-events-none absolute right-0 top-0 h-full w-px bg-gradient-to-b from-transparent via-sidebar-border/60 to-transparent" />
        <SidebarHeader className="border-b border-white/40">
          <SidebarLogo isOpen={open} />
          <ModuleSelector isOpen={open} />
        </SidebarHeader>
        <SidebarContent>
          <SidebarGroup>
            <SidebarGroupContent>
              <SidebarNav items={menuItems} isOpen={open} />
            </SidebarGroupContent>
          </SidebarGroup>
        </SidebarContent>
        <SidebarFooter className="border-t border-white/40">
          <UserMenu isOpen={open} />
        </SidebarFooter>
      </Sidebar>
    </TooltipProvider>
  );
}
