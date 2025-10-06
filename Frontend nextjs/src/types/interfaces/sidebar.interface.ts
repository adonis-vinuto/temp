// src\types\interfaces\sidebar.intf.ts
import { LucideIcon } from "lucide-react";

export interface IisSidebarOpen {
  isOpen: boolean;
}

export interface INavItem {
  title: string;
  url: string;
  icon: LucideIcon;
}

export interface ISidebarNavProps {
  items: INavItem[];
  isOpen: boolean;
}

export interface ILogoutButtonProps {
  showText?: boolean;
  variant?: "ghost" | "outline" | "default" | "destructive" | "secondary" | "link";
  size?: "default" | "sm" | "lg" | "icon";
  className?: string;
}
