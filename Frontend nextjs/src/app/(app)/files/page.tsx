"use client";

import { useState } from "react";
import { PageHeader } from "@/components/page-header";
import { useFilesPage } from "./_hooks/use-files-page.hook";
import { FileList } from "./_components/file-list";
import { UploadForm } from "./_components/upload-form";
import { LoadingState } from "./_components/loading-state";
import { ErrorState } from "./_components/error-state";
import { FileDetails } from "./_components/file-details";

export default function FilesPage() {
  const {
    files,
    isLoading,
    isError,
    error,
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
  } = useFilesPage();

  const [selectedFileId, setSelectedFileId] = useState<string | null>(null);

  return (
    <div className="p-6 space-y-6">
      <PageHeader
        title="Arquivos"
        description="Gerencie e visualize seus arquivos"
      />

      <div className="bg-white rounded-xl p-6 grid grid-cols-2 gap-6 border border-blue-600/20 text-card-foreground shadow-sm">
        {/* Coluna da lista */}
        <div className="space-y-6 border-r pr-6 border-foreground/30">
          {isLoading && <LoadingState />}
          {isError && <ErrorState error={error} />}
          {files && (
            <FileList
              files={files}
              onNew={() => {
                setIsCreating(true);
                setSelectedFileId(null);
              }}
              onSelectFile={(id) => {
                setSelectedFileId(id);
                setIsCreating(false);
              }}
              onPageChange={setPagina}
            />
          )}
          {!isLoading && !isError && !files && (
            <p className="text-foreground/80">Nenhum arquivo encontrado.</p>
          )}
        </div>

        <div className="pl-6">
          {isCreating ? (
            <div className="space-y-6">
              <div>
                <h3 className="text-xl font-semibold text-foreground mb-2">
                  Enviar Novo Arquivo
                </h3>
                <p className="text-sm text-foreground/60">
                  Arraste e solte seu arquivo ou clique para selecionar
                </p>
              </div>
              <UploadForm
                uploading={uploading}
                selectedFileName={selectedFileName}
                fileInputRef={fileInputRef}
                isDragging={isDragging}
                handleUpload={handleUpload}
                handleFileSelect={handleFileSelect}
                handleDragEnter={handleDragEnter}
                handleDragOver={handleDragOver}
                handleDragLeave={handleDragLeave}
                handleDrop={handleDrop}
                onCancel={() => setIsCreating(false)}
              />
            </div>
          ) : selectedFileId ? (
            <FileDetails
              fileId={selectedFileId}
              onBack={() => setSelectedFileId(null)}
            />
          ) : (
            <div>
              <h3 className="text-xl font-semibold text-foreground mb-4">
                Detalhes
              </h3>
              <p className="text-foreground/70">
                Clique em <strong>Novo</strong> para adicionar um arquivo, ou
                selecione um existente na lista para ver os detalhes.
              </p>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
