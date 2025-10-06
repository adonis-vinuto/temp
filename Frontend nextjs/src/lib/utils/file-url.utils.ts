/**
 * Gera uma URL do PRÓPRIO FRONT para abrir/baixar o arquivo,
 * encapsulando a URL externa (blob/presigned) atrás do proxy /api/file/proxy
 *
 * @param urlFile URL externa (blob/presigned) recebida no FileResponse
 * @param fileName nome do arquivo para Content-Disposition
 * @param opts { download?: boolean } -> attachment quando true; inline quando false
 */
export function buildFrontFileUrl(
  urlFile: string,
  fileName: string,
  opts?: { download?: boolean }
): string {
  const params = new URLSearchParams({
    url: urlFile,
    filename: fileName || "arquivo",
    disposition: opts?.download ? "attachment" : "inline",
  });
  return `/api/file/proxy?${params.toString()}`;
}
