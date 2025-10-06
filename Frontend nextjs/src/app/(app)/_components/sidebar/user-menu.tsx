// src/app/(app)/_components/sidebar/user-menu.tsx
"use client";

import Link from "next/link";
import { useSession } from "next-auth/react";
import { ChevronsUpDown, User, Settings, LogOut } from "lucide-react";
import { cn } from "@/lib/utils";
import { UserAvatar } from "@/components/user-avatar";
import logout from "@/lib/utils/logout.utils";
import {
  DropdownMenu,
  DropdownMenuTrigger,
  DropdownMenuContent,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuItem,
} from "@/components/ui/dropdown-menu";
import { IisSidebarOpen } from "@/types/interfaces/sidebar.interface";

export function UserMenu({ isOpen }: IisSidebarOpen) {
  const { data: session, status } = useSession();
  const handleLogout = async () => {
    await logout();
    window.location.href = "/";
  };

  if (!isOpen) {
    return (
      <div className="p-2">
        <div className="flex justify-center">
          <UserAvatar
            name={session?.user?.name || "Usuário"}
            image={null}
            size={36}
          />
        </div>
      </div>
    );
  }

  return (
    <div className="p-4">
      <DropdownMenu>
        <DropdownMenuTrigger asChild>
          <button
            className={cn(
              "sidebar-outline-button relative flex w-full items-center gap-2 rounded-md px-2 py-2 text-left transition-colors",
              "hover:bg-sidebar-accent/25"
            )}
          >
            <UserAvatar
              name={session?.user?.name || "Usuário"}
              image={null}
              size={36}
            />
            <div className="flex flex-col overflow-hidden">
              <span className="text-sm font-medium truncate">
                {status === "loading"
                  ? "Carregando..."
                  : session?.user?.name || "Usuário"}
              </span>
              <span className="text-xs text-sidebar-foreground/75 truncate">
                {status === "loading"
                  ? "..."
                  : session?.user?.email || "email@exemplo.com"}
              </span>
            </div>
            <ChevronsUpDown className="ml-auto h-4 w-4 text-sidebar-foreground/60" />
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
        </DropdownMenuTrigger>
        <DropdownMenuContent
          side="right"
          align="start"
          className="w-56 ml-1 bg-background text-card-foreground border border-sidebar-border"
        >
          <DropdownMenuLabel className="flex flex-col">
            <span className="font-bold">
              {session?.user?.name || "Usuário"}
            </span>
            <span className="text-xs text-card-foreground/70">
              {session?.user?.email || "email@exemplo.com"}
            </span>
          </DropdownMenuLabel>
          <DropdownMenuSeparator />
          <DropdownMenuItem asChild>
            <a
              href="/api/keycloak/profile"
              target="_blank"
              rel="noopener noreferrer"
              className="flex items-center gap-2 rounded-sm px-2 py-1.5 transition-colors data-[highlighted]:bg-white data-[highlighted]:text-card-foreground focus:bg-white focus:text-card-foreground"
            >
              <User className="h-4 w-4 text-card-foreground" />
              Perfil
            </a>
          </DropdownMenuItem>
          <DropdownMenuItem asChild>
            <Link
              href="/settings"
              className="flex items-center gap-2 rounded-sm px-2 py-1.5 transition-colors data-[highlighted]:bg-white data-[highlighted]:text-card-foreground focus:bg-white focus:text-card-foreground"
            >
              <Settings className="h-4 w-4 text-card-foreground" />
              Configurações
            </Link>
          </DropdownMenuItem>
          <DropdownMenuItem
            className="flex items-center gap-2 rounded-sm px-2 py-1.5 transition-colors text-destructive data-[highlighted]:bg-white focus:bg-white focus:text-destructive cursor-pointer text-red-600"
            onClick={handleLogout}
          >
            <LogOut className="h-4 w-4 text-red-600" />
            Sair
          </DropdownMenuItem>
        </DropdownMenuContent>
      </DropdownMenu>
    </div>
  );
}
