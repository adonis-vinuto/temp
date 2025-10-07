import { z } from "zod";
import { PaginatedSchema } from "./pagination.schema";
import { FileSchema } from "./file.schema";

export const AgentSchema = z.object({
  id: z.string().uuid(),
  organization: z.string(),
  module: z.number(),
  name: z.string(),
  description: z.string().nullable().optional(),
  typeAgent: z.number(),
});

export const PageSchema = z.object({
  id: z.string().uuid(),
  pageNumber: z.number(),
  title: z.string(),
  content: z.string(),
  resumePage: z.string(),
});

export const FileWithPagesSchema = FileSchema.extend({
  pages: z
    .array(PageSchema)
    .nullable()
    .transform((pages) => pages ?? []),
});

export const ChatSessionSchema = z.object({
  messageResponse: z.string(),
});

export const ChatHistorySchema = z.object({
  role: z.number(),
  content: z.string(),
  sendDate: z.string(),
  usage: z.object({
    totalTokens: z.number(),
    totalTime: z.number(),
  }),
});

export const AgentDetailSchema = AgentSchema.extend({
  files: z.array(FileWithPagesSchema),
  chatSessions: z.array(ChatSessionSchema),
  chatHistory: z.array(ChatHistorySchema),
  twilioConfig: z.any().nullable(),      // pode vir null
  seniorHcmConfig: z.array(z.any()),     // sempre array
  knowledge: z.array(z.any()),           // sempre array
  seniorErpConfig: z.array(z.any()),     // sempre array
});

export const PaginatedAgentsSchema = PaginatedSchema(AgentSchema);

export type Agent = z.infer<typeof AgentSchema>;
export type AgentDetail = z.infer<typeof AgentDetailSchema>;
export type PaginatedAgents = z.infer<typeof PaginatedAgentsSchema>;
