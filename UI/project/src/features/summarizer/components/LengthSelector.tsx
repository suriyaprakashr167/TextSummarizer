import { SummaryLength } from '../types/summarizerTypes';

interface LengthSelectorProps {
  length: SummaryLength;
  maxLength: number | null;
  onLengthChange: (length: SummaryLength) => void;
  onMaxLengthChange: (maxLength: number | null) => void;
}

export const LengthSelector = ({
  length,
  maxLength,
  onLengthChange,
  onMaxLengthChange,
}: LengthSelectorProps) => {
  return (
    <div className="space-y-3">
      <label className="block text-sm font-medium text-gray-700">
        Summary Length
      </label>
      <select
        value={length}
        onChange={(e) => {
          const newLength = e.target.value as SummaryLength;
          onLengthChange(newLength);
          if (newLength !== 'custom') {
            onMaxLengthChange(null);
          }
        }}
        className="w-full px-4 py-2.5 text-gray-900 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
      >
        <option value="short">Short</option>
        <option value="medium">Medium</option>
        <option value="long">Long</option>
        <option value="custom">Custom</option>
      </select>

      {length === 'custom' && (
        <div className="animate-fadeIn">
          <label className="block text-sm font-medium text-gray-700 mb-2">
            Maximum Length (words)
          </label>
          <input
            type="number"
            min="1"
            value={maxLength || ''}
            onChange={(e) =>
              onMaxLengthChange(
                e.target.value ? parseInt(e.target.value, 10) : null
              )
            }
            placeholder="Enter max words..."
            className="w-full px-4 py-2.5 text-gray-900 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
          />
        </div>
      )}
    </div>
  );
};
