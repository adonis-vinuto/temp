// src/app/(app)/files/_hooks/use-file-details.hook.ts
"use client";

import { useQuery } from "@tanstack/react-query";
import { getFile } from "@/lib/api/file.client";
import type { FileResponse } from "@/types/schemas/file.schema";

export function useFileDetails(id: string | null) {
  return useQuery<FileResponse, Error>({
    queryKey: ["file", id],
    queryFn: () => {
      if (!id) throw new Error("ID inválido");
      return getFile(id);
    },
    enabled: !!id,
  });
}
