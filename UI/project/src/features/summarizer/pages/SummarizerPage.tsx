import { useState } from 'react';
import { Loader2, AlertCircle, FileText } from 'lucide-react';
import { TextInput } from '../components/TextInput';
import { LengthSelector } from '../components/LengthSelector';
import { SummaryOutput } from '../components/SummaryOutput';
import { summarizeText } from '../api/summarizerApi';
import { SummaryLength } from '../types/summarizerTypes';

export const SummarizerPage = () => {
  const [text, setText] = useState('');
  const [length, setLength] = useState<SummaryLength>('medium');
  const [maxLength, setMaxLength] = useState<number | null>(null);
  const [summary, setSummary] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSummarize = async () => {
    if (!text.trim()) {
      setError('Please enter some text to summarize');
      return;
    }

    if (length === 'custom' && !maxLength) {
      setError('Please enter a maximum length for custom summary');
      return;
    }

    setLoading(true);
    setError(null);
    setSummary(null);

    try {
      const response = await summarizeText({
        text,
        length,
        maxLength: length === 'custom' ? maxLength : null,
      });
      setSummary(response.summary);
    } catch (err) {
      setError(
        err instanceof Error
          ? err.message
          : 'Failed to generate summary. Please try again.'
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-50 to-gray-100">
      <div className="max-w-4xl mx-auto px-4 py-12">
        <div className="text-center mb-12">
          <div className="inline-flex items-center justify-center w-16 h-16 bg-blue-600 rounded-full mb-4">
            <FileText className="w-8 h-8 text-white" />
          </div>
          <h1 className="text-4xl font-bold text-gray-900 mb-2">
            Text Summarizer
          </h1>
          <p className="text-lg text-gray-600">
            Transform long text into concise summaries instantly
          </p>
        </div>

        <div className="bg-white rounded-xl shadow-lg p-8 space-y-6">
          <TextInput value={text} onChange={setText} />

          <LengthSelector
            length={length}
            maxLength={maxLength}
            onLengthChange={setLength}
            onMaxLengthChange={setMaxLength}
          />

          <button
            onClick={handleSummarize}
            disabled={loading || !text.trim()}
            className="w-full py-3 px-6 text-white font-medium bg-blue-600 rounded-lg hover:bg-blue-700 disabled:bg-gray-400 disabled:cursor-not-allowed transition-colors flex items-center justify-center gap-2"
          >
            {loading ? (
              <>
                <Loader2 className="w-5 h-5 animate-spin" />
                Generating Summary...
              </>
            ) : (
              'Summarize'
            )}
          </button>

          {error && (
            <div className="flex items-start gap-3 p-4 bg-red-50 border border-red-200 rounded-lg animate-fadeIn">
              <AlertCircle className="w-5 h-5 text-red-600 flex-shrink-0 mt-0.5" />
              <p className="text-red-800 text-sm">{error}</p>
            </div>
          )}

          {summary && <SummaryOutput summary={summary} />}
        </div>

        <div className="mt-8 text-center text-sm text-gray-500">
          <p>Powered by advanced AI summarization</p>
        </div>
      </div>
    </div>
  );
};
