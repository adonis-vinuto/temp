// src\types\schemas\chat.schema.ts
import { Session } from "inspector/promises";
import { string, z } from "zod";

export const ChatUsageSchema = z.object({
  totalTokens: z.number(),
  totalTime: z.number(),
});

export const ChatHistoryItemSchema = z.object({
  role: z.number(),
  content: z.string(),
  sendDate: z.string(),
  usage: ChatUsageSchema,
});

export type ChatUsage = z.infer<typeof ChatUsageSchema>;
export type ChatHistoryItem = z.infer<typeof ChatHistoryItemSchema>;

export const SessionHistorySchema = z.object({
  sessionId: z.string(),
  lastSendDate: z.string(),
  title: z.string(),
  totalInteractions: z.string().optional(),
});

export type SessionHistory = z.infer<typeof SessionHistorySchema>;

export const FirstMessageRequestSchema = z.object({
  message: z.string(),
});

export const FirstMessageResponseSchema = z.object({
  messageResponse: z.string(),
  sessionId: z.string(),
});

export type FirstMessageRequest = z.infer<typeof FirstMessageRequestSchema>;
export type FirstMessageResponse = z.infer<typeof FirstMessageResponseSchema>;

export const ContinueMessageRequestSchema = z.object({
  message: z.string(),
});

export const ContinueMessageResponseSchema = z.object({
  messageResponse: z.string(),
  sessionId: z.string(),
});

export type ContinueMessageRequest = z.infer<typeof ContinueMessageRequestSchema>;
export type ContinueMessageResponse = z.infer<typeof ContinueMessageResponseSchema>;
