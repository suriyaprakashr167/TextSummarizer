import axiosClient from '../../../utils/axiosClient';
import {
  SummarizeRequest,
  SummarizeResponse,
} from '../types/summarizerTypes';

export const summarizeText = async (
  request: SummarizeRequest
): Promise<SummarizeResponse> => {
  const response = await axiosClient.post<SummarizeResponse>(
    '/api/Summarization',
    request
  );
  return response.data;
};
