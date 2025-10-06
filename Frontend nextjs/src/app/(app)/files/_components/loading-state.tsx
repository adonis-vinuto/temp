// src/app/(app)/files/_components/loading-state.tsx
"use client";

import { Skeleton } from "@/components/ui/skeleton";

export function LoadingState() {
  return (
    <div className="p-6 space-y-6">
      <Skeleton className="h-8 w-48 bg-white" />
      <Skeleton className="h-4 w-96 bg-white" />
      <Skeleton className="h-px w-full opacity-40 bg-white" />

      <div className="grid grid-cols-2 md:grid-cols-2 lg:grid-cols-3 gap-4">
        {Array.from({ length: 6 }).map((_, i) => (
          <Skeleton key={i} className="h-32 rounded-lg bg-white" />
        ))}
      </div>

      <div className="flex justify-between items-center pt-4">
        <Skeleton className="h-8 w-20 bg-white" />
        <Skeleton className="h-6 w-32 bg-white" />
        <Skeleton className="h-8 w-20 bg-white" />
      </div>
    </div>
  );
}
