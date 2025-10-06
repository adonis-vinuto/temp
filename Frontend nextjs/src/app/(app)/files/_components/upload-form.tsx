// src/app/(app)/files/_components/upload-form.tsx
"use client";

interface UploadFormProps {
  uploading: boolean;
  selectedFileName: string;
  fileInputRef: React.RefObject<HTMLInputElement | null>;
  isDragging: boolean;
  handleUpload: (e: React.FormEvent<HTMLFormElement>) => void;
  handleFileSelect: (e: React.ChangeEvent<HTMLInputElement>) => void;
  handleDragEnter: (e: React.DragEvent) => void;
  handleDragOver: (e: React.DragEvent) => void;
  handleDragLeave: (e: React.DragEvent) => void;
  handleDrop: (e: React.DragEvent) => void;
  onCancel: () => void;
}

export function UploadForm({
  uploading,
  selectedFileName,
  fileInputRef,
  isDragging,
  handleUpload,
  handleFileSelect,
  handleDragEnter,
  handleDragOver,
  handleDragLeave,
  handleDrop,
  onCancel,
}: UploadFormProps) {
  return (
    <form onSubmit={handleUpload} className="space-y-6">
      <div
        className={`
          relative border-2 border-dashed rounded-xl p-8
          transition-all duration-200 cursor-pointer
          ${isDragging 
            ? "border-foreground bg-blue-50" 
            : "border-gray-300 hover:border-gray-400 bg-gray-50"}
        `}
        onDragEnter={handleDragEnter}
        onDragOver={handleDragOver}
        onDragLeave={handleDragLeave}
        onDrop={handleDrop}
        onClick={() => fileInputRef.current?.click()}
      >
        <input
          ref={fileInputRef}
          type="file"
          name="arquivo"
          accept=".pdf,.doc,.docx,.xls,.xlsx,.csv,.txt"
          onChange={handleFileSelect}
          className="hidden"
        />

        <div className="flex flex-col items-center justify-center space-y-3">
          <svg
            className={`w-12 h-12 ${isDragging ? "text-foreground" : "text-foreground/70"}`}
            fill="none"
            stroke="currentColor"
            viewBox="0 0 24 24"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth={2}
              d="M7 16a4 4 0 01-.88-7.903A5 5 0 1115.9 6L16 6a5 5 0 011 9.9M15 13l-3-3m0 0l-3 3m3-3v12"
            />
          </svg>

          <div className="text-center">
            <p className="font-medium text-foreground/70 text-xs">
              {isDragging ? (
                "Solte o arquivo aqui"
              ) : (
                <>
                  <span className="text-foreground/80 hover:text-foreground text-sm">
                    Clique para escolher
                  </span>{" "}
                  ou arraste um arquivo
                </>
              )}
            </p>
          </div>

          {selectedFileName && !uploading && (
            <div className="mt-2 px-3 py-1 bg-white rounded-lg border border-gray-200">
              <p className="text-sm text-foreground font-medium">
                📄 {selectedFileName}
              </p>
            </div>
          )}

          {uploading && (
            <div className="mt-2 text-center">
              <div className="inline-flex items-center space-x-2">
                <svg
                  className="animate-spin h-5 w-5 text-foreground"
                  fill="none"
                  viewBox="0 0 24 24"
                >
                  <circle
                    className="opacity-25"
                    cx="12"
                    cy="12"
                    r="10"
                    stroke="currentColor"
                    strokeWidth="4"
                  />
                  <path
                    className="opacity-75"
                    fill="currentColor"
                    d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
                  />
                </svg>
                <span className="text-sm text-foreground">
                  Enviando {selectedFileName}...
                </span>
              </div>
            </div>
          )}
        </div>
      </div>

      <div className="bg-blue-50 border border-blue-200 rounded-lg p-4">
        <div className="flex items-start space-x-2">
          <svg
            className="w-5 h-5 text-blue-600 mt-0.5"
            fill="currentColor"
            viewBox="0 0 20 20"
          >
            <path
              fillRule="evenodd"
              d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a1 1 0 000 2v3a1 1 0 001 1h1a1 1 0 100-2v-3a1 1 0 00-1-1H9z"
              clipRule="evenodd"
            />
          </svg>
          <div className="flex-1">
            <p className="text-sm text-foreground font-medium">Formatos aceitos</p>
            <p className="text-xs text-foreground/50 mt-1">
              Documentos (PDF, DOCX), Planilhas (XLSX, CSV), TXT e outros formatos
            </p>
            <p className="text-xs text-foreground/50 mt-1">
              Tamanho máximo: 10MB por arquivo
            </p>
          </div>
        </div>
      </div>

      <button
        type="submit"
        disabled={uploading || !selectedFileName}
        className="w-full px-4 py-3 bg-foreground text-white rounded-lg hover:bg-foreground/90 disabled:opacity-50 disabled:cursor-not-allowed transition-colors font-medium"
      >
        {uploading ? "Enviando arquivo..." : "Enviar arquivo"}
      </button>

      <button
        type="button"
        onClick={onCancel}
        className="w-full px-4 py-2 text-foreground/70 hover:text-foreground transition-colors text-sm"
      >
        Cancelar
      </button>
    </form>
  );
}
