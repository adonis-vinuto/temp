// src/app/(app)/_components/sidebar/sidebar-nav.tsx
"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";

import { cn } from "@/lib/utils";
import {
  SidebarMenu,
  SidebarMenuItem,
  SidebarMenuButton,
} from "@/components/ui/sidebar";
import {
  Tooltip,
  TooltipTrigger,
  TooltipContent,
} from "@/components/ui/tooltip";
import { ISidebarNavProps } from "@/types/interfaces/sidebar.interface";

export function SidebarNav({ items, isOpen }: ISidebarNavProps) {
  const pathname = usePathname();
  
  const isActive = (url: string) =>
    pathname === url || pathname.startsWith(`${url}/`);

  return (
    <SidebarMenu>
      {items.map((item) => (
        <SidebarMenuItem key={item.title}>
          {!isOpen ? (
            <Tooltip>
              <TooltipTrigger asChild>
                <SidebarMenuButton
                  asChild
                  isActive={pathname === item.url}
                  className={cn(
                    "sidebar-outline-button relative justify-center transition-colors",
                    "hover:bg-sidebar-accent/25 data-[active=true]:bg-sidebar-primary/15"
                  )}
                >
                  <Link href={item.url} className="relative flex items-center">
                    <item.icon className="h-8 w-8" />
                    <span className="sr-only">{item.title}</span>
                    <svg className="mo__svg" viewBox="0 0 100 100" preserveAspectRatio="none" aria-hidden="true">
                      <rect className="mo__rect" x="1.5" y="1.5" width="97" height="97" rx="4" ry="4" />
                    </svg>
                  </Link>
                </SidebarMenuButton>
              </TooltipTrigger>
              <TooltipContent side="right">{item.title}</TooltipContent>
            </Tooltip>
          ) : (
            <SidebarMenuButton
              asChild
              isActive={isActive(item.url)}
              className={cn(
                "sidebar-outline-button relative transition-colors",
                "hover:bg-sidebar-accent/25",
                "data-[active=true]:bg-sidebar-primary/15 data-[active=true]:text-sidebar-foreground"
              )}
            >
              <Link href={item.url} className="relative flex items-center gap-2">
                <item.icon className="h-10 w-10" />
                <span className="text-base">{item.title}</span>
                <svg className="mo__svg" viewBox="0 0 100 100" preserveAspectRatio="none" aria-hidden="true">
                  <rect className="mo__rect" x="1.5" y="1.5" width="97" height="97" rx="4" ry="4" />
                </svg>
              </Link>
            </SidebarMenuButton>
          )}
        </SidebarMenuItem>
      ))}
    </SidebarMenu>
  );
}
