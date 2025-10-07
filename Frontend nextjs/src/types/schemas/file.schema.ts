// src\types\schemas\file.schema.ts
import { z } from "zod";

export const FileSchema = z.object({
  id: z.string().uuid(),
  fileName: z.string().min(1, "Nome do arquivo é obrigatório"),
  generatedName: z.string().min(1).nullable().optional(),
  urlFile: z.string().nullable(),
  fileSize: z.number().optional(),
  mimeType: z.string().optional(),
  uploadedAt: z.string().datetime().optional(),
  idAgent: z.string().uuid().optional(),
  resume: z.string().min(1).nullable().optional(),
});

export const UploadResultSchema = z.object({
  uri: z.string().url(),
  name: z.string(),
  contentType: z.string(),
});

export const AgentSchema = z.object({
  id: z.string().uuid(),
  organization: z.string(),
  module: z.number(),
  name: z.string(),
  description: z.string().nullable().optional(),
  typeAgent: z.number(),
});

export const FileWithRelationsSchema = FileSchema.extend({
  pages: z.array(z.any()),
  agents: z.array(AgentSchema),
});

export type FileResponse = z.infer<typeof FileSchema>;
export type UploadResult = z.infer<typeof UploadResultSchema>;
export type FileWithRelations = z.infer<typeof FileWithRelationsSchema>;
