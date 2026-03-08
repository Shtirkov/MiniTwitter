interface DividerProps {
  label?: string;
}

export function Divider({ label }: DividerProps) {
  return (
    <div className="flex items-center gap-3">
      <div className="h-px flex-1 bg-brand-border" />
      {label && (
        <span className="text-xs text-gray-500">{label}</span>
      )}
      <div className="h-px flex-1 bg-brand-border" />
    </div>
  );
}
