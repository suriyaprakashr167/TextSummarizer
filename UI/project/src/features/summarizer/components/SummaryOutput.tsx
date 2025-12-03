import { Copy, Check } from 'lucide-react';
import { useState } from 'react';

interface SummaryOutputProps {
  summary: string;
}

export const SummaryOutput = ({ summary }: SummaryOutputProps) => {
  const [copied, setCopied] = useState(false);

  const handleCopy = async () => {
    try {
      await navigator.clipboard.writeText(summary);
      setCopied(true);
      setTimeout(() => setCopied(false), 2000);
    } catch (err) {
      console.error('Failed to copy:', err);
    }
  };

  const wordCount = summary.split(/\s+/).filter(Boolean).length;

  return (
    <div className="space-y-3 animate-fadeIn">
      <div className="flex justify-between items-center">
        <label className="block text-sm font-medium text-gray-700">
          Summary
        </label>
        <button
          onClick={handleCopy}
          className="inline-flex items-center gap-2 px-3 py-1.5 text-sm font-medium text-white bg-blue-600 rounded-lg hover:bg-blue-700 transition-colors"
        >
          {copied ? (
            <>
              <Check className="w-4 h-4" />
              Copied!
            </>
          ) : (
            <>
              <Copy className="w-4 h-4" />
              Copy to Clipboard
            </>
          )}
        </button>
      </div>
      <div className="p-4 bg-gray-50 border border-gray-200 rounded-lg">
        <p className="text-gray-900 leading-relaxed whitespace-pre-wrap">
          {summary}
        </p>
      </div>
      <div className="text-sm text-gray-500">{wordCount} words</div>
    </div>
  );
};
