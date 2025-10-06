"use client";

import { useState } from "react";
import { Button } from "@/components/ui/button";
import { Dialog, DialogContent, DialogTitle } from "@/components/ui/dialog";
import { FileResponse } from "../../../../../../types/schemas/file.schema";
import Image from "next/image";
import { Plus, Unplug } from "lucide-react";
import { FileDetails } from "../../../../files/_components/file-details";
import { FileList as SelectFileList } from "../../../../files/_components/file-list";
import { UploadForm } from "../../../../files/_components/upload-form";
import { useFilesPage } from "../../../../files/_hooks/use-files-page.hook";
import { attachFileToAgent } from "@/lib/api/file.client";
import { toast } from "sonner";

interface FileListProps {
  files?: FileResponse[];
  agentId: string;
  onFilesChanged?: () => void;
}

export function FileList({ files, agentId, onFilesChanged }: FileListProps) {
  const [selectedFileId, setSelectedFileId] = useState<string | null>(null);
  const [dialogOpen, setDialogOpen] = useState(false);

  const [addDialogOpen, setAddDialogOpen] = useState(false);
  const [showUploadForm, setShowUploadForm] = useState(false);

  const {
    files: availableFiles,
    isLoading,
    setPagina,
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

  const handleAttachFile = async (fileId: string, desanexar = false) => {
    try {
      await attachFileToAgent(fileId, { idAgent: agentId });
      toast.success(
        `Arquivo ${desanexar ? "desanexado do" : "anexado ao"} agente!`
      );
      onFilesChanged?.();
      setAddDialogOpen(false);
    } catch (err) {
      toast.error("Erro ao anexar arquivo: " + err);
    }
  };

  const handleUploadSuccess = async () => {
    setShowUploadForm(false);
  };

  const filteredItens = availableFiles?.itens.filter(
    (file) => !files?.some((attached) => attached.id === file.id)
  );

  const filteredFiles = {
    ...availableFiles,
    itens: filteredItens ?? [],
    totalItens: filteredItens?.length ?? 0,
    totalPaginas: Math.max(
      1,
      Math.ceil(
        (filteredItens?.length ?? 1) / (availableFiles?.tamanhoPagina ?? 1)
      )
    ),
    tamanhoPagina: availableFiles?.tamanhoPagina ?? 10,
    indice: Math.min(
      availableFiles?.indice ?? 0,
      Math.max(
        1,
        Math.ceil(
          (filteredItens?.length ?? 1) / (availableFiles?.tamanhoPagina ?? 1)
        )
      )
    ),
  };

  return (
    <div className="flex h-full flex-col">
      <div className="p-2">
        <Button className="w-full" onClick={() => setAddDialogOpen(true)}>
          <Plus /> Adicionar arquivo
        </Button>
      </div>
      <div className="flex-1 overflow-y-auto space-y-1 p-2">
        {files?.map((file) => {
          const ext = file.fileName.split(".").pop()?.toLowerCase() ?? "";
          const isImage = ["jpg", "jpeg", "png", "gif", "webp"].includes(ext);
          const isPdf = ext === "pdf";
          const isDocx = ext === "docx";
          const isXlsx = ["xlsx", "xls", "csv"].includes(ext);

          return (
            <div
              key={file.id}
              className="relative bg-sidebar hover:bg-sidebar/80 border border-blue-300/20 shadow-md text-white rounded-lg hover:shadow-lg transition flex flex-col items-center justify-between p-4 h-24 cursor-pointer"
              onClick={() => {
                setSelectedFileId(file.id);
                setDialogOpen(true);
              }}
            >
              <button
                type="button"
                className="absolute top-2 right-2 text-red-400 hover:text-red-600"
                onClick={(e) => {
                  e.stopPropagation();
                  handleAttachFile(file.id, true);
                }}
                title="Desanexar"
              >
                <Unplug size={18} />
              </button>
              <Image
                src={
                  isImage
                    ? file.urlFile ?? "/images/file.png"
                    : isPdf
                    ? "/images/pdf.png"
                    : isDocx
                    ? "/images/docx.png"
                    : isXlsx
                    ? "/images/xlsx.png"
                    : "/images/file.png"
                }
                alt={file.fileName}
                width={32}
                height={32}
                className="object-contain"
              />
              <p
                className="text-[12px] font-medium text-center truncate w-full"
                title={file.fileName}
              >
                {file.fileName}
              </p>
            </div>
          );
        })}
        {files?.length === 0 && (
          <p className="p-2 text-sm text-foreground/70">
            Nenhum arquivo atrelado a este agente.
          </p>
        )}
      </div>

      <Dialog open={dialogOpen} onOpenChange={setDialogOpen}>
        <DialogContent>
          <DialogTitle></DialogTitle>
          <FileDetails
            fileId={selectedFileId}
            onBack={() => setDialogOpen(false)}
            onDeleted={() => setDialogOpen(false)}
          />
        </DialogContent>
      </Dialog>

      <Dialog open={addDialogOpen} onOpenChange={setAddDialogOpen}>
        <DialogContent>
          <DialogTitle>Selecionar arquivo para anexar</DialogTitle>
          {showUploadForm ? (
            <UploadForm
              uploading={uploading}
              selectedFileName={selectedFileName}
              fileInputRef={fileInputRef}
              isDragging={isDragging}
              handleUpload={async (e) => {
                await handleUpload(e);
                handleUploadSuccess();
              }}
              handleFileSelect={handleFileSelect}
              handleDragEnter={handleDragEnter}
              handleDragOver={handleDragOver}
              handleDragLeave={handleDragLeave}
              handleDrop={handleDrop}
              onCancel={() => setShowUploadForm(false)}
            />
          ) : isLoading ? (
            <div>Carregando arquivos...</div>
          ) : (
            <SelectFileList
              files={filteredFiles}
              onNew={() => setShowUploadForm(true)}
              onPageChange={setPagina}
              onSelectFile={(id) => handleAttachFile(id)}
            />
          )}
        </DialogContent>
      </Dialog>
    </div>
  );
}
