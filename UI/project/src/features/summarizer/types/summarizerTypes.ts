export type SummaryLength = 'short' | 'medium' | 'long' | 'custom';

export interface SummarizeRequest {
  text: string;
  length?: SummaryLength;
  maxLength?: number | null;
}

export interface SummarizeResponse {
  summary: string;
}
