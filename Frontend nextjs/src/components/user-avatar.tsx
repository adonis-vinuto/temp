// src\components\user-avatar.tsx
"use client";

import * as React from "react";
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import { cn } from "@/lib/utils";

interface IUserAvatarProps {
  name?: string | null;
  image?: string | null;
  size?: number;
  className?: string;
  bgClassName?: string;
  textClassName?: string;
  ring?: boolean;
};

function getInitials(name?: string | null) {
  return (
    (name ?? "")
      .trim()
      .split(/\s+/)
      .slice(0, 2)
      .map((n) => n[0]?.toUpperCase() ?? "")
      .join("") || "U"
  );
}

export function UserAvatar({
  name,
  image,
  size = 36,
  className,
  bgClassName = "bg-white",
  textClassName = "text-[#007AA5]",
  ring = true,
}: IUserAvatarProps) {
  const style = { width: size, height: size } as React.CSSProperties;

  return (
    <Avatar
      className={cn(ring && "ring-1 ring-white/25", className)}
      style={style}
    >
      {image ? <AvatarImage src={image} alt={name ?? "Usuário"} /> : null}
      <AvatarFallback
        className={cn(
          "flex items-center justify-center font-semibold",
          bgClassName,
          textClassName
        )}
      >
        {getInitials(name)}
      </AvatarFallback>
    </Avatar>
  );
}
