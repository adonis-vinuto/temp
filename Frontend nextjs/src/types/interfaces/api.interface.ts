// src\types\interfaces\api.intf.ts
export interface IApiResponse {
  [key: string]: unknown;
}

export interface IApiSuccessResponse<T = unknown> {
  success: boolean;
  data: T;
  message?: string;
}

export interface IApiErrorResponse {
  success: false;
  error: {
    code: string;
    message: string;
    details?: unknown;
  };
  statusCode?: number;
}

export interface IApiPaginatedResponse<T> {
  data: T[];
  pagination: {
    page: number;
    pageSize: number;
    total: number;
    totalPages: number;
  };
}

export interface IApiUploadResponse {
  success: boolean;
  fileUrl?: string;
  fileId?: string;
  fileName?: string;
  fileSize?: number;
  mimeType?: string;
}

export type TApiResponseType<T = unknown> = 
  | IApiSuccessResponse<T> 
  | IApiErrorResponse;

export interface IFormDataInput {
  [key: string]: string | number | boolean | File | Blob | (File | Blob)[] | null | undefined;
}
