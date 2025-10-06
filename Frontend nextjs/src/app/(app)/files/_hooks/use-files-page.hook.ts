// src\app\(app)\files\_hooks\use-files-page.hook.ts
"use client";

import { useState, useRef } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { listFiles, createFile } from "@/lib/api/file.client";
import type { FileResponse } from "@/types/schemas/file.schema";
import type { TPaginated } from "@/types/schemas/pagination.schema";
import { toast } from "sonner";

export function useFilesPage() {
  const queryClient = useQueryClient();
  const [pagina, setPagina] = useState(1);
  const [tamanhoPagina] = useState(10);
  const [isCreating, setIsCreating] = useState(false);
  const [selectedFileName, setSelectedFileName] = useState("");
  const fileInputRef = useRef<HTMLInputElement>(null);

  const [isDragging, setIsDragging] = useState(false);

  const {
    data: files,
    isLoading,
    isError,
    error,
  } = useQuery<TPaginated<FileResponse>, Error>({
    queryKey: ["files", pagina, tamanhoPagina],
    queryFn: () => listFiles(pagina, tamanhoPagina),
  });

  const { mutateAsync: uploadFile, isPending: uploading } = useMutation({
    mutationFn: async (file: File) => {
      return await createFile(file);
    },
    onSuccess: () => {
      toast.success("Arquivo enviado com sucesso!");
      queryClient.invalidateQueries({ queryKey: ["files"] });
      setIsCreating(false);
      setSelectedFileName("");
    },
    onError: (err: unknown) => {
      toast.error(
        err instanceof Error
          ? err.message
          : "Não foi possível enviar o arquivo."
      );
    },
  });

  async function handleUpload(e: React.FormEvent<HTMLFormElement>) {
    e.preventDefault();
    const file = fileInputRef.current?.files?.[0];
    if (!file) {
      toast.warning("Selecione um arquivo para enviar.");
      return;
    }
    await uploadFile(file);
  }

  function handleFileSelect(e: React.ChangeEvent<HTMLInputElement>) {
    const file = e.target.files?.[0];
    if (file) setSelectedFileName(file.name);
  }

  function handleDragEnter(e: React.DragEvent) {
    e.preventDefault();
    setIsDragging(true);
  }
  function handleDragOver(e: React.DragEvent) {
    e.preventDefault();
    setIsDragging(true);
  }
  function handleDragLeave(e: React.DragEvent) {
    e.preventDefault();
    setIsDragging(false);
  }
  function handleDrop(e: React.DragEvent) {
    e.preventDefault();
    setIsDragging(false);
    const file = e.dataTransfer.files?.[0];
    if (file) {
      setSelectedFileName(file.name);
      if (fileInputRef.current) {
        const dataTransfer = new DataTransfer();
        dataTransfer.items.add(file);
        fileInputRef.current.files = dataTransfer.files;
      }
    }
  }

  return {
    files,
    isLoading,
    isError,
    error,
    pagina,
    tamanhoPagina,
    setPagina,
    isCreating,
    setIsCreating,
    uploading,
    selectedFileName,
    fileInputRef,
    handleUpload,
    isDragging,
    handleFileSelect,
    handleDragEnter,
    handleDragOver,
    handleDragLeave,
    handleDrop,
  };
}
