// src/app/(app)/files/_components/error-state.tsx
"use client";

import { parseErrorMessage } from "@/lib/utils/error-parser.utils";

interface IErrorStateProps {
  error: unknown;
}

export function ErrorState({ error }: IErrorStateProps) {
  return (
    <div className="rounded-md bg-red-50 dark:bg-red-900/20 p-4">
      <p className="text-sm text-red-800 dark:text-red-200">
        {parseErrorMessage(error)}
      </p>
    </div>
  );
}
