// src/app/(app)/settings/_components/loading-state.tsx
import { Skeleton } from '@/components/ui/skeleton';

export function LoadingState() {
  return (
    <div className="p-6 space-y-6">
      <Skeleton className="h-8 w-48 bg-white" />
      <Skeleton className="h-4 w-96 bg-white" />
      <Skeleton className="h-px w-full opacity-40 bg-white" />
      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        <Skeleton className="h-96 bg-white" />
        <Skeleton className="h-96 bg-white" />
      </div>
    </div>
  );
}
