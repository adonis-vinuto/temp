"use client";

import Image from "next/image";
import type { FileResponse } from "@/types/schemas/file.schema";
import type { TPaginated } from "@/types/schemas/pagination.schema";

interface FileListProps {
  files: TPaginated<FileResponse>;
  onNew: () => void;
  onPageChange: (page: number) => void;
  onSelectFile?: (id: string) => void;
}

export function FileList({
  files,
  onNew,
  onPageChange,
  onSelectFile,
}: FileListProps) {
  return (
    <div className="space-y-6">
      <div className="grid grid-cols-2 md:grid-cols-2 lg:grid-cols-3 gap-4">
        <div
          className="bg-sidebar hover:bg-sidebar/80 text-white rounded-lg shadow hover:shadow-lg transition flex flex-col items-center justify-center p-4 h-32 cursor-pointer"
          onClick={onNew}
        >
          <span className="text-2xl font-bold">+</span>
          <p className="mt-2 text-sm font-semibold">Novo</p>
        </div>

        {files.itens.map((file) => {
          const ext = file.fileName.split(".").pop()?.toLowerCase() ?? "";
          const isImage = ["jpg", "jpeg", "png", "gif", "webp"].includes(ext);
          const isPdf = ext === "pdf";
          const isDocx = ext === "docx";
          const isXlsx = ["xlsx", "xls", "csv"].includes(ext);

          return (
            <div
              key={file.id}
              className="bg-sidebar hover:bg-sidebar/80 border border-blue-300/20 shadow-md text-white rounded-lg hover:shadow-lg transition flex flex-col items-center justify-between p-4 h-32 cursor-pointer relative"
              onClick={() => onSelectFile?.(file.id)}
            >
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
      </div>

      <div className="flex justify-between items-center pt-4">
        <button
          className="px-3 py-1 bg-foreground hover:bg-foreground/80 text-white rounded disabled:opacity-50"
          disabled={files.indice <= 1}
          onClick={() => onPageChange(Math.max(1, files.indice - 1))}
        >
          Anterior
        </button>
        <span className="text-sm text-foreground/80">
          Página {files.indice} de {files.totalPaginas}
        </span>
        <button
          className="px-3 py-1 bg-foreground hover:bg-foreground/80 text-white rounded disabled:opacity-50"
          disabled={files.indice >= files.totalPaginas}
          onClick={() => onPageChange(files.indice + 1)}
        >
          Próxima
        </button>
      </div>
    </div>
  );
}
