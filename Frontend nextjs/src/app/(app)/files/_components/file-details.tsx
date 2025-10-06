"use client";

import { useFileDetails } from "../_hooks/use-file-details.hook";
import { LoadingState } from "./loading-state";
import { ErrorState } from "./error-state";
import { Button } from "@/components/ui/button";
import { DownloadIcon, TrashIcon, ExternalLinkIcon } from "lucide-react";
import { deleteFile } from "@/lib/api/file.client";
import { toast } from "sonner";
import { buildFrontFileUrl } from "@/lib/utils/file-url.utils";

interface FileDetailsProps {
  fileId: string | null;
  onBack?: () => void;
  onDeleted?: () => void;
}

export function FileDetails({ fileId, onBack, onDeleted }: FileDetailsProps) {
  const { data: fileData, isLoading, isError, error } = useFileDetails(fileId ?? "");

  if (!fileId) {
    return (
      <div className="text-foreground/70">
        Selecione um arquivo para ver os detalhes.
      </div>
    );
  }

  if (isLoading) return <LoadingState />;
  if (isError) return <ErrorState error={error} />;
  if (!fileData) return null;

  const openUrl = fileData.urlFile
    ? buildFrontFileUrl(fileData.urlFile, fileData.fileName, { download: false })
    : null;

  const downloadUrl = fileData.urlFile
    ? buildFrontFileUrl(fileData.urlFile, fileData.fileName, { download: true })
    : null;

  const handleDelete = async () => {
    if (!fileData?.id) return;
    try {
      await deleteFile(fileData.id);
      toast.success("Arquivo excluído com sucesso!");
      onDeleted?.();
      onBack?.();
    } catch (err) {
      console.error(err);
      toast.error("Erro ao excluir o arquivo.");
    }
  };

  return (
    <div className="space-y-4">
      <h3 className="text-xl font-semibold text-foreground">Detalhes do Arquivo</h3>

      <div className="space-y-2">
        <p>
          <span className="font-medium">Nome:</span> {fileData.fileName}
        </p>
        <p>
          <span className="font-medium">Tipo:</span> {fileData.mimeType ?? "N/A"}
        </p>
        <p>
          <span className="font-medium">Tamanho:</span>{" "}
          {fileData.fileSize ? `${(fileData.fileSize / 1024).toFixed(2)} KB` : "N/A"}
        </p>
        <p>
          <span className="font-medium">Enviado em:</span>{" "}
          {fileData.uploadedAt ? new Date(fileData.uploadedAt).toLocaleString() : "N/A"}
        </p>
      </div>

      <div className="flex gap-2">
        {downloadUrl && (
          <a href={downloadUrl} className="w-28" download>
            <Button variant="default" size="sm" className="w-full" asChild>
              <span>
                <DownloadIcon size={16} className="mr-2" />
                Baixar
              </span>
            </Button>
          </a>
        )}

        {openUrl && (
          <a href={openUrl} target="_blank" rel="noopener noreferrer" className="w-28">
            <Button variant="secondary" size="sm" className="w-full" asChild>
              <span>
                <ExternalLinkIcon size={16} className="mr-2" />
                Abrir
              </span>
            </Button>
          </a>
        )}

        <Button
          variant="destructive"
          size="sm"
          className="w-28"
          onClick={handleDelete}
        >
          <TrashIcon size={16} className="mr-2" />
          Excluir
        </Button>
      </div>

      {onBack && (
        <button
          onClick={onBack}
          className="px-4 py-2 text-sm bg-gray-100 rounded hover:bg-gray-200"
        >
          Voltar
        </button>
      )}
    </div>
  );
}
